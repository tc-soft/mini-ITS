import React, { useState, useEffect } from 'react';
import { usersServices } from '../../services/UsersServices';

const UsersList = (props) => {
    const {
        pagedQuery,
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
                    <button style={{ backgroundColor: "lightblue" }}>10</button>
                    <button>20</button>
                    <button>50</button>
                </div>
                <div>Strona 1 z 5 &nbsp;
                    <button>&#60;&#60;</button>
                    <button>&#60;</button>
                    <button>&#62;</button>
                    <button>&#62;&#62;</button>
                </div>
            </div>
        </>
    );
};

export default UsersList;