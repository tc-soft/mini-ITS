import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { enrollmentServices } from '../../services/EnrollmentServices';

const Dashboard = () => {
    const { currentUser } = useAuth();

    const [pagedQuery, setPagedQuery] = useState(() => ({
        filter: currentUser.role === 'Administrator' ? [
            {
                name: 'State',
                operator: '<>',
                value: 'Closed'
            }
        ] : [
            {
                name: 'Department',
                operator: '=',
                value: currentUser.department
            }
        ],
        sortColumnName: 'DateAddEnrollment',
        sortDirection: 'DESC'
    }));

    const [numberOfNewEnrollments, setNumberOfNewEnrollments] = useState(null);
    const [numberOfAssignedEnrollments, setNumberOfAssignedEnrollmrnts] = useState(null);
    const [companyName, setCompanyName] = useState('');
    
    useEffect(() => {
        const fetchData = async () => {
            try {
                const enrollmentResponse = await enrollmentServices.index(pagedQuery);
                if (!enrollmentResponse.ok) {
                    throw new Error('Failed to fetch enrollments data');
                }
                const enrollmentsData = await enrollmentResponse.json();

                const numberOfNewEnrollments = enrollmentsData.results
                    ? enrollmentsData.results.filter(enrollment =>
                        enrollment.state === 'New' && enrollment.department === currentUser.department
                    ).length
                    : 0;

                const numberOfAssignedEnrollments = enrollmentsData.results
                    ? enrollmentsData.results.filter(enrollment =>
                        (enrollment.state === 'Assigned' || enrollment.state === 'ReOpened') &&
                        enrollment.department === currentUser.department
                    ).length
                    : 0;

                setNumberOfNewEnrollments(numberOfNewEnrollments);
                setNumberOfAssignedEnrollmrnts(numberOfAssignedEnrollments);

                const companyResponse = await fetch('/Company.json');
                if (!companyResponse.ok) {
                    throw new Error('Failed to fetch company data');
                }
                const companyData = await companyResponse.json();
                setCompanyName(companyData.companyName);
            }
            catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        setTimeout(fetchData, 0);
    }, [pagedQuery]);

    return (
        <div>
            <h3>Witaj {currentUser.firstName}</h3>
            <p>mini-ITS jest gotowy na Twoje działania.</p>
            <br />

            <p>
                Nowe zgłoszenia:&nbsp;<strong>{numberOfNewEnrollments}</strong>
            </p>
            <p>
                Otwarte&nbsp;zgłoszenia:&nbsp;<strong>{numberOfAssignedEnrollments}</strong>
            </p>
            <br />

            <Link to='/Enrollments'
                state={{
                    initialDepartmentFilter: currentUser.department,
                    initialStateFilter: 'New'
                }}
            >
                <button>
                    Nowe zgłoszenia
                </button>
            </Link>

            <div>
                <p>{companyName}</p>
                <p><strong>Dział: {currentUser.department}</strong></p>    
            </div>
        </div>
    );
};

export default Dashboard;