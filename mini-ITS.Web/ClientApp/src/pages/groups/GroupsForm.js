import React, { useCallback, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { groupsServices } from '../../services/GroupsServices';

const GroupsForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const navigate = useNavigate();
    const { groupId } = useParams();

    const { handleSubmit, register, reset, getValues, setFocus, formState: { errors } } = useForm();
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

    const handleErrorResponse = (response, errorMessage) => {
        if (!response.ok) throw errorMessage;
    };

    const onSubmit = async (values) => {
        try {
            if (isMode === 'Edit') {
                handleErrorResponse(
                    await groupsServices.update(values.id, values),
                    'Aktualizacja nie powiodła się!');
            }

            navigate('/Groups');
        }
        catch (error) {
            console.error(error);
        };
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                if (isMode === 'Detail' || isMode === 'Edit') {
                    resetAsyncForm();
                };
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        fetchData();
    }, [resetAsyncForm]);

    useEffect(() => {
        setFocus('groupName');
    }, [setFocus]);

    return (
        <>
            <div>
                <h3>{title[isMode]}</h3><br />
            </div>

            <div>
                <p>Grupa:<span>{getValues('groupName')}</span></p><br />
            </div>

            <form onSubmit={handleSubmit(onSubmit)}>
                <div>
                    <div>
                        <label>Nazwa grupy</label><br />
                        <input
                            tabIndex='1'
                            type='text'
                            placeholder='Wpisz nazwę grupy'
                            error={errors.login}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('groupName', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^[^\s].+[^\s]$/g, message: 'Niedozwolony znak.' },
                                maxLength: { value: 60, message: 'Za duża ilośc znaków.' }
                            })}
                        />
                        {errors.groupName ? <p style={{ color: 'red' }} >{errors.groupName?.message}</p> : <p>&nbsp;</p>}<br />

                    </div>
                </div>
                <div>
                    {(isMode === 'Edit') && (
                        <>
                            <button
                                tabIndex='2'
                                type='submit'>
                                Zapisz
                            </button>
                        </>
                    )}
                    &nbsp;
                    <Link tabIndex='-1' to={'..'}>
                        <button tabIndex='3'>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default GroupsForm;