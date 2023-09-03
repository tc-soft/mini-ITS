import React, { useState, useEffect } from 'react';
import { groupsServices } from '../../services/GroupsServices';

const GroupsList = (props) => {
    const {
        pagedQuery,
    } = props;

    const [groups, setGroups] = useState({
        results: null,
        page: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });

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
        </>
    );
};

export default GroupsList;