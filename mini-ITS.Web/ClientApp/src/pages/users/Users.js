import React from 'react';
import { Routes, Route } from 'react-router-dom';

const Users = () => {

    return (
        <Routes>
            <Route path="/" element={<p>Użytkownicy</p>} />
        </Routes>
    );
};

export default Users;