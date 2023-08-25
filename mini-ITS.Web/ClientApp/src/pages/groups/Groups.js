import React from 'react';
import { Routes, Route } from 'react-router-dom';

const Groups = () => {

    return (
        <Routes>
            <Route path="/" element={<p>Grupy</p>} />
        </Routes>
    );
};

export default Groups;