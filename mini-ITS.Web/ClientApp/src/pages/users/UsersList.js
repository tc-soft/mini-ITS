import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { usersServices } from '../../services/UsersServices';
import ModalDialog from '../../components/Modal';
import { ReactComponent as IconAdd } from '../../images/iconAdd.svg';
import { ReactComponent as IconDetail } from '../../images/iconDetail.svg';
import { ReactComponent as IconEdit } from '../../images/iconEdit.svg';
import { ReactComponent as IconDelete } from '../../images/iconDelete.svg';
import { ReactComponent as IconFirstPage } from '../../images/iconFirstPage.svg';
import { ReactComponent as IconPrevPage } from '../../images/iconPrevPage.svg';
import { ReactComponent as IconNextPage } from '../../images/iconNextPage.svg';
import { ReactComponent as IconLastPage } from '../../images/iconLastPage.svg';

import '../../styles/pages/Users.scss';

const UsersList = (props) => {
    const {
        pagedQuery,
        setPagedQuery,
        activeDepartmentFilter,
        setActiveDepartmentFilter,
        activeRoleFilter,
        setActiveRoleFilter
    } = props;
    const { currentUser } = useAuth();

    const [users, setUsers] = useState({
        results: null,
        page: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });

    const [mapDepartment, setMapDepartment] = useState([]);
    const [mapRole, setMapRole] = useState([]);

    const [modalDialogOpen, setModalDialogOpen] = useState(false);
    const [modalDialogType, setModalDialogType] = useState('');
    const [modalDialogTitle, setModalDialogTitle] = useState('');
    const [modalDialogMessage, setModalDialogMessage] = useState('');
    const [modalDialogUserId, setModalDialogUserId] = useState('');
    const [modalDialogUserLogin, setModalDialogUserLogin] = useState('');

    const handleModalClose = () => {
        setModalDialogType('');
        setModalDialogTitle('');
        setModalDialogMessage('')
        setModalDialogUserId('');
        setModalDialogUserLogin('');
        setModalDialogOpen(false);
    };

    const handleModalConfirm = async () => {
        switch (modalDialogType) {
            case 'Dialog':
                setModalDialogOpen(false);
                await handleDeleteStage2(modalDialogUserId, modalDialogUserLogin);
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

    const handleRoleFilter = (event) => {
        if (pagedQuery.filter && pagedQuery.filter.find(x => x.name === 'Role')) {
            setPagedQuery(prevState => ({
                ...prevState,
                filter: [...prevState.filter.filter(x => x.name !== 'Role'), {
                    name: 'Role',
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
                    name: 'Role',
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
                    name: 'Role',
                    operator: '=',
                    value: event.target.value
                }],
                page: 1
            })
            );
        };

        setActiveRoleFilter(event.target.value);
    };

    const handleSetResultsPerPage = (number) => {
        setPagedQuery(prevState => ({
            ...prevState,
            resultsPerPage: number,
            page: 1
        }));
    };

    const handleFirstPage = () => {
        if (users.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: 1
            }));
        };
    };

    const handlePrevPage = () => {
        if (users.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: users.page - 1
            }));
        };
    };

    const handleNextPage = () => {
        if (users.page < users.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: users.page + 1
            }));
        };
    };

    const handleLastPage = () => {
        if (users.page < users.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: users.totalPages
            }));
        };
    };

    const handleDeleteStage1 = (id, login) => {
        if (id === currentUser.id) {
            setModalDialogType('Error');
            setModalDialogTitle('Usuwanie użytkownika');
            setModalDialogMessage(`Nie można usunąć aktualnie zalogowanego użytkownika ${login}`);
            setModalDialogOpen(true);
            return;
        };

        if (login === 'admin') {
            setModalDialogType('Error');
            setModalDialogTitle('Usuwanie użytkownika');
            setModalDialogMessage(`Nie można usunąć użytkownika ${login}`);
            setModalDialogOpen(true);
            return;
        };

        if (currentUser.role === 'Administrator') {
            setModalDialogType('Dialog');
            setModalDialogTitle('Usuwanie użytkownika');
            setModalDialogMessage(`Czy na pewno chcesz usunąć użytkownika ${login}?`);
            setModalDialogUserId(id);
            setModalDialogUserLogin(login);
            setModalDialogOpen(true);
        };
    };

    const handleDeleteStage2 = async (id, login) => {
        try {
            const deleteResponse = await usersServices.delete(id);
            if (!deleteResponse.ok) {
                throw new Error('Usunięcie użytkownika nie powiodło się!');
            };

            const indexResponse = await usersServices.index(pagedQuery);
            if (!indexResponse.ok) {
                throw new Error('Błąd podczas pobierania zaktualizowanej listy użytkowników.');
            };

            setTimeout(() => {
                setModalDialogType('Information');
                setModalDialogTitle('Usuwanie użytkownika');
                setModalDialogMessage(`Pomyślnie usunięto użytkownika ${login}.`);
                setModalDialogOpen(true);
            }, 400);

            const data = await indexResponse.json();
            setUsers(data);
        }
        catch (error) {
            alert(error.message);
        };
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                const departmentResponse = await fetch('/Department.json');
                const departmentData = await departmentResponse.json();
                setMapDepartment(departmentData.map((item) => item.value === '' ? { ...item, name: 'Wszyscy' } : item));

                const roleResponse = await fetch('/Role.json');
                const roleData = await roleResponse.json();
                setMapRole(roleData.map((item) => item.value === '' ? { ...item, name: 'Wszyscy' } : item));

                const response = await usersServices.index(pagedQuery);
                if (response.ok) {
                    const data = await response.json();
                    setUsers(data);
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
    }, [pagedQuery, activeDepartmentFilter, activeRoleFilter]);

    return (
        <div className='usersList'>
            <ModalDialog
                modalDialogOpen={modalDialogOpen}
                modalDialogType={modalDialogType}
                modalDialogTitle={modalDialogTitle}
                modalDialogMessage={modalDialogMessage}
                modalDialogUserId={modalDialogUserId}
                modalDialogUserLogin={modalDialogUserLogin}

                handleModalConfirm={handleModalConfirm}
                handleModalClose={handleModalClose}
            />
            <div className='usersList-panel'>
                <div className='usersList-panel-tittle'>
                    <p>Lista użytkowników</p>
                    <Link to='Create'>
                        <button title='Dodaj nowy'>
                            <IconAdd />
                            <span>Dodaj</span>
                        </button>
                    </Link>
                </div>
                <div className='usersList-panel-filter'>
                    <div>
                        Dział : &nbsp;
                        <select value={activeDepartmentFilter} onChange={handleDepartmentFilter}>
                            {mapDepartment.map((x, y) => <option key={y} value={x.value}>{x.name}</option>)}
                        </select>
                    </div>
                    <div>
                        Rola : &nbsp;
                        <select value={activeRoleFilter} onChange={handleRoleFilter}>
                            {mapRole.map((x, y) => <option key={y} value={x.value}>{x.name}</option>)}
                        </select>
                    </div>
                </div>
            </div>
            <table className='usersList-table'>
                <thead>
                    <tr>
                        <th style={{ width: '05%' }}>Lp.</th>
                        <th style={{ width: '20%' }}>Login</th>
                        <th style={{ width: '20%' }}>Imię</th>
                        <th style={{ width: '20%' }}>Nazwisko</th>
                        <th style={{ width: '10%' }}>Dział</th>
                        <th style={{ width: '10%' }}>Rola</th>
                        <th style={{ width: '15%' }}>Operacje</th>
                    </tr>
                </thead>
                <tbody>
                    {users.results && users.results.map((user, index) => {
                        const record = index + ((users.page - 1) * users.resultsPerPage) + 1;
                        return (
                            <tr key={index}>
                                <td>{record}</td>
                                <td>{user.login}</td>
                                <td>{user.firstName}</td>
                                <td>{user.lastName}</td>
                                <td>{user.department}</td>
                                <td>{user.role}</td>
                                <td>
                                    <span>
                                        <Link to={`Detail/${user.id}`}>
                                            <IconDetail title='Szczegóły' />
                                        </Link>
                                    </span>
                                    <span>
                                        <Link to={`Edit/${user.id}`}>
                                            <IconEdit title='Edycja' />
                                        </Link>
                                    </span>
                                    <span
                                        title='Usuń'
                                        onClick={() => handleDeleteStage1(user.id, user.login)}
                                        style={{ cursor: 'pointer' }}
                                    >
                                        <IconDelete title='Usuń' />
                                    </span>
                                </td>
                            </tr>
                        )
                    }
                    )}
                    {
                        users.results === null &&
                        <tr className='usersList-table--info'>
                            <td colSpan='7'>
                                <div>Brak danych...</div>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
            <div className='usersList-paginationPanel'>
                <div>Ilość wyników na stronie : &nbsp;
                    <button
                        className={users.resultsPerPage === 10 ? 'usersList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(10) }}
                    >
                        10
                    </button>
                    <button
                        className={users.resultsPerPage === 20 ? 'usersList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(20) }}
                    >
                        20
                    </button>
                    <button
                        className={users.resultsPerPage === 50 ? 'usersList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(50) }}
                    >
                        50
                    </button>
                </div>
                <div>Strona {users.page} z {users.totalPages} &nbsp;
                    <button
                        onClick={() => { handleFirstPage() }}
                    >
                        <IconFirstPage title='Początek' />
                    </button>

                    <button
                        onClick={() => { handlePrevPage() }}
                    >
                        <IconPrevPage title='Wstecz' />
                    </button>

                    <button
                        onClick={() => { handleNextPage() }}
                    >
                        <IconNextPage title='Następna' />
                    </button>

                    <button
                        onClick={() => { handleLastPage() }}
                    >
                        <IconLastPage title='Koniec' />
                    </button>
                </div>
            </div>
        </div>
     );
};

export default UsersList;