import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { groupsServices } from '../../services/GroupsServices';
import { ReactComponent as IconAdd } from '../../images/iconAdd.svg';
import { ReactComponent as IconDetail } from '../../images/iconDetail.svg';
import { ReactComponent as IconEdit } from '../../images/iconEdit.svg';
import { ReactComponent as IconDelete } from '../../images/iconDelete.svg';
import { ReactComponent as IconFirstPage } from '../../images/iconFirstPage.svg';
import { ReactComponent as IconPrevPage } from '../../images/iconPrevPage.svg';
import { ReactComponent as IconNextPage } from '../../images/iconNextPage.svg';
import { ReactComponent as IconLastPage } from '../../images/iconLastPage.svg';

import '../../styles/pages/Groups.scss';

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
        <div className='groupsList'>
            <div className='groupsList-panel'>
                <div className='groupsList-panel-tittle'>
                    <p>Lista grup</p>
                    <Link>
                        <button title='Dodaj nową'>
                            <IconAdd />
                            <span>Dodaj</span>
                        </button>
                    </Link>
                </div>
            </div>
            <table className='groupsList-table'>
                <thead>
                    <tr>
                        <th style={{ width: '05%' }}>Lp.</th>
                        <th style={{ width: '80%' }}>Nazwa grupy</th>
                        <th style={{ width: '15%' }}>Operacje</th>
                    </tr>
                </thead>
                <tbody>
                    {groups.results && groups.results.map((group, index) => {
                        const record = index + ((groups.page - 1) * groups.resultsPerPage) + 1;
                        return (
                            <tr key={index}>
                                <td>{record}</td>
                                <td style={{ textAlign: 'left' }}>{group.groupName}</td>
                                <td>
                                    <span>
                                        <Link><IconDetail title='Szczegóły' /></Link>
                                    </span>
                                    <span>
                                        <Link><IconEdit title='Edycja' /></Link>
                                    </span>
                                    <span title='Usuń'>
                                        <IconDelete title='Usuń' />
                                    </span>
                                </td>
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
            <div className='groupsList-paginationPanel'>
                <div>Ilość wyników na stronie : &nbsp;
                    <button
                        className={groups.resultsPerPage === 10 ? 'groupsList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(10) }}
                    >
                        10
                    </button>
                    <button
                        className={groups.resultsPerPage === 20 ? 'groupsList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(20) }}
                    >
                        20
                    </button>
                    <button
                        className={groups.resultsPerPage === 50 ? 'groupsList-paginationPanel--buttonActive' : ''}
                        onClick={() => { handleSetResultsPerPage(50) }}
                    >
                        50
                    </button>
                </div>
                <div>Strona {groups.page} z {groups.totalPages} &nbsp;
                    <button
                        onClick={() => { handleFirstPage() }}
                    >
                        <IconFirstPage title='Początek' />
                    </button>

                    <button
                        onClick={() => { handlePrevPage() }}
                    >
                        <IconPrevPage title='Wstecz' />
                    </button>

                    <button
                        onClick={() => { handleNextPage() }}
                    >
                        <IconNextPage title='Następna' />
                    </button>

                    <button
                        onClick={() => { handleLastPage() }}
                    >
                        <IconLastPage title='Koniec' />
                    </button>
                </div>
            </div>
        </div>
    );
};

export default GroupsList;