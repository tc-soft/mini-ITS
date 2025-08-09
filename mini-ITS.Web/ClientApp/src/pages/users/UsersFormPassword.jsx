import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';

const UsersFormPassword = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const returnTo = location.state?.from || '/';

    const { handleSubmit, register, formState: { isSubmitting } } = useForm();

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
                                {...register('oldPassword',
                                    {
                                        required: 'Hasło jest wymagane.'
                                    }
                                )
                                }
                            />
                        </div>

                        <label>Nowe hasło:</label>
                        <div>
                            <input
                                tabIndex='2'
                                type='password'
                                {...register('passwordHash',
                                    {
                                        required: 'Hasło jest wymagane.'
                                    }
                                )
                                }
                            />
                        </div>

                        <label>Powtórz hasło:</label>
                        <div>
                            <input
                                tabIndex='3'
                                type='password'
                                {...register('confirmPasswordHash',
                                    {
                                        required: 'Hasło jest wymagane.'
                                    }
                                )
                                }
                            />
                        </div>
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