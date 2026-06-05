import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import DatePicker from 'react-datepicker';
import { enrollmentServices } from '../../services/EnrollmentServices';
import pl from 'date-fns/locale/pl';
import iconDetail from '../../images/iconDetail.svg';

import 'react-datepicker/dist/react-datepicker.css';

const EnrollmentsReport = (props) => {
    const {
        pagedQuery,
        setPagedQuery
    } = props;

    const [enrollments, setEnrollments] = useState({
        results: null,
        page: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });

    const mapPriority = {
        '0': 'Normalny',
        '1': 'Wysoki',
        '2': 'Krytyczny'
    };

    const mapState = {
        '': 'Wszystkie',
        'New': 'Nowy',
        'Assigned': 'W trakcie',
        'Closed': 'Zamknięte',
        'ReOpened': 'Otwarte ponownie'
    };

    const [dateFromFilter, setDateFromFilter] = useState(null);
    const [dateToFilter, setDateToFilter] = useState(null);
    const [searchDescription, setSearchDescription] = useState('');
    const [usersAddEnrollments, setUsersAddEnrollments] = useState([]);
    const [selectedUserAddEnrollment, setSelectedUserAddEnrollment] = useState('');
    const [departments, setDepartments] = useState([]);
    const [selectedDepartment, setSelectedDepartment] = useState('');
    const [priority, setPriority] = useState('');
    const [state, setState] = useState('');
    const [readyForClose, setReadyForClose] = useState('');

    const handleSetResultsPerPage = (number) => {
        setPagedQuery(prevState => ({
            ...prevState,
            resultsPerPage: number,
            page: 1
        }));
    };

    const handleFirstPage = () => {
        if (enrollments.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: 1
            }));
        };
    };

    const handlePrevPage = () => {
        if (enrollments.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: enrollments.page - 1
            }));
        };
    };

    const handleNextPage = () => {
        if (enrollments.page < enrollments.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: enrollments.page + 1
            }));
        };
    };

    const handleLastPage = () => {
        if (enrollments.page < enrollments.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: enrollments.totalPages
            }));
        };
    };

    const handleSort = (columnName) => {
        setPagedQuery(prev => {
            const same = prev.sortColumnName === columnName;
            const nextDir = same ? (prev.sortDirection === 'ASC' ? 'DESC' : 'ASC') : 'ASC';
            return { ...prev, page: 1, sortColumnName: columnName, sortDirection: nextDir };
        });
    };

    const sortIndicator = (col) => {
        if (!pagedQuery || pagedQuery.sortColumnName !== col) return '';
        return pagedQuery.sortDirection === 'ASC' ? ' ▲' : ' ▼';
    };

    const upsertFilter = (name, operator, valueOrNull) => {
        setPagedQuery(prev => {
            const next = { ...prev };
            const current = Array.isArray(prev.filter) ? prev.filter : [];
            const filtered = current.filter(f => !(f.name === name && f.operator === operator));
            if (valueOrNull == null) {
                next.filter = filtered;
            } else {
                next.filter = [...filtered, { name, operator, value: valueOrNull }];
            }
            next.page = 1;
            return next;
        });
    };

    const handleDateFromFilterChange = (date) => {
        setDateFromFilter(date);
        if (!date) {
            upsertFilter('DateAddEnrollment', '>=', null);
            return;
        }

        const isoStart = new Date(
            date.getFullYear(),
            date.getMonth(),
            date.getDate(),
            0, 0, 0, 0
        ).toISOString();
        upsertFilter('DateAddEnrollment', '>=', isoStart);
    };

    const handleDateToFilterChange = (date) => {
        setDateToFilter(date);
        if (!date) {
            upsertFilter('DateAddEnrollment', '<=', null);
            return;
        }

        const isoEnd = new Date(
            date.getFullYear(),
            date.getMonth(),
            date.getDate(),
            23, 59, 59, 999
        ).toISOString();
        upsertFilter('DateAddEnrollment', '<=', isoEnd);
    };

    const handleUserAddEnrollmentChange = (e) => {
        const val = e.target.value || '';
        setSelectedUserAddEnrollment(val);
        upsertFilter('UserAddEnrollment', '=', val === '' ? null : val);

        setPagedQuery(prev => ({ ...prev, page: 1 }));
    };

    const handleDepartmentChange = (e) => {
        const val = e.target.value || '';
        setSelectedDepartment(val);
        upsertFilter('Department', '=', val === '' ? null : val);

        setPagedQuery(prev => ({ ...prev, page: 1 }));
    };

    const handlePriorityChange = (e) => {
        const val = e.target.value || '';
        setPriority(val);
        upsertFilter('Priority', '=', val === '' ? null : val);

        setPagedQuery(prev => ({ ...prev, page: 1 }));
    };

    const handleStateChange = (e) => {
        const val = e.target.value || '';
        setState(val);
        upsertFilter('State', '=', val === '' ? null : val);

        setPagedQuery(prev => ({ ...prev, page: 1 }))
    };

    const handleReadyForCloseChange = (e) => {
        const val = e.target.value;
        setReadyForClose(val);

        if (val === '') {
            upsertFilter('ReadyForClose', '=', null);
        } else if (val === 'true') {
            upsertFilter('ReadyForClose', '=', '1');
        } else if (val === 'false') {
            upsertFilter('ReadyForClose', '=', '0');
        }

        setPagedQuery(prev => ({ ...prev, page: 1 }))
    };

    const setCurrentMonthRange = () => {
        const now = new Date();
        const currentYear = now.getFullYear();
        const currentMonth = now.getMonth();

        const from = new Date(currentYear, currentMonth, 1, 0, 0, 0, 0);
        const to = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59, 999);

        setDateFromFilter(from);
        setDateToFilter(to);

        upsertFilter('DateAddEnrollment', '>=', from.toISOString());
        upsertFilter('DateAddEnrollment', '<=', to.toISOString());
    };

    const setPreviousMonthRange = () => {
        const now = new Date();
        const currentYear = now.getFullYear();
        const currentMonth = now.getMonth();

        const prevMonth = currentMonth === 0 ? 11 : currentMonth - 1;
        const yearOfPrevMonth = currentMonth === 0 ? currentYear - 1 : currentYear;

        const from = new Date(yearOfPrevMonth, prevMonth, 1, 0, 0, 0, 0);
        const to = new Date(yearOfPrevMonth, prevMonth + 1, 0, 23, 59, 59, 999);

        setDateFromFilter(from);
        setDateToFilter(to);

        upsertFilter('DateAddEnrollment', '>=', from.toISOString());
        upsertFilter('DateAddEnrollment', '<=', to.toISOString());
    };

    const setCurrentQuarterRange = () => {
        const now = new Date();
        const currentYear = now.getFullYear();
        const currentMonth = now.getMonth();
        const currentQuarter = Math.floor(currentMonth / 3);
        const quarterStartMonth = currentQuarter * 3;

        const from = new Date(currentYear, quarterStartMonth, 1, 0, 0, 0, 0);
        const to = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59, 999);

        setDateFromFilter(from);
        setDateToFilter(to);

        upsertFilter('DateAddEnrollment', '>=', from.toISOString());
        upsertFilter('DateAddEnrollment', '<=', to.toISOString());
    };

    const setPreviousQuarterRange = () => {
        const now = new Date();
        const currentYear = now.getFullYear();
        const currentMonth = now.getMonth();
        const currentQuarter = Math.floor(currentMonth / 3);

        const prevQuarter = currentQuarter === 0 ? 3 : currentQuarter - 1;
        const yearOfPrevQuarter = currentQuarter === 0 ? currentYear - 1 : currentYear;

        const quarterStartMonth = prevQuarter * 3;
        const quarterEndMonth = quarterStartMonth + 2;

        const from = new Date(yearOfPrevQuarter, quarterStartMonth, 1, 0, 0, 0, 0);
        const to = new Date(yearOfPrevQuarter, quarterEndMonth + 1, 0, 23, 59, 59, 999);

        setDateFromFilter(from);
        setDateToFilter(to);

        upsertFilter('DateAddEnrollment', '>=', from.toISOString());
        upsertFilter('DateAddEnrollment', '<=', to.toISOString());
    };

    const setCurrentYearRange = () => {
        const now = new Date();
        const currentYear = now.getFullYear();

        const from = new Date(currentYear, 0, 1, 0, 0, 0, 0);
        const to = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 23, 59, 59, 999);

        setDateFromFilter(from);
        setDateToFilter(to);

        upsertFilter('DateAddEnrollment', '>=', from.toISOString());
        upsertFilter('DateAddEnrollment', '<=', to.toISOString());
    };

    const setPreviousYearRange = () => {
        const now = new Date();
        const lastYear = now.getFullYear() - 1;

        const from = new Date(lastYear, 0, 1, 0, 0, 0, 0);
        const to = new Date(lastYear, 11, 31, 23, 59, 59, 999);

        setDateFromFilter(from);
        setDateToFilter(to);

        upsertFilter('DateAddEnrollment', '>=', from.toISOString());
        upsertFilter('DateAddEnrollment', '<=', to.toISOString());
    };

    const handleResetFilters = () => {
        setDateFromFilter(null);
        setDateToFilter(null);
        setSearchDescription('');
        setSelectedUserAddEnrollment('');
        setSelectedDepartment('');
        setPriority('');
        setState('');
        setReadyForClose('');

        setPagedQuery(prev => ({ ...prev, page: 1, filter: [] }));
    };

    useEffect(() => {
        const id = setTimeout(() => {
            const t = (searchDescription || '').trim();
            if (t.length === 0) {
                upsertFilter('Description', 'LIKE', null);
            } else {
                const escaped = t.replace(/'/g, "''");
                upsertFilter('Description', 'LIKE', `%${escaped}%`);
            }
        }, 1000);

        return () => clearTimeout(id);
    }, [searchDescription]);

    useEffect(() => {
        let aborted = false;

        const loadUsersAddEnrollments = async () => {
            try {
                const usersAddEnrollmentsResponse = await enrollmentServices.getUsersAddEnrollments();
                if (!usersAddEnrollmentsResponse.ok) throw new Error('Failed to fetch usersAddEnrollments');
                const data = await usersAddEnrollmentsResponse.json();
                const mapped = Array.isArray(data)
                    ? data.map(x => ({
                        userAddEnrollment: x.userAddEnrollment ?? x.UserAddEnrollment,
                        userAddEnrollmentFullName: x.userAddEnrollmentFullName ?? x.UserAddEnrollmentFullName
                    }))
                    : [];
                if (!aborted) setUsersAddEnrollments(mapped);
            } catch (error) {
                console.error('Error loading usersAddEnrollments:', error);
            }
        };

        const loadDepartments = async () => {
            try {
                const departmentsResponse = await fetch('/Department.json');
                if (!departmentsResponse.ok) throw new Error('Failed to fetch departments');
                const departmentData = await departmentsResponse.json();
                const mapped = Array.isArray(departmentData)
                    ? departmentData.map(item => item.value === '' ? { ...item, name: 'Wszyscy' } : item)
                    : [];
                if (!aborted) setDepartments(mapped);
            } catch (error) {
                console.error('Error loading departments:', error);
            }
        };

        loadUsersAddEnrollments();
        loadDepartments();

        return () => { aborted = true; };
    }, []);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const enrollmentResponse = await enrollmentServices.index(pagedQuery);
                if (!enrollmentResponse.ok) {
                    throw new Error('Failed to fetch enrollments data');
                };
                const enrollmentsData = await enrollmentResponse.json();
                setEnrollments(enrollmentsData);
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        setTimeout(fetchData, 0);
    }, [pagedQuery]);

    return (
        <>
            <p>Lista zgłoszeń</p>
            <div>
                <span>Data dodania: </span>
                <DatePicker
                    tabIndex='1'
                    selected={dateFromFilter}

                    dateFormat='dd.MM.yyyy'
                    showMonthDropdown
                    showYearDropdown
                    scrollableYearDropdown
                    yearDropdownItemNumber={15}

                    selectsStart
                    startDate={dateFromFilter}
                    endDate={dateToFilter}

                    placeholderText='Data od'
                    onChange={handleDateFromFilterChange}
                    isClearable
                    locale={pl}
                />
                <DatePicker
                    tabIndex='2'
                    selected={dateToFilter}

                    dateFormat='dd.MM.yyyy'
                    showMonthDropdown
                    showYearDropdown
                    scrollableYearDropdown
                    yearDropdownItemNumber={15}

                    selectsEnd
                    startDate={dateFromFilter}
                    endDate={dateToFilter}
                    minDate={dateFromFilter}

                    placeholderText='Data do'
                    onChange={handleDateToFilterChange}
                    isClearable
                    locale={pl}
                />
            </div>
            <br/>
            <div>
                <button
                    tabIndex='3'
                    type='button'
                    onClick={setCurrentMonthRange}>
                    Bieżący miesiąc
                </button>
                &nbsp;
                <button
                    tabIndex='4'
                    type='button'
                    onClick={setPreviousMonthRange}>
                    Poprzedni miesiąc
                </button>
                &nbsp;
                <button
                    tabIndex='5'
                    type='button'
                    onClick={setCurrentQuarterRange}>
                    Bieżący kwartał
                </button>
            </div>
            <br />
            <div>
                <button
                    tabIndex='6'
                    type='button'
                    onClick={setPreviousQuarterRange}>
                    Poprzedni kwartał
                </button>
                &nbsp;
                <button
                    tabIndex='7'
                    type='button'
                    onClick={setCurrentYearRange}>
                    Bieżący rok
                </button>
                &nbsp;
                <button
                    tabIndex='8'
                    type='button'
                    onClick={setPreviousYearRange}>
                    Poprzedni rok
                </button>
            </div>
            <div>
                <br />
                <span>Opis:</span>
                &nbsp;
                <input
                    tabIndex='9'
                    type='text'
                    placeholder='szukaj w opisie...'
                    value={searchDescription}
                    onChange={(e) => setSearchDescription(e.target.value)}
                />
            </div>
            <div>
                <br />
                <label style={{ marginRight: 6 }}>Zgłaszający:</label>
                <select
                    tabIndex='10'
                    value={selectedUserAddEnrollment}
                    onChange={handleUserAddEnrollmentChange}
                    disabled={usersAddEnrollments.length === 0}
                >
                    <option value=''>-- wszyscy --</option>
                    {(usersAddEnrollments ?? usersAddEnrollments).map(u => (
                        <option key={u.userAddEnrollment} value={u.userAddEnrollment}>
                            {u.userAddEnrollmentFullName}
                        </option>
                    ))}
                </select>
            </div>
            <div>
                <br />
                <label style={{ marginRight: 6 }}>Filtruj według działu docelowego:</label>
                <select
                    tabIndex='11'
                    value={selectedDepartment}
                    onChange={handleDepartmentChange}
                >
                    <option value=''>-- Wszystkie --</option>
                    {departments.map((department, index) => (
                        <option key={index} value={department.value}>
                            {department.name}
                        </option>
                    ))}
                </select>
            </div>
            <div>
                <br />
                <label style={{ marginRight: 6 }}>Filtruj według priorytetu:</label>
                <select
                    tabIndex='12'
                    value={priority}
                    onChange={handlePriorityChange}
                >
                    <option value=''>-- Wszystkie --</option>
                    {Object.entries(mapPriority).map(([value, label]) => (
                        <option key={value} value={value}>
                            {label}
                        </option>
                    ))}
                </select>
            </div>
            <div>
                <br />
                <label style={{ marginRight: 6 }}>Filtruj według statusu:</label>
                <select
                    tabIndex='13'
                    value={state}
                    onChange={handleStateChange}
                >
                    <option value=''>-- Wszystkie --</option>
                    {Object.entries(mapState).map(([value, name], index) => (
                        <option key={index} value={value}>
                            {name}
                        </option>
                    ))}
                </select>
            </div>
            <div>
                <br />
                <label style={{ marginRight: 6 }}>Filtruj według gotowości:</label>
                <select
                    tabIndex='14'
                    value={readyForClose}
                    onChange={handleReadyForCloseChange}
                >
                    <option value=''>-- Wszystkie --</option>
                    <option value='true'>Gotowe</option>
                    <option value='false'>Niegotowe</option>
                </select>
            </div>
            <div>
                <br />
                <button
                    tabIndex='15'
                    type="button"
                    onClick={handleResetFilters}
                    style={{ marginLeft: 12 }}
                >
                    Reset filtrów
                </button>
            </div>
            <table>
                <thead>
                    <tr>
                        <th style={{ width: '05%', cursor: 'pointer' }} onClick={() => handleSort('NrDisplay')}>
                            Nr{sortIndicator('NrDisplay')}
                        </th>

                        <th style={{ width: '05%', cursor: 'pointer' }} onClick={() => handleSort('DateAddEnrollment')}>
                            Data wpr.{sortIndicator('DateAddEnrollment')}
                        </th>

                        <th style={{ width: '05%', cursor: 'pointer' }} onClick={() => handleSort('DateEndDeclareByDepartment')}>
                            Data zak.{sortIndicator('DateEndDeclareByDepartment')}
                        </th>

                        <th style={{ width: '25%', cursor: 'pointer' }} onClick={() => handleSort('Description')}>
                            Opis{sortIndicator('Description')}
                        </th>

                        <th style={{ width: '10%', cursor: 'pointer' }} onClick={() => handleSort('UserAddEnrollmentFullName')}>
                            Zgłaszający{sortIndicator('UserAddEnrollmentFullName')}
                        </th>

                        <th style={{ width: '10%', cursor: 'pointer' }} onClick={() => handleSort('Department')}>
                            Dział docelowy{sortIndicator('Department')}
                        </th>

                        <th style={{ width: '06%', cursor: 'pointer' }} onClick={() => handleSort('Priority')}>
                            Priorytet{sortIndicator('Priority')}
                        </th>

                        <th style={{ width: '05%', cursor: 'pointer' }} onClick={() => handleSort('State')}>
                            Status{sortIndicator('State')}
                        </th>

                        <th style={{ width: '06%', cursor: 'pointer' }} onClick={() => handleSort('ReadyForClose')}>
                            Gotowe{sortIndicator('ReadyForClose')}
                        </th>

                        <th style={{ width: '23%' }}>Operacje</th>
                    </tr>
                </thead>
                <tbody>
                    {enrollments.results && enrollments.results.map((enrollment, index) => {
                        const dateaddenrollment = enrollment.dateAddEnrollment ? format(new Date(enrollment.dateAddEnrollment), 'dd.MM.yyyy') : '';
                        const dateendenrollment = enrollment.dateEndDeclareByDepartment ? format(new Date(enrollment.dateEndDeclareByDepartment), 'dd.MM.yyyy') : '';
                        return (
                            <tr key={index} style={{
                                backgroundColor: enrollment.priority === 2 ? 'lavenderblush' : 'inherit'
                            }}>
                                <td>{enrollment.nr}/{enrollment.year}</td>
                                <td>{dateaddenrollment}</td>
                                <td>{dateendenrollment}</td>
                                <td>
                                    {enrollment.description.length > 60
                                        ? `${enrollment.description.substring(0, 60)}...`
                                        : enrollment.description}
                                </td>
                                <td>{enrollment.userAddEnrollmentFullName}</td>
                                <td>{enrollment.department}</td>
                                <td>{mapPriority[enrollment.priority] || enrollment.priority}</td>
                                <td>{mapState[enrollment.state] || enrollment.state}</td>

                                <td
                                    style={{
                                        position: 'relative',
                                        background:
                                            enrollment.actionRequest !== 0
                                                ? 'repeating-linear-gradient(-45deg, rgba(0, 0, 0, 0.3), rgba(0, 0, 0, 0.3) 1px, transparent 2px, transparent 2px, rgba(0, 0, 0, 0.3) 3px)'
                                                : 'inherit'
                                    }}
                                >
                                    <input
                                        type='checkbox'
                                        checked={enrollment.readyForClose}
                                        readOnly
                                    />
                                </td>
                                <td>
                                    <span>
                                        <Link to={`../${enrollment.id}`}>
                                            <img src={iconDetail} alt='Szczegóły' title='Szczegóły' />
                                        </Link>
                                    </span>
                                </td>
                            </tr>
                        );
                    }
                    )}
                    {
                        enrollments.results === null &&
                        <tr>
                            <td colSpan='10'>
                                <div>Brak danych...</div>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
            <br />
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                <div>Ilość wyników na stronie : &nbsp;
                    <button
                        style={enrollments.resultsPerPage === 10 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(10) }}
                    >
                        10
                    </button>
                    <button
                        style={enrollments.resultsPerPage === 20 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(20) }}
                    >
                        20
                    </button>
                    <button
                        style={enrollments.resultsPerPage === 50 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(50) }}
                    >
                        50
                    </button>
                </div>
                <div>Strona {enrollments.page} z {enrollments.totalPages} &nbsp;
                    <button
                        onClick={() => { handleFirstPage() }}
                    >
                        &#60;&#60;
                    </button>
                    <button
                        onClick={() => { handlePrevPage() }}
                    >
                        &#60;
                    </button>
                    <button
                        onClick={() => { handleNextPage() }}
                    >
                        &#62;
                    </button>
                    <button
                        onClick={() => { handleLastPage() }}
                    >
                        &#62;&#62;
                    </button>
                </div>
            </div>
        </>
    );
};

export default EnrollmentsReport;