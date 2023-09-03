import React, { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import GroupsList from './GroupsList';

const Groups = () => {
    const [pagedQuery, setPagedQuery] = useState({
        filter: null,
        sortColumnName: 'GroupName',
        sortDirection: 'ASC',
        page: 1,
        resultsPerPage: 10
    });

    return (
        <Routes>
            <Route index element={<GroupsList
                pagedQuery={pagedQuery}
                setPagedQuery={setPagedQuery}
                />}
            />
        </Routes>
    );
};

export default Groups;