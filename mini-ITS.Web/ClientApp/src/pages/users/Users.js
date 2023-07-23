import React from 'react';
import { Routes, Route } from 'react-router-dom';
import UsersList from './UsersList';

const Users = () => {

    return (
        <Routes>
            <Route path="/" element={<UsersList />} />
        </Routes>
    );
};

export default Users;