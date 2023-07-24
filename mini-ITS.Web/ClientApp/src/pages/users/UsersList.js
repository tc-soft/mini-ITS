import React, { useState, useEffect } from 'react';
import { usersServices } from '../../services/UsersServices';

const UsersList = (props) => {
    const {
        pagedQuery,
        setPagedQuery,
        activeDepartmentFilter,
        activeRoleFilter
    } = props;

    const [users, setUsers] = useState({
        results: null,
        page: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });

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

    useEffect(() => {
        const fetchData = async () => {
            try {
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
        <>
            <p>Lista użytkowników</p>

            <table>
                <thead>
                    <tr>
                        <th style={{ width: '05%' }}>Lp.</th>
                        <th style={{ width: '20%' }}>Login</th>
                        <th style={{ width: '20%' }}>Imię</th>
                        <th style={{ width: '20%' }}>Nazwisko</th>
                        <th style={{ width: '10%' }}>Dział</th>
                        <th style={{ width: '10%' }}>Rola</th>
                        <th style={{ width: '15%' }}></th>
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
                                <td></td>
                            </tr>
                        );
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
            <br />
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <div>Ilość wyników na stronie : &nbsp;
                    <button
                        style={users.resultsPerPage === 10 ? { backgroundColor: "lightblue" } : {}}
                        onClick={() => { handleSetResultsPerPage(10) }}
                    >
                        10
                    </button>
                    <button
                        style={users.resultsPerPage === 20 ? { backgroundColor: "lightblue" } : {}}
                        onClick={() => { handleSetResultsPerPage(20) }}
                    >
                        20
                    </button>
                    <button
                        style={users.resultsPerPage === 50 ? { backgroundColor: "lightblue" } : {}}
                        onClick={() => { handleSetResultsPerPage(50) }}
                    >
                        50
                    </button>
                </div>
                <div>Strona {users.page} z {users.totalPages} &nbsp;
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

export default UsersList;