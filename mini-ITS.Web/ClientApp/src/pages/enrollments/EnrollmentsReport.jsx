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
            <br />
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