import { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import EnrollmentsList from './EnrollmentsList';

const Enrollments = () => {
    const [pagedQuery, setPagedQuery] = useState({
        filter: null,
        sortColumnName: 'DateAddEnrollment',
        sortDirection: 'DESC',
        page: 1,
        resultsPerPage: 10
    });

    return (
        <Routes>
            <Route path="/" element={<EnrollmentsList />} />
            <Route index element={<EnrollmentsList
                pagedQuery={pagedQuery}
                setPagedQuery={setPagedQuery}
            />}
            />
        </Routes>
    );
};

export default Enrollments;