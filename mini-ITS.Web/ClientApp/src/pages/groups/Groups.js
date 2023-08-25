import React from 'react';
import { Routes, Route } from 'react-router-dom';
import GroupsList from './GroupsList';

const Groups = () => {

    return (
        <Routes>
            <Route path='/' element={<GroupsList />} />
        </Routes>
    );
};

export default Groups;