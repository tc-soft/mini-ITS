import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { enrollmentServices } from '../../services/EnrollmentServices';
import welcomeImage from '../../images/welcomeImage.svg';

import '../../styles/pages/Dashboard.scss';

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
    const [companyLink, setCompanyLink] = useState('');

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
                setCompanyLink(companyData.companyLink);
            }
            catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        setTimeout(fetchData, 0);
    }, [pagedQuery]);

    return (
        <div className='dashboard'>
            <div className='dashboard__panel'>
                <div className='dashboard__info'>
                    <h3 className='dashboard__titleText1'>Witaj {currentUser.firstName}</h3>
                    <p className='dashboard__titleText2'>mini-ITS jest gotowy na Twoje działania.</p>

                    <p className="dashboard__text">
                        Nowe zgłoszenia:&nbsp;<strong className="dashboard__text--red">{numberOfNewEnrollments}</strong>
                    </p>
                    <p className="dashboard__text">
                        Otwarte&nbsp;zgłoszenia:&nbsp;<strong>{numberOfAssignedEnrollments}</strong>
                    </p>

                    <Link to='/Enrollments'
                        state={{
                            initialDepartmentFilter: currentUser.department,
                            initialStateFilter: 'New'
                        }}
                    >
                        <button className='dashboard__button'>
                            Nowe zgłoszenia
                        </button>
                    </Link>

                    <div className='dashboard__company'>
                        <a className="dashboard__logo" href={companyLink}>
                            <img src='/Logo.svg' alt="Welcome image" />
                        </a>
                        <p className='dashboard__companyText'>{companyName}</p>
                        <p className='dashboard__companyText'><strong>Dział: {currentUser.department}</strong></p>    
                    </div>
                </div>
                <div className='dashboard__image'>
                    <img src={welcomeImage} alt='Welcome image' />
                </div>
            </div>
        </div>
    );
};

export default Dashboard;