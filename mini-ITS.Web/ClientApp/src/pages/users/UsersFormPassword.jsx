import React, { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { useForm } from 'react-hook-form';
import { usersServices } from '../../services/UsersServices';
import iconEdit from '../../images/iconEdit.svg';
import iconUser from '../../images/iconUser.svg';
import iconSave from '../../images/iconSave.svg';
import iconCancel from '../../images/iconCancel.svg';
import iconShowPassword from '../../images/iconShowPassword.svg';
import iconHidePassword from '../../images/iconHidePassword.svg';

import '../../styles/pages/Users.scss';

const UsersFormPassword = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const returnTo = location.state?.from || '/';
    const { currentUser } = useAuth();

    const { handleSubmit, register, getValues, formState: { errors, isSubmitting } } = useForm();
    const [showPassword, setShowPassword] = useState([false, false, false]);

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
                        <div className={`usersFormPassword-detail-section__inputPassword usersFormPassword-detail-section__inputPassword--active`}>
                            <input
                                tabIndex='1'
                                type={showPassword[0] ? 'text' : 'password'}
                                error={errors.oldPassword}
                                {...register('oldPassword',
                                    {
                                        required: 'Hasło jest wymagane.'
                                    }
                                )
                                }
                            />
                            <button
                                tabIndex='-1'
                                type='button'
                                onClick={() => setShowPassword(prevState => ([!showPassword[0], prevState[1], prevState[2]]))}
                            >
                                {showPassword[0] ? <img src={iconShowPassword} alt='iconShowPassword' /> : <img src={iconHidePassword} alt='iconHidePassword' />}
                            </button>
                        </div>
                        {errors.oldPassword ?
                            <p className='usersFormPassword-detail-section__errorMessage'>{errors.oldPassword?.message}</p>
                            :
                            <p className='usersFormPassword-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersFormPassword-detail-section__label'>Nowe hasło:</label>
                        <div className={`usersFormPassword-detail-section__inputPassword usersFormPassword-detail-section__inputPassword--active`}>
                            <input
                                tabIndex='2'
                                type={showPassword[1] ? 'text' : 'password'}
                                error={errors.passwordHash}
                                {...register('passwordHash',
                                    {
                                        required: 'Hasło jest wymagane.',
                                        minLength: { value: 8, message: 'Hasło musi zawierać min. 8 znaków.' },
                                        pattern: { value: /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).{1,}$/g, message: 'Hasło nie spełnia wymogów.' },
                                        validate: value => {
                                            if (getValues('oldPassword') !== '') {
                                                return value !== getValues('oldPassword') || 'Hasło musi być inne niż poprzednio.';
                                            }
                                            return true
                                        }
                                    }
                                )
                                }
                            />
                            <button
                                tabIndex='-1'
                                type='button'
                                onClick={() => setShowPassword(prevState => ([prevState[0], !showPassword[1], prevState[2]]))}
                            >
                                {showPassword[1] ? <img src={iconShowPassword} alt='iconShowPassword' /> : <img src={iconHidePassword} alt='iconHidePassword' />}
                            </button>
                        </div>
                        {errors.passwordHash ?
                            <p className='usersFormPassword-detail-section__errorMessage'>{errors.passwordHash?.message}</p>
                            :
                            <p className='usersFormPassword-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersFormPassword-detail-section__label'>Powtórz hasło:</label>
                        <div className={`usersFormPassword-detail-section__inputPassword usersFormPassword-detail-section__inputPassword--active`}>
                            <input
                                tabIndex='3'
                                type={showPassword[2] ? 'text' : 'password'}
                                error={errors.confirmPasswordHash}
                                {...register('confirmPasswordHash',
                                    {
                                        required: 'Hasło jest wymagane.',
                                        validate: value => value === getValues('passwordHash') || 'Hasła muszą być zgodne.'
                                    }
                                )
                                }
                            />
                            <button
                                tabIndex='-1'
                                type='button'
                                onClick={() => setShowPassword(prevState => ([prevState[0], prevState[1], !showPassword[2]]))}
                            >
                                {showPassword[2] ? <img src={iconShowPassword} alt='iconShowPassword' /> : <img src={iconHidePassword} alt='iconHidePassword' />}
                            </button>
                        </div>
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