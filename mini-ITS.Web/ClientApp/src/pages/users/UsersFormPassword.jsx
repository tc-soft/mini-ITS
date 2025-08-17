import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';

const UsersFormPassword = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const returnTo = location.state?.from || '/';

    const { handleSubmit, register, getValues, formState: { errors, isSubmitting } } = useForm();

    const onSubmit = async (values) => {
        try {
            await new Promise(resolve => setTimeout(resolve, 100));
            navigate(returnTo);
        }
        catch (error) {
            console.error(error);
        };
    };

    return (
        <>
            <div>
                <p>Zmiana hasła</p><br />
            </div>

            <div>
                <p>Użytkownik:</p><br />
            </div>

            <form onSubmit={handleSubmit(onSubmit)} autoComplete='off'>
                <div>
                    <div>
                        <label>Stare hasło:</label>
                        <div>
                            <input
                                tabIndex='1'
                                type='password'
                                error={errors.oldPassword}
                                {...register('oldPassword',
                                    {
                                        required: 'Hasło jest wymagane.'
                                    }
                                )
                                }
                            />
                        </div>
                        {errors.oldPassword ?
                            <p style={{ color: 'red' }}>{errors.oldPassword?.message}</p>
                            :
                            <p>&nbsp;</p>
                        }

                        <label>Nowe hasło:</label>
                        <div>
                            <input
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
                        </div>
                        {errors.passwordHash ?
                            <p style={{ color: 'red' }}>{errors.passwordHash?.message}</p>
                            :
                            <p>&nbsp;</p>
                        }

                        <label>Powtórz hasło:</label>
                        <div>
                            <input
                                tabIndex='3'
                                type='password'
                                error={errors.confirmPasswordHash}
                                {...register('confirmPasswordHash', {
                                    required: 'Hasło jest wymagane.',
                                    validate: value => value === getValues('passwordHash') || 'Hasła muszą być zgodne.'
                                })}
                            />
                        </div>
                        {errors.confirmPasswordHash ?
                            <p style={{ color: 'red' }}>{errors.confirmPasswordHash?.message}</p>
                            :
                            <p>&nbsp;</p>
                        }
                    </div>
                </div>
                <br />
                <div>
                    <>
                        <button
                            tabIndex='4'
                            type='submit'
                            disabled={isSubmitting}>
                            Zapisz
                        </button>
                    </>
                    &nbsp;
                    <Link tabIndex='-1' to={returnTo}>
                        <button
                            tabIndex='5'>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default UsersFormPassword;