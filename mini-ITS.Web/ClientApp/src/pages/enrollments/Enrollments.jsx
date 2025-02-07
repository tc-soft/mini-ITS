import { useState } from 'react';
import { Routes, Route, useLocation } from 'react-router-dom';
import EnrollmentsList from './EnrollmentsList';
import EnrollmentsForm from './EnrollmentsForm';

const Enrollments = () => {
    const location = useLocation();
    const initialStateFilter = location.state?.initialStateFilter || '';
    const initialDepartmentFilter = location.state?.initialDepartmentFilter || '';

    const [pagedQuery, setPagedQuery] = useState({
        filter: initialDepartmentFilter ? [
            {
                name: 'State',
                operator: '=',
                value: initialStateFilter
            },
            {
                name: 'Department',
                operator: '=',
                value: initialDepartmentFilter
            }
        ] : null,
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
    const [activeStateFilter, setActiveStateFilter] = useState(initialStateFilter);
    const [activeDepartmentFilter, setActiveDepartmentFilter] = useState(initialDepartmentFilter);

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
            <Route path='/:enrollmentId' element={<EnrollmentsForm
                isMode={'Detail'}
                groupsPagedQuery={groupsPagedQuery}
                setGroupsPagedQuery={setGroupsPagedQuery}
            />}
            />
            <Route path='/:enrollmentId/Edit' element={<EnrollmentsForm
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