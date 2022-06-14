import React, { useState } from 'react';
import { useAuth } from '../../components/AuthProvider';
import { useForm } from "react-hook-form";
import { usersServices } from '../../services/UsersServices';
import ErrorMessage from './ErrorMessage';

import '../../styles/pages/Login.scss';

function LoginForm() {
    const { handleLogin, navigate } = useAuth();
    const [loginError, setLoginError] = useState("");
    const { handleSubmit, register, reset, formState: { errors } } = useForm();

    function onSubmit(values) {
        usersServices.login(values.login, values.password)
            .then((response) => {
                if (response.ok) {
                    return response.json()
                        .then((data) => {
                            handleLogin(data);
                            setLoginError(null);
                            reset();
                            navigate('/');
                        })
                } else {
                    return response.json()
                        .then((data) => {
                            setLoginError(data);
                            reset();
                        })
                }
            })
            .catch((error) => {
                setTimeout(() => {
                    console.error('Error:', error);
                    alert(error);
                }, 200);
            });
    };

    return (
        <>
            <form onSubmit={handleSubmit(onSubmit)} className='login'>
                <p className='login__title'>Logowanie</p>

                <ErrorMessage errors={loginError} />

                <label className='login__labels'>Nazwa użytkownika</label>
                <input
                    className='login__inputs'
                    size='35'
                    type='text'
                    placeholder='Wpisz login'
                    error={errors.login}
                    {...register('login', {
                        required: 'Nazwa użytkownika jest wymagana',
                        maxLength: { value: 20, message: 'Nazwa użytkownika za długa' }
                    })}
                />
                <ErrorMessage errors={errors.login?.message} />

                <label className='login__labels'>Hasło</label>
                <input
                    className='login__inputs'
                    size='35'
                    type='password'
                    placeholder='Wpiz hasło'
                    autoComplete='on'
                    error={errors.password}
                    {...register('password', {
                        required: 'Hasło jest wymagane',
                        maxLength: { value: 40, message: 'Hasło za długie' }
                    })}
                />
                <ErrorMessage errors={errors.password?.message} />

                <div>
                    <button
                        className='login__button'
                        type='submit'
                        disabled={false}
                    >
                        Zaloguj się
                    </button>
                </div>

            </form>
        </>
    );
}

export default LoginForm;