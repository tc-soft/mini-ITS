import { useState, useEffect } from 'react';
import { format } from 'date-fns';
import { enrollmentServices } from '../../services/EnrollmentServices';

const EnrollmentsList = (props) => {
    const {
        pagedQuery,
        setPagedQuery
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

    const handleSetResultsPerPage = (number) => {
        setPagedQuery(prevState => ({
            ...prevState,
            resultsPerPage: number,
            page: 1
        }));
    };

    const handleFirstPage = () => {
        if (enrollments.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: 1
            }));
        };
    };

    const handlePrevPage = () => {
        if (enrollments.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: enrollments.page - 1
            }));
        };
    };

    const handleNextPage = () => {
        if (enrollments.page < enrollments.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: enrollments.page + 1
            }));
        };
    };

    const handleLastPage = () => {
        if (enrollments.page < enrollments.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: enrollments.totalPages
            }));
        };
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await enrollmentServices.index(pagedQuery);
                if (response.ok) {
                    const data = await response.json();
                    setEnrollments(data);
                }
                else {
                    throw new Error('Network response was not ok');
                };
            }
            catch (error) {
                console.error('Error loading data:', error);
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
                        <th style={{ width: '07%' }}>Nr</th>
                        <th style={{ width: '07%' }}>Data wpr.</th>
                        <th style={{ width: '07%' }}>Data zak.</th>
                        <th style={{ width: '15%' }}>Opis</th>
                        <th style={{ width: '10%' }}>Zgłaszający</th>
                        <th style={{ width: '10%' }}>Dział docelowy</th>
                        <th style={{ width: '07%' }}>Priorytet</th>
                        <th style={{ width: '07%' }}>Status</th>
                        <th style={{ width: '05%' }}>Gotowe</th>
                        <th style={{ width: '25%' }}>Operacje</th>
                    </tr>
                </thead>
                <tbody>
                    {enrollments.results && enrollments.results.map((enrollment, index) => {
                        const dateaddenrollment = enrollment.dateAddEnrollment ? format(new Date(enrollment.dateAddEnrollment), 'dd.MM.yyyy') : '';
                        const dateendenrollment = enrollment.dateEndEnrollment ? format(new Date(enrollment.dateEndEnrollment), 'dd.MM.yyyy') : '';
                        return (
                            <tr key={index} style={{
                                backgroundColor: enrollment.priority === 2 ? 'lavenderblush' : 'inherit'
                            }}>
                                <td>{enrollment.nr}/{enrollment.year}</td>
                                <td>{dateaddenrollment}</td>
                                <td>{dateendenrollment}</td>
                                <td>{enrollment.description}</td>
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
                                <td></td>
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
            <br />
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <div>Ilość wyników na stronie : &nbsp;
                    <button
                        style={enrollments.resultsPerPage === 10 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(10) }}
                    >
                        10
                    </button>
                    <button
                        style={enrollments.resultsPerPage === 20 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(20) }}
                    >
                        20
                    </button>
                    <button
                        style={enrollments.resultsPerPage === 50 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(50) }}
                    >
                        50
                    </button>
                </div>
                <div>Strona {enrollments.page} z {enrollments.totalPages} &nbsp;
                    <button
                        onClick={() => { handleFirstPage() }}
                    >
                        &#60;&#60;
                    </button>
                    <button
                        onClick={() => { handlePrevPage() }}
                    >
                        &#60;
                    </button>
                    <button
                        onClick={() => { handleNextPage() }}
                    >
                        &#62;
                    </button>
                    <button
                        onClick={() => { handleLastPage() }}
                    >
                        &#62;&#62;
                    </button>
                 </div>
            </div>
        </>
    );
};

export default EnrollmentsList;