import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import { enrollmentServices } from '../../services/EnrollmentServices';
import iconAdd from '../../images/iconAdd.svg';
import iconDetail from '../../images/iconDetail.svg';
import iconEdit from '../../images/iconEdit.svg';
import iconDelete from '../../images/iconDelete.svg';
import iconFirstPage from '../../images/iconFirstPage.svg';
import iconPrevPage from '../../images/iconPrevPage.svg';
import iconNextPage from '../../images/iconNextPage.svg';
import iconLastPage from '../../images/iconLastPage.svg';

import '../../styles/pages/Enrollments.scss';

const EnrollmentsList = (props) => {
    const {
        pagedQuery,
        setPagedQuery,
        activeStateFilter,
        setActiveStateFilter,
        activeDepartmentFilter,
        setActiveDepartmentFilter
    } = props;

    const [enrollments, setEnrollments] = useState({
        results: null,
        page: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });

    const [mapDepartment, setMapDepartment] = useState([]);

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

    const handleStateFilter = (event) => {
        if (pagedQuery.filter && pagedQuery.filter.find(x => x.name === 'State')) {
            setPagedQuery(prevState => ({
                ...prevState,
                filter: [...prevState.filter.filter(x => x.name !== 'State'), {
                    name: 'State',
                    operator: '=',
                    value: event.target.value
                }],
                page: 1
            })
            );
        }
        else if (pagedQuery.filter) {
            setPagedQuery(prevState => ({
                ...prevState,
                filter: [...prevState.filter, {
                    name: 'State',
                    operator: '=',
                    value: event.target.value
                }],
                page: 1
            })
            );
        }
        else {
            setPagedQuery(prevState => ({
                ...prevState,
                filter: [{
                    name: 'State',
                    operator: '=',
                    value: event.target.value
                }],
                page: 1
            })
            );
        };

        setActiveStateFilter(event.target.value);
    };

    const handleDepartmentFilter = (event) => {
        if (pagedQuery.filter && pagedQuery.filter.find(x => x.name === 'Department')) {
            setPagedQuery(prevState => ({
                ...prevState,
                filter: [...prevState.filter.filter(x => x.name !== 'Department'), {
                    name: 'Department',
                    operator: '=',
                    value: event.target.value
                }],
                page: 1
            })
            );
        }
        else if (pagedQuery.filter) {
            setPagedQuery(prevState => ({
                ...prevState,
                filter: [...prevState.filter, {
                    name: 'Department',
                    operator: '=',
                    value: event.target.value
                }],
                page: 1
            })
            );
        }
        else {
            setPagedQuery(prevState => ({
                ...prevState,
                filter: [{
                    name: 'Department',
                    operator: '=',
                    value: event.target.value
                }],
                page: 1
            })
            );
        };

        setActiveDepartmentFilter(event.target.value);
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
                const departmentResponse = await fetch('/Department.json');
                const departmentData = await departmentResponse.json();
                setMapDepartment(departmentData.map((item) => item.value === '' ? { ...item, name: 'Wszyscy' } : item));

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
    }, [pagedQuery, activeStateFilter, activeDepartmentFilter]);

    return (
        <div className='enrollmentsList'>
            <div className='enrollmentsList-panel'>
                <div className='enrollmentsList-panel-tittle'>
                    <p>Lista zgłoszeń</p>
                    <Link>
                        <button title='Dodaj nowe'>
                            <img src={iconAdd} alt='iconAdd' />
                            <span>Dodaj</span>
                        </button>
                    </Link>
                </div>
                <div className='enrollmentsList-panel-filter'>
                    <div>
                        Status : &nbsp;
                        <select value={activeStateFilter} onChange={handleStateFilter}>
                            {Object.entries(mapState).map(([value, name], index) => (
                                <option key={index} value={value}>
                                    {name}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div>
                        Dział docelowy : &nbsp;
                        <select value={activeDepartmentFilter} onChange={handleDepartmentFilter}>
                            {mapDepartment.map((department, index) => (
                                <option key={index} value={department.value}>
                                    {department.name}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>
            </div>
            <table className='enrollmentsList-table'>
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
            <div className='enrollmentsList-paginationPanel'>
                <div>Ilość wyników na stronie : &nbsp;
                    <button
                        className={enrollments.resultsPerPage === 10 ? 'enrollmentsList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(10) }}
                    >
                        10
                    </button>
                    <button
                        className={enrollments.resultsPerPage === 20 ? 'enrollmentsList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(20) }}
                    >
                        20
                    </button>
                    <button
                        className={enrollments.resultsPerPage === 50 ? 'enrollmentsList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(50) }}
                    >
                        50
                    </button>
                </div>
                <div>Strona {enrollments.page} z {enrollments.totalPages} &nbsp;
                    <button
                        onClick={() => { handleFirstPage() }}
                    >
                        <img src={iconFirstPage} alt='Początek' title='Początek' />
                    </button>
                    <button
                        onClick={() => { handlePrevPage() }}
                    >
                        <img src={iconPrevPage} alt='Wstecz' title='Wstecz' />
                    </button>
                    <button
                        onClick={() => { handleNextPage() }}
                    >
                        <img src={iconNextPage} alt='Następna' title='Następna' />
                    </button>
                    <button
                        onClick={() => { handleLastPage() }}
                    >
                        <img src={iconLastPage} alt='Koniec' title='Koniec' />
                    </button>
                </div>
            </div>
        </div>
    );
};

export default EnrollmentsList;