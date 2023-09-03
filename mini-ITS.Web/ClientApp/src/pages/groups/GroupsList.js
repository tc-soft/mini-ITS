import React, { useState, useEffect } from 'react';
import { groupsServices } from '../../services/GroupsServices';

const GroupsList = (props) => {
    const {
        pagedQuery,
        setPagedQuery
    } = props;

    const [groups, setGroups] = useState({
        results: null,
        page: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });

    const handleSetResultsPerPage = (number) => {
        setPagedQuery(prevState => ({
            ...prevState,
            resultsPerPage: number,
            page: 1
        }));
    };

    const handleFirstPage = () => {
        if (groups.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: 1
            }));
        };
    };

    const handlePrevPage = () => {
        if (groups.page > 1) {
            setPagedQuery((prevState) => ({
                ...prevState,
                page: groups.page - 1
            }));
        };
    };

    const handleNextPage = () => {
        if (groups.page < groups.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: groups.page + 1
            }));
        };
    };

    const handleLastPage = () => {
        if (groups.page < groups.totalPages) {
            setPagedQuery(prevState => ({
                ...prevState,
                page: groups.totalPages
            }));
        };
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await groupsServices.index(pagedQuery);
                if (response.ok) {
                    const data = await response.json();
                    setGroups(data);
                }
                else {
                    throw new Error('Network response was not ok');
                };
            }
            catch (error) {
                console.error('Error loading data:', error);
            };
        };

        setTimeout(fetchData, 0);
    }, [pagedQuery]);

    return (
        <>
            <p>Lista grup</p>

            <table>
                <thead>
                    <tr>
                        <th style={{ width: '05%' }}>Lp.</th>
                        <th style={{ width: '80%' }}>Nazwa grupy</th>
                        <th style={{ width: '15%' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {groups.results && groups.results.map((group, index) => {
                        const record = index + ((groups.page - 1) * groups.resultsPerPage) + 1;
                        return (
                            <tr key={index}>
                                <td>{record}</td>
                                <td>{group.groupName}</td>
                                <td></td>
                            </tr>
                        );
                    }
                    )}
                    {
                        groups.results === null &&
                        <tr>
                            <td colSpan='3'>
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
                        style={groups.resultsPerPage === 10 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(10) }}
                    >
                        10
                    </button>
                    <button
                        style={groups.resultsPerPage === 20 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(20) }}
                    >
                        20
                    </button>
                    <button
                        style={groups.resultsPerPage === 50 ? { backgroundColor: 'lightblue' } : {}}
                        onClick={() => { handleSetResultsPerPage(50) }}
                    >
                        50
                    </button>
                </div>
                <div>Strona {groups.page} z {groups.totalPages} &nbsp;
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

export default GroupsList;