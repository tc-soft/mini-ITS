import { useCallback, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { groupsServices } from '../../services/GroupsServices';
import iconEdit from '../../images/iconEdit.svg';
import iconGroup from '../../images/iconGroup.svg';
import iconSave from '../../images/iconSave.svg';
import iconCancel from '../../images/iconCancel.svg';

import '../../styles/pages/Groups.scss';

const GroupsForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const navigate = useNavigate();
    const { groupId } = useParams();

    const { handleSubmit, register, reset, getValues, setFocus, formState: { errors, isSubmitting } } = useForm();
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
            await new Promise(resolve => setTimeout(resolve, 500));

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
                <img src={iconEdit} height='17px' alt='iconEdit' />
                <p>{title[isMode]}</p>
            </div>

            <div className='groupsForm-groupsInfo'>
                <img src={iconGroup} alt='iconGroup' />
                <p>Grupa:<span>{getValues('groupName')}</span></p>
            </div>

            {isSubmitting && <div className="overlay">Zapisywanie...</div>}

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
                                type='submit'
                                disabled={isSubmitting}
                                className='groupsForm-submit__button groupsForm-submit__button--saveButton'>
                                <img src={iconSave} alt='iconSave' />
                                Zapisz
                            </button>
                        </>
                    )}
                    <Link tabIndex='-1' to={'..'}>
                        <button
                            tabIndex='3'
                            className='groupsForm-submit__button groupsForm-submit__button--cancelButton'>
                            <img src={iconCancel} alt='iconCancel' />
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </div>
    );
};

export default GroupsForm;