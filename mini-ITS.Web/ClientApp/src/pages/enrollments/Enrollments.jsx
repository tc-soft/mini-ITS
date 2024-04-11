import { useState } from 'react';
import { Routes, Route } from 'react-router-dom';
import EnrollmentsList from './EnrollmentsList';
import EnrollmentsForm from './EnrollmentsForm';
import EnrollmentsAddDescription from './EnrollmentsAddDescription';
import EnrollmentsAddDescriptionSetEndDate from './EnrollmentsAddDescriptionSetEndDate';

const Enrollments = () => {
    const [pagedQuery, setPagedQuery] = useState({
        filter: null,
        sortColumnName: 'DateAddEnrollment',
        sortDirection: 'DESC',
        page: 1,
        resultsPerPage: 10
    });
    const [groupsPagedQuery, setGroupsPagedQuery] = useState({
        filter: null,
        sortColumnName: 'DateAddGroup',
        sortDirection: 'ASC',
        page: 1,
        resultsPerPage: 100
    });
    const [activeStateFilter, setActiveStateFilter] = useState('');
    const [activeDepartmentFilter, setActiveDepartmentFilter] = useState('');

    return (
        <Routes>
            <Route path='/' element={<EnrollmentsList />} />
            <Route index element={<EnrollmentsList
                pagedQuery={pagedQuery}
                setPagedQuery={setPagedQuery}
                activeStateFilter={activeStateFilter}
                setActiveStateFilter={setActiveStateFilter}
                activeDepartmentFilter={activeDepartmentFilter}
                setActiveDepartmentFilter={setActiveDepartmentFilter}
            />}
            />
            <Route path='/Detail/:enrollmentId' element={<EnrollmentsForm
                isMode={'Detail'}
                groupsPagedQuery={groupsPagedQuery}
                setGroupsPagedQuery={setGroupsPagedQuery}
            />}
            />
            <Route path='/Edit/:enrollmentId/AddDescription' element={<EnrollmentsAddDescription
            />}
            />
            <Route path='/Edit/:enrollmentId/AddDescriptionSetEndDate' element={<EnrollmentsAddDescriptionSetEndDate
            />}
            />
            <Route path='/Edit/:enrollmentId' element={<EnrollmentsForm
                isMode={'Edit'}
                groupsPagedQuery={groupsPagedQuery}
                setGroupsPagedQuery={setGroupsPagedQuery}
            />}
            />
            <Route path='/Create' element={<EnrollmentsForm
                isMode={'Create'}
                groupsPagedQuery={groupsPagedQuery}
                setGroupsPagedQuery={setGroupsPagedQuery}
            />}
            />
        </Routes>
    );
};

export default Enrollments;