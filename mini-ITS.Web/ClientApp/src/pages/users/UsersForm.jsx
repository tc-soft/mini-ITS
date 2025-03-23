import { useCallback, useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { useForm } from 'react-hook-form';
import { usersServices } from '../../services/UsersServices';
import iconEdit from '../../images/iconEdit.svg';
import iconUser from '../../images/iconUser.svg';
import iconShowPassword from '../../images/iconShowPassword.svg';
import iconHidePassword from '../../images/iconHidePassword.svg';
import iconSave from '../../images/iconSave.svg';
import iconCancel from '../../images/iconCancel.svg';

import '../../styles/pages/Users.scss';

const UsersForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const navigate = useNavigate();
    const { userId } = useParams();
    const { currentUser } = useAuth();

    const [mapDepartment, setMapDepartment] = useState([]);
    const [mapRole, setMapRole] = useState([]);
    const { handleSubmit, register, reset, getValues, setValue, setFocus, clearErrors, formState: { errors, isSubmitting } } = useForm();
    const title = { Create: 'Dodaj użytkownika', Detail: 'Szczegóły użytkownika', Edit: 'Edycja' };
    const [activePassword, setActivePassword] = useState(false);
    const [showPassword, setShowPassword] = useState([false, false, false]);
    const [activeFocus, setActiveFocus] = useState([false, false, false]);
    const [changePasswordError, setChangePasswordError] = useState();

    const resetAsyncForm = useCallback(async () => {
        try {
            const response = await usersServices.edit(userId);
            if (response.ok) {
                const data = await response.json();
                data.passwordHash = '';
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
                if (activePassword) {
                    if ((userId !== currentUser.id) && (currentUser.role === 'Administrator')) {
                        handleErrorResponse(
                            await usersServices.setPassword({ id: values.id, newPassword: values.passwordHash }),
                            'Zmiana hasła nie powiodła się!');
                    }
                    else if (userId === currentUser.id) {
                        handleErrorResponse(
                            await usersServices.changePassword({ login: values.login, oldPassword: values.oldPassword, newPassword: values.passwordHash }),
                            'Zmiana hasła nie powiodła się!');
                    }
                    else {
                        throw 'Zmiana hasła nie powiodła się!';
                    };
                };

                handleErrorResponse(
                    await usersServices.update(values.id, values),
                    'Zmiana hasła nie powiodła się!');
            }
            else if (isMode === 'Create') {
                handleErrorResponse(
                    await usersServices.create(values),
                    'Zmiana hasła nie powiodła się!');
            };

            navigate('/Users');
        }
        catch (error) {
            setChangePasswordError(error);
            console.error(error);
        };
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                const departmentResponse = await fetch('/Department.json');
                const departmentData = await departmentResponse.json();
                setMapDepartment(departmentData);

                const roleResponse = await fetch('/Role.json');
                const roleData = await roleResponse.json();
                setMapRole(roleData);

                if (isMode === 'Detail' || isMode === 'Edit') {
                    resetAsyncForm();
                };

                if (isMode === 'Create') {
                    setActivePassword(true);
                };
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        fetchData();
    }, [resetAsyncForm]);

    useEffect(() => {
        setFocus('login');
    }, [setFocus]);

    return (
        <div className='usersForm'>
            <div className='usersForm-title'>
                <img src={iconEdit} height='17px'  alt='iconEdit' />
                <p>{title[isMode]}</p>
            </div>

            <div className='usersForm-userInfo'>
                <img src={iconUser} alt='iconUser' />
                <p>Użytkownik:<span>{getValues('login')}</span></p>
            </div>

            {isSubmitting && <div className="overlay">Zapisywanie...</div>}

            <form onSubmit={handleSubmit(onSubmit)}>
                <div className='usersForm-detail'>
                    <div className='usersForm-detail-section'>
                        <label className='usersForm-detail-section__label'>Login</label>
                        <input className='usersForm-detail-section__input'
                            tabIndex='1'
                            type='text'
                            placeholder='Wpisz login'
                            error={errors.login}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('login', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^(?![@\-_.])[a-zA-Z0-9@\-_.]+(?:\s+[a-zA-Z0-9@\-_.]+)*$/g, message: 'Niedozwolony znak.' },
                                maxLength: { value: 60, message: 'Za duża ilośc znaków.' }
                            })}
                        />
                        {errors.login ?
                            <p className='usersForm-detail-section__errorMessage'>{errors.login?.message}</p>
                            :
                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersForm-detail-section__label'>Imię</label>
                        <input className='usersForm-detail-section__input'
                            tabIndex='2'
                            type='text'
                            placeholder='Wpisz imię'
                            error={errors.firstName}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('firstName', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+(?:\s+[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+)*$/g, message: 'Niedozwolony znak.' },
                                maxLength: { value: 40, message: 'Za duża ilośc znaków.' }
                            })}
                        />
                        {errors.firstName ?
                            <p className='usersForm-detail-section__errorMessage'>{errors.firstName?.message}</p>
                            :
                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersForm-detail-section__label'>Nazwisko</label>
                        <input className='usersForm-detail-section__input'
                            tabIndex='3'
                            type='text'
                            placeholder='Wpisz nazwisko'
                            error={errors.lastName}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('lastName', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+(?:\s+[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+)*$/g, message: 'Niedozwolony znak.' },
                                maxLength: { value: 40, message: 'Za duża ilośc znaków.' }
                            })}
                        />
                        {errors.lastName ?
                            <p className='usersForm-detail-section__errorMessage'>{errors.lastName?.message}</p>
                            :
                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersForm-detail-section__label'>Dział</label>
                        <select className='usersForm-detail-section__select'
                            tabIndex='4'
                            placeholder='Wybierz dział'
                            error={errors.department}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('department', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+(?:\s+[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+)*$/g, message: 'Niedozwolony znak.' },
                                maxLength: { value: 40, message: 'Za duża ilośc znaków.' }
                            })}
                        >
                            {mapDepartment.map(option => (
                                <option key={option.value} value={option.value}>{option.name}</option>
                            ))}

                        </select>
                        {errors.department ?
                            <p className='usersForm-detail-section__errorMessage'>{errors.department?.message}</p>
                            :
                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersForm-detail-section__label'>Rola</label>
                        <select className='usersForm-detail-section__select'
                            tabIndex='5'
                            placeholder='Wybierz rolę'
                            error={errors.role}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('role', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+(?:\s+[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+)*$/g, message: 'niedozwolony znak' },
                                maxLength: { value: 40, message: 'za duża ilośc znaków' }
                            })}
                        >
                            {mapRole.map(
                                (x, y) => <option key={y} value={x.value}>{x.name}</option>)
                            }
                        </select>
                        {errors.role ?
                            <p className='usersForm-detail-section__errorMessage'>{errors.role?.message}</p>
                            :
                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersForm-detail-section__label'>Email</label>
                        <input className='usersForm-detail-section__input'
                            tabIndex='6'
                            type='text'
                            placeholder='Wpisz email'
                            error={errors.email}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('email', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^(?!.*@.*@)[a-zA-Z0-9._%+-]+@[a-z0-9._-]+\.[a-z]{2,}$/g, message: 'Nieprawidłowy format.' },
                                maxLength: { value: 60, message: 'Za duża ilośc znaków.' }
                            })}
                        />
                        {errors.email ?
                            <p className='usersForm-detail-section__errorMessage'>{errors.email?.message}</p>
                            :
                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                        }

                        <label className='usersForm-detail-section__label'>Telefon</label>
                        <input className='usersForm-detail-section__input'
                            tabIndex='7'
                            type='tel'
                            placeholder='Wpisz telefon'
                            error={errors.phone}
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('phone', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: { value: /^[X0-9]{9}$/g, message: 'Nieprawidłowy format.' },
                                minLength: { value: 9, message: 'Za mała ilość znaków.' },
                                maxLength: { value: 14, message: 'Za duża ilość znaków.' }
                            })}
                        />
                        {errors.phone ?
                            <p className='usersForm-detail-section__errorMessage'>{errors.phone?.message}</p>
                            :
                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                        }
                    </div>
                    <div className='usersForm-detail-section'>
                        {(isMode === 'Edit' || isMode === 'Create') &&
                            <>
                                <label className='usersForm-detail-section__label'>
                                    <input
                                        tabIndex='8'
                                        type='checkbox'
                                        checked={activePassword}
                                        onChange={() => {
                                            setActivePassword(!activePassword);
                                            setShowPassword(false);
                                            setChangePasswordError(null);
                                            setValue('oldPassword', '');
                                            clearErrors('oldPassword');
                                            setValue('passwordHash', '');
                                            clearErrors('passwordHash');
                                            setValue('confirmPasswordHash', '');
                                            clearErrors('confirmPasswordHash');
                                        }
                                        }
                                    />
                                    &nbsp;Zmiana hasła
                                </label>
                                {changePasswordError ?
                                    <p className='usersForm-detail-section__errorMessage'>{changePasswordError}</p>
                                    :
                                    <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                                }

                                {isMode === 'Edit' && (userId === currentUser.id) &&
                                    <>
                                        <label className='usersForm-detail-section__label'>Wpisz stare hasło:</label>
                                        <div className={`usersForm-detail-section__inputPassword ${activePassword && activeFocus[0] ? 'usersForm-detail-section__inputPassword--focus' : activePassword ? 'usersForm-detail-section__inputPassword--active' : ''}`}>
                                            <input
                                                tabIndex='9'
                                                type={showPassword[0] ? 'text' : 'password'}
                                                disabled={!activePassword}
                                                onFocus={() => setActiveFocus(prevState => ([true, activeFocus[1], activeFocus[2]]))}
                                                autoComplete='on'
                                                error={errors.oldPassword}
                                                {...register('oldPassword', activePassword ?
                                                    {
                                                        onBlur: () => setActiveFocus(prevState => ([false, activeFocus[1], activeFocus[2]])),
                                                        required: 'Hasło jest wymagane'
                                                    }
                                                    :
                                                    {
                                                        required: false
                                                    }
                                                )
                                                }
                                            />
                                            <button
                                                tabIndex='-1'
                                                type='button'
                                                disabled={!activePassword}
                                                onClick={() => activePassword && setShowPassword(prevState => ([!showPassword[0], prevState[1], prevState[2]]))}
                                            >
                                        {showPassword[0] ? <img src={iconShowPassword} alt='iconShowPassword' /> : <img src={iconHidePassword} alt='iconHidePassword' /> }
                                            </button>
                                        </div>
                                        {errors.oldPassword ?
                                            <p className='usersForm-detail-section__errorMessage'>{errors.oldPassword?.message}</p>
                                            :
                                            <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                                        }
                                    </>
                                }

                                <label className='usersForm-detail-section__label'>Wpisz hasło</label>
                                <div className={`usersForm-detail-section__inputPassword ${activePassword && activeFocus[1] ? 'usersForm-detail-section__inputPassword--focus' : activePassword ? 'usersForm-detail-section__inputPassword--active' : ''}`}>
                                    <input
                                        tabIndex='10'
                                        type={showPassword[1] ? 'text' : 'password'}
                                        disabled={!activePassword}
                                        onFocus={() => setActiveFocus(prevState => ([activeFocus[0], true, activeFocus[2]]))}
                                        autoComplete='on'
                                        error={errors.passwordHash}
                                        {...register('passwordHash', activePassword ?
                                            {
                                                onBlur: () => setActiveFocus(prevState => ([activeFocus[0], false, activeFocus[2]])),
                                                required: 'Hasło jest wymagane.',
                                                minLength: { value: 8, message: 'Hasło musi zawierać min. 8 znaków.' },
                                                pattern: { value: /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).{1,}$/g, message: 'Hasło nie spełnia wymogów.'},
                                                validate: value => {
                                                    if (getValues('oldPassword') !== '') {
                                                        return value !== getValues('oldPassword') || 'Hasło musi być inne niż poprzednio.';
                                                    }
                                                    return true
                                                }
                                            }
                                            :
                                            {
                                                required: false
                                            }
                                        )
                                        }
                                    />
                                    <button
                                        tabIndex='-1'
                                        type='button'
                                        disabled={!activePassword}
                                        onClick={() => activePassword && setShowPassword(prevState => ([prevState[0], !showPassword[1], prevState[2]]))}
                                    >
                                    {showPassword[1] ? <img src={iconShowPassword} alt='iconShowPassword' /> : <img src={iconHidePassword} alt='iconHidePassword' />}
                                    </button>
                                </div>
                                {errors.passwordHash ?
                                    <p className='usersForm-detail-section__errorMessage'>{errors.passwordHash?.message}</p>
                                    :
                                    <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                                }

                                <label className='usersForm-detail-section__label'>Powtórz hasło</label>
                                <div className={`usersForm-detail-section__inputPassword ${activePassword && activeFocus[2] ? 'usersForm-detail-section__inputPassword--focus' : activePassword ? 'usersForm-detail-section__inputPassword--active' : ''}`}>
                                    <input
                                        tabIndex='11'
                                        name='confirmPasswordHash'
                                        type={showPassword[2] ? 'text' : 'password'}
                                        disabled={!activePassword}
                                        onFocus={() => setActiveFocus(prevState => ([activeFocus[0], [activeFocus[1], true]]))}
                                        autoComplete='off'
                                        error={errors.confirmPasswordHash}
                                        {...register('confirmPasswordHash', activePassword ?
                                            {
                                                onBlur: () => setActiveFocus(prevState => ([activeFocus[0], [activeFocus[1], false]])),
                                                required: 'Hasło jest wymagane.',
                                                validate: value => value === getValues('passwordHash') || 'Hasła muszą być zgodne.'
                                            }
                                            :
                                            {
                                                required: false
                                            }
                                        )
                                        }
                                    />
                                    <button
                                        tabIndex='-1'
                                        type='button'
                                        disabled={!activePassword}
                                        onClick={() => activePassword && setShowPassword(prevState => ([prevState[0], prevState[1], !showPassword[2]]))}
                                    >
                                    {showPassword[2] ? <img src={iconShowPassword} alt='iconShowPassword' /> : <img src={iconHidePassword} alt='iconHidePassword' />}
                                    </button>
                                </div>
                                {errors.confirmPasswordHash ?
                                    <p className='usersForm-detail-section__errorMessage'>{errors.confirmPasswordHash?.message}</p>
                                    :
                                    <p className='usersForm-detail-section__errorMessage'>&nbsp;</p>
                                }
                            </>
                        }
                    </div>
                </div>
                <div className='usersForm-submit'>
                    {(isMode === 'Edit' || isMode === 'Create') && (
                        <>
                            <button
                                tabIndex='12'
                                type='submit'
                                disabled={isSubmitting}
                                className='usersForm-submit__button usersForm-submit__button--saveButton'>
                                <img src={iconSave} alt='iconSave' />
                                Zapisz
                            </button>
                        </>
                    )}
                    <Link tabIndex='-1' to={'..'}>
                        <button
                            tabIndex='13'
                            className='usersForm-submit__button usersForm-submit__button--cancelButton'>
                            <img src={iconCancel} alt='iconCancel' />
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </div>
    );
};

export default UsersForm;