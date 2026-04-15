import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import { enrollmentServices } from '../../services/EnrollmentServices';
import iconDetail from '../../images/iconDetail.svg';

const EnrollmentsReport = (props) => {
    const {
        pagedQuery
    } = props;

    const [enrollments, setEnrollments] = useState({
        results: null,
        page: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });

    const mapPriority = {
        '0': 'Normalny',
        '1': 'Wysoki',
        '2': 'Krytyczny'
    };

    const mapState = {
        '': 'Wszystkie',
        'New': 'Nowy',
        'Assigned': 'W trakcie',
        'Closed': 'Zamknięte',
        'ReOpened': 'Otwarte ponownie'
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                const enrollmentResponse = await enrollmentServices.index(pagedQuery);
                if (!enrollmentResponse.ok) {
                    throw new Error('Failed to fetch enrollments data');
                };
                const enrollmentsData = await enrollmentResponse.json();
                setEnrollments(enrollmentsData);
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        setTimeout(fetchData, 0);
    }, [pagedQuery]);

    return (
        <>
            <p>Lista zgłoszeń</p>
            <table>
                <thead>
                    <tr>
                        <th style={{ width: '05%' }}>Nr</th>
                        <th style={{ width: '05%' }}>Data wpr.</th>
                        <th style={{ width: '05%' }}>Data zak.</th>
                        <th style={{ width: '25%' }}>Opis</th>
                        <th style={{ width: '10%' }}>Zgłaszający</th>
                        <th style={{ width: '10%' }}>Dział docelowy</th>
                        <th style={{ width: '06%' }}>Priorytet</th>
                        <th style={{ width: '05%' }}>Status</th>
                        <th style={{ width: '06%' }}>Gotowe</th>
                        <th style={{ width: '23%' }}>Operacje</th>
                    </tr>
                </thead>
                <tbody>
                    {enrollments.results && enrollments.results.map((enrollment, index) => {
                        const dateaddenrollment = enrollment.dateAddEnrollment ? format(new Date(enrollment.dateAddEnrollment), 'dd.MM.yyyy') : '';
                        const dateendenrollment = enrollment.dateEndDeclareByDepartment ? format(new Date(enrollment.dateEndDeclareByDepartment), 'dd.MM.yyyy') : '';
                        return (
                            <tr key={index} style={{
                                backgroundColor: enrollment.priority === 2 ? 'lavenderblush' : 'inherit'
                            }}>
                                <td>{enrollment.nr}/{enrollment.year}</td>
                                <td>{dateaddenrollment}</td>
                                <td>{dateendenrollment}</td>
                                <td>
                                    {enrollment.description.length > 60
                                        ? `${enrollment.description.substring(0, 60)}...`
                                        : enrollment.description}
                                </td>
                                <td>{enrollment.userAddEnrollmentFullName}</td>
                                <td>{enrollment.department}</td>
                                <td>{mapPriority[enrollment.priority] || enrollment.priority}</td>
                                <td>{mapState[enrollment.state] || enrollment.state}</td>

                                <td
                                    style={{
                                        position: 'relative',
                                        background:
                                            enrollment.actionRequest !== 0
                                                ? 'repeating-linear-gradient(-45deg, rgba(0, 0, 0, 0.3), rgba(0, 0, 0, 0.3) 1px, transparent 2px, transparent 2px, rgba(0, 0, 0, 0.3) 3px)'
                                                : 'inherit'
                                    }}
                                >
                                    <input
                                        type='checkbox'
                                        checked={enrollment.readyForClose}
                                        readOnly
                                    />
                                </td>
                                <td>
                                    <span>
                                        <Link to={`../${enrollment.id}`}>
                                            <img src={iconDetail} alt='Szczegóły' title='Szczegóły' />
                                        </Link>
                                    </span>
                                </td>
                            </tr>
                        );
                    }
                    )}
                    {
                        enrollments.results === null &&
                        <tr>
                            <td colSpan='10'>
                                <div>Brak danych...</div>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </>
    );
};

export default EnrollmentsReport;