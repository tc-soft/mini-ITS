import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { useForm } from 'react-hook-form';
import { usersServices } from '../../services/UsersServices';
import iconEdit from '../../images/iconEdit.svg';
import iconUser from '../../images/iconUser.svg';
import iconSave from '../../images/iconSave.svg';
import iconCancel from '../../images/iconCancel.svg';

import '../../styles/pages/Users.scss';

const UsersFormPassword = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const returnTo = location.state?.from || '/';
    const { currentUser } = useAuth();

    const { handleSubmit, register, getValues, formState: { errors, isSubmitting } } = useForm();

    const handleErrorResponse = (response, errorMessage) => {
        if (!response.ok) throw errorMessage;
    };

    const onSubmit = async (values) => {
        try {
            await new Promise(resolve => setTimeout(resolve, 100));

            handleErrorResponse(
                await usersServices.changePassword({ login: currentUser.login, oldPassword: values.oldPassword, newPassword: values.passwordHash }),
                'Zmiana hasła nie powiodła się!');

            navigate(returnTo);
        }
        catch (error) {
            console.error(error);
        };
    };

    return (
        <div className='usersFormPassword'>
            <div className='usersFormPassword-title'>
                <img src={iconEdit} height='17px' alt='iconEdit' />
                <p>Zmiana hasła</p>
            </div>

            <div className='usersFormPassword-userInfo'>
                <img src={iconUser} alt='iconUser' />
                <p>Użytkownik:<span>{currentUser.login}</span></p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} autoComplete='off'>
                <div className='usersFormPassword-detail'>
                    <div className='usersFormPassword-detail-section'>
                        <label className='usersFormPassword-detail-section__label'>Stare hasło:</label>
                        <input className='usersFormPassword-detail-section__input'
                            tabIndex='1'
                            type='password'
                            error={errors.oldPassword}
                            {...register('oldPassword', { required: 'Hasło jest wymagane.' })}
                        />
                        {errors.oldPassword ?
                            <p className='usersFormPassword-detail-section__errorMessage'>{errors.oldPassword?.message}</p>
                            :
                            <p className='usersFormPassword-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersFormPassword-detail-section__label'>Nowe hasło:</label>
                        <input className='usersFormPassword-detail-section__input'
                            tabIndex='2'
                            type='password'
                            error={errors.passwordHash}
                            {...register('passwordHash', {
                                required: 'Hasło jest wymagane.',
                                minLength: { value: 8, message: 'Hasło musi zawierać min. 8 znaków.' },
                                pattern: { value: /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).{1,}$/g, message: 'Hasło nie spełnia wymogów.' },
                                validate: value => {
                                    if (getValues('oldPassword') !== '') {
                                        return value !== getValues('oldPassword') || 'Hasło musi być inne niż poprzednio.';
                                    }
                                    return true
                                }
                            })}
                        />
                        {errors.passwordHash ?
                            <p className='usersFormPassword-detail-section__errorMessage'>{errors.passwordHash?.message}</p>
                            :
                            <p className='usersFormPassword-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersFormPassword-detail-section__label'>Powtórz hasło:</label>
                        <input className='usersFormPassword-detail-section__input'
                            tabIndex='3'
                            type='password'
                            error={errors.confirmPasswordHash}
                            {...register('confirmPasswordHash', {
                                required: 'Hasło jest wymagane.',
                                validate: value => value === getValues('passwordHash') || 'Hasła muszą być zgodne.'
                            })}
                        />
                        {errors.confirmPasswordHash ?
                            <p className='usersFormPassword-detail-section__errorMessage'>{errors.confirmPasswordHash?.message}</p>
                            :
                            <p className='usersFormPassword-detail-section__errorMessage'>&nbsp;</p>
                        }

                    </div>
                </div>
                <div className='usersFormPassword-submit'>
                    <>
                        <button
                            tabIndex='4'
                            type='submit'
                            disabled={isSubmitting}
                            className='usersFormPassword-submit__button usersFormPassword-submit__button--saveButton'>
                            <img src={iconSave} alt='iconSave' />
                            Zapisz
                        </button>
                    </>
                    <Link tabIndex='-1' to={returnTo}>
                        <button
                            tabIndex='5'
                            className='usersFormPassword-submit__button usersFormPassword-submit__button--cancelButton'>
                            <img src={iconCancel} alt='iconCancel' />
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </div>
    );
};

export default UsersFormPassword;