import React, { useCallback, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { groupsServices } from '../../services/GroupsServices';
import { ReactComponent as IconEdit } from '../../images/iconEdit.svg';
import { ReactComponent as IconGroup } from '../../images/iconGroup.svg';
import { ReactComponent as IconSave } from '../../images/iconSave.svg';
import { ReactComponent as IconCancel } from '../../images/iconCancel.svg';

import '../../styles/pages/Groups.scss';

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
            else if (isMode === 'Create') {
                handleErrorResponse(
                    await groupsServices.create(values),
                    'Zapis nie powiódł się!');
            };

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
        <div className='groupsForm'>
            <div className='groupsForm-title'>
                <IconEdit height='17px' />
                <p>{title[isMode]}</p>
            </div>

            <div className='groupsForm-groupsInfo'>
                <IconGroup />
                <p>Grupa:<span>{getValues('groupName')}</span></p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)}>
                <div className='groupsForm-detail'>
                    <div className='groupsForm-detail-section'>
                        <label className='groupsForm-detail-section__label'>Nazwa grupy</label>
                        <input className='groupsForm-detail-section__input'
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
                        {errors.groupName ?
                            <p className='groupsForm-detail-section__errorMessage'>{errors.groupName?.message}</p>
                            :
                            <p className='groupsForm-detail-section__errorMessage'>&nbsp;</p>
                        }
                    </div>
                </div>
                <div className='groupsForm-submit'>
                    {(isMode === 'Edit' || isMode === 'Create') && (
                        <>
                            <button
                                tabIndex='2'
                                className='groupsForm-submit__button groupsForm-submit__button--saveButton'
                                type='submit'>
                                <IconSave />
                                Zapisz
                            </button>
                        </>
                    )}
                    <Link tabIndex='-1' to={'..'}>
                        <button
                            tabIndex='3'
                            className='groupsForm-submit__button groupsForm-submit__button--cancelButton'>
                            <IconCancel />
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </div>
    );
};

export default GroupsForm;