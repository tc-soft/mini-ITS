import React, { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import GroupsList from './GroupsList';
import GroupsForm from './GroupsForm';

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
            <Route path='/Detail/:groupId' element={<GroupsForm isMode={'Detail'} />} />
            <Route path='/Edit/:groupId' element={<GroupsForm isMode={'Edit'} />} />
            <Route path='/Create' element={<GroupsForm isMode={'Create'} />} />
        </Routes>
    );
};

export default Groups;