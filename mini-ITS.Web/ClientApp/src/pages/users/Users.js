import React, { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import UsersList from './UsersList';
import UsersForm from './UsersForm';

const Users = () => {
    const [pagedQuery, setPagedQuery] = useState({
        filter: null,
        sortColumnName: 'Login',
        sortDirection: 'ASC',
        page: 1,
        resultsPerPage: 10
    });
    const [activeDepartmentFilter, setActiveDepartmentFilter] = useState('');
    const [activeRoleFilter, setActiveRoleFilter] = useState('');

    return (
        <Routes>
            <Route index element={<UsersList
                pagedQuery={pagedQuery}
                setPagedQuery={setPagedQuery}
                activeDepartmentFilter={activeDepartmentFilter}
                setActiveDepartmentFilter={setActiveDepartmentFilter}
                activeRoleFilter={activeRoleFilter}
                setActiveRoleFilter={setActiveRoleFilter}
                />}
            />
            <Route path="/Detail/:userId" element={<UsersForm isMode={'Detail'} />} />
            <Route path="/Edit/:userId" element={<UsersForm isMode={'Edit'} />} />
        </Routes>
    );
};

export default Users;