import React, { useState } from 'react';
import { useAuth } from '../../components/AuthProvider';
import { useForm } from "react-hook-form";
import { usersServices } from '../../services/UsersServices';

function LoginForm() {
    const { handleLogin, navigate } = useAuth();
    const { handleSubmit, register, reset, formState: { errors } } = useForm();

    function onSubmit(values) {
        usersServices.login(values.login, values.password)
            .then((response) => {
                if (response.ok) {
                    return response.json()
                        .then((data) => {
                            handleLogin(data);
                            reset();
                            navigate('/');
                        })
                } else {
                    return response.json()
                        .then((data) => {
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
            <form onSubmit={handleSubmit(onSubmit)}>
                <p>Logowanie</p>

                <label>Nazwa użytkownika</label>
                <input
                    size='35'
                    type='text'
                    placeholder='Wpisz login'
                    error={errors.login}
                    {...register('login', {
                        required: 'Nazwa użytkownika jest wymagana',
                        maxLength: { value: 20, message: 'Nazwa użytkownika za długa' }
                    })}
                />

                <label>Hasło</label>
                <input
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

                <div>
                    <button
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