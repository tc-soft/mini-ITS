import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { format } from 'date-fns';
import { enrollmentServices } from '../../services/EnrollmentServices';
import ModalDialog from '../../components/Modal';
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

    const { currentUser } = useAuth();

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
        'New': 'Nowy',
        'Assigned': 'W trakcie',
        'Closed': 'Zamknięte',
        'ReOpened': 'Otwarte ponownie'
    };

    const [maxNumber, setMaxNumber] = useState('');

    const [modalDialogOpen, setModalDialogOpen] = useState(false);
    const [modalDialogType, setModalDialogType] = useState('');
    const [modalDialogTitle, setModalDialogTitle] = useState('');
    const [modalDialogMessage, setModalDialogMessage] = useState('');
    const [modalDialogEnrollmentId, setModalDialogEnrollmentId] = useState('');
    const [modalDialogEnrollmentNr, setModalDialogEnrollmentNr] = useState('');
    const [modalDialogEnrollmentYear, setModalDialogEnrollmentYear] = useState('');

    const handleModalClose = () => {
        setModalDialogType('');
        setModalDialogTitle('');
        setModalDialogMessage('')
        setModalDialogEnrollmentId('');
        setModalDialogEnrollmentNr('');
        setModalDialogEnrollmentYear('');
        setModalDialogOpen(false);
    };

    const handleModalConfirm = async () => {
        switch (modalDialogType) {
            case 'Dialog':
                setModalDialogOpen(false);
                await handleDeleteStage2(modalDialogEnrollmentId, modalDialogEnrollmentNr, modalDialogEnrollmentYear);
                break;
            case 'Information':
                handleModalClose();
                break;
            case 'Error':
                handleModalClose();
                break;
            default:
                break;
        };
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

    const handleDeleteStage1 = (enrollmentId, enrollmentNr, enrollmentYear) => {
        try {
            setModalDialogType('Dialog');
            setModalDialogTitle('Usuwanie zgłoszenia');
            setModalDialogMessage(`Czy na pewno chcesz usunąć zgłoszenie ${enrollmentNr}/${enrollmentYear}?`);
            setModalDialogEnrollmentId(enrollmentId);
            setModalDialogEnrollmentNr(enrollmentNr);
            setModalDialogEnrollmentYear(enrollmentYear);
            setModalDialogOpen(true);
        }
        catch (error) {
            console.error(error);
        };
    };

    const handleDeleteStage2 = async (enrollmentId, enrollmentNr, enrollmentYear) => {
        try {
            const deleteResponse = await enrollmentServices.delete(enrollmentId);
            if (!deleteResponse.ok) {
                throw new Error('Failed to delete the enrollment!');
            };

            const indexResponse = await enrollmentServices.index(pagedQuery);
            if (!indexResponse.ok) {
                throw new Error('Failed fetching updated list of enrollments.');
            };

            setTimeout(() => {
                setModalDialogType('Information');
                setModalDialogTitle('Usuwanie zgłoszenia');
                setModalDialogMessage(`Pomyślnie usunięto zgłoszenie ${enrollmentNr}/${enrollmentYear}.`);
                setModalDialogOpen(true);
            }, 50);

            const data = await indexResponse.json();
            setEnrollments(data);

            const maxNumberResponse = await enrollmentServices.getMaxNumber(new Date().getFullYear());
            if (!maxNumberResponse.ok) {
                throw new Error('Failed to fetch getMaxNumber value');
            };
            const { maxNumber } = await maxNumberResponse.json();
            setMaxNumber(maxNumber);
        }
        catch (error) {
            alert(error.message);
        };
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                const currentYear = new Date().getFullYear();

                const departmentResponse = await fetch('/Department.json');
                if (!departmentResponse.ok) {
                    throw new Error('Failed to fetch department data');
                };
                const departmentData = await departmentResponse.json();
                setMapDepartment(departmentData.map((item) => item.value === '' ? { ...item, name: 'Wszyscy' } : item));

                const maxNumberResponse = await enrollmentServices.getMaxNumber(currentYear);
                if (!maxNumberResponse.ok) {
                    throw new Error('Failed to fetch getMaxNumber value');
                };
                const { maxNumber } = await maxNumberResponse.json();
                setMaxNumber(maxNumber);

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
    }, [pagedQuery, activeStateFilter, activeDepartmentFilter]);

    return (
        <div className='enrollmentsList'>
            <ModalDialog
                modalDialogOpen={modalDialogOpen}
                modalDialogType={modalDialogType}
                modalDialogTitle={modalDialogTitle}
                modalDialogMessage={modalDialogMessage}

                handleModalConfirm={handleModalConfirm}
                handleModalClose={handleModalClose}
            />
            <div className='enrollmentsList-panel'>
                <div className='enrollmentsList-panel-tittle'>
                    <p>Lista zgłoszeń</p>
                    <Link to='Create'>
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
                            <option value=''>-- Wszystkie --</option>
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
                            <option value=''>-- Wszystkie --</option>
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
                        <th style={{ width: '05%' }}>Nr</th>
                        <th style={{ width: '05%' }}>Data wpr.</th>
                        <th style={{ width: '05%' }}>Data zak.</th>
                        <th style={{ width: '25%' }}>Opis</th>
                        <th style={{ width: '10%' }}>Zgłaszający</th>
                        <th style={{ width: '10%' }}>Dział docelowy</th>
                        <th style={{ width: '05%' }}>Priorytet</th>
                        <th style={{ width: '05%' }}>Status</th>
                        <th style={{ width: '05%' }}>Gotowe</th>
                        <th style={{ width: '25%' }}>Operacje</th>
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
                                        <Link to={`${enrollment.id}`}>
                                            <img src={iconDetail} alt='Szczegóły' title='Szczegóły' />
                                        </Link>
                                    </span>
                                    <span>
                                        {
                                            (currentUser && (
                                                currentUser.role === 'Administrator' ||
                                                (
                                                    enrollment.state !== 'Closed' &&
                                                    (
                                                        currentUser.role === 'Manager' ||
                                                        currentUser.department === enrollment.department ||
                                                        currentUser.id === enrollment.userAddEnrollment
                                                    ) &&
                                                    !(
                                                        currentUser.role === 'User' &&
                                                        enrollment.state === 'New' &&
                                                        currentUser.department === enrollment.department
                                                    )
                                                )
                                            )) &&
                                            <Link to={`${enrollment.id}/Edit`}>
                                                <img src={iconEdit} alt='Edycja' title='Edycja' />
                                            </Link>
                                        }
                                    </span>
                                    <span>
                                        {
                                            (currentUser && (
                                                currentUser.role === 'Administrator' ||
                                                (
                                                    (enrollment.state === 'New' && (currentUser.id === enrollment.userAddEnrollment)) &&
                                                    (enrollment.nr === maxNumber && enrollment.year === new Date().getFullYear())
                                                )
                                            )) &&
                                            (
                                                <span
                                                    title='Usuń'
                                                    onClick={() => handleDeleteStage1(enrollment.id, enrollment.nr, enrollment.year)}
                                                    style={{ cursor: 'pointer' }}
                                                >
                                                    <img src={iconDelete} alt='Usuń' title='Usuń' />
                                                </span>
                                            )}
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