import { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import UsersList from './UsersList';
import UsersForm from './UsersForm';
import UsersFormPassword from './UsersFormPassword';

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
            <Route path='/Detail/:userId' element={<UsersForm isMode={'Detail'} />} />
            <Route path='/Edit/:userId' element={<UsersForm isMode={'Edit'} />} />
            <Route path='/Create' element={<UsersForm isMode={'Create'} />} />
            <Route path='/ChangePassword' element={<UsersFormPassword/>} />
        </Routes>
    );
};

export default Users;