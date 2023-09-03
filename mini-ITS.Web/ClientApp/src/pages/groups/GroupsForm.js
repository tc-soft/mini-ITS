import React, { useCallback, useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { groupsServices } from '../../services/GroupsServices';

const GroupsForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const { groupId } = useParams();

    const { register, reset, getValues } = useForm();
    const title = { Create: 'Dodaj grupę', Detail: 'Szczegóły grupy', Edit: 'Edycja' };

    const resetAsyncForm = useCallback(async () => {
        try {
            const response = await groupsServices.edit(groupId);
            if (response.ok) {
                const data = await response.json();
                reset(data);
            }
            else {
                const errorData = await response.json();
                console.log(errorData);
            };
        }
        catch (error) {
            console.error(error);
        };
    }, [reset]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                resetAsyncForm();
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        fetchData();
    }, [resetAsyncForm]);

    return (
        <>
            <div>
                <h3>{title[isMode]}</h3><br />
            </div>

            <div>
                <p>Grupa:<span>{getValues('groupName')}</span></p><br />
            </div>

            <form>
                <div>
                    <div>
                        <label>Nazwa grupy</label><br />
                        <input
                            type='text'
                            placeholder='Wpisz nazwę grupy'
                            disabled={isReadMode}
                            {...register('groupName')}
                        />
                        <br /><br />
                    </div>
                </div>
                <div>
                    <Link tabIndex='-1' to={'..'}>
                        <button>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default GroupsForm;