import React, { useCallback, useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useForm } from "react-hook-form";
import { usersServices } from '../../services/UsersServices';

const UsersForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const { userId } = useParams();
    
    const [mapDepartment, setMapDepartment] = useState([]);
    const [mapRole, setMapRole] = useState([]);
    const { register, reset, getValues } = useForm();
    const title = { Create: 'Dodaj użytkownika', Detail: 'Szczegóły użytkownika', Edit: 'Edycja' };

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

    useEffect(() => {
        const fetchData = async () => {
            try {
                const departmentResponse = await fetch('/Department.json');
                const departmentData = await departmentResponse.json();
                setMapDepartment(departmentData);

                const roleResponse = await fetch('/Role.json');
                const roleData = await roleResponse.json();
                setMapRole(roleData);

                resetAsyncForm();
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        fetchData();
    }, [resetAsyncForm]);

    return (
        <>
            <div>
                <h3>{title[isMode]}</h3><br />
            </div>

            <div>
                <p>Użytkownik:<span>{getValues('login')}</span></p><br />
            </div>

            <form>
                <div>
                    <div>
                        <label>Login</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz login"
                            disabled={isReadMode}
                            {...register('login')}
                        />
                        <br />

                        <label>Imię</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz imię"
                            disabled={isReadMode}
                            {...register('firstName')}
                        />
                        <br />

                        <label>Nazwisko</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz nazwisko"
                            disabled={isReadMode}
                            {...register('lastName')}
                        />
                        <br />

                        <label>Dział</label><br />
                        <select
                            placeholder="Wybierz dział"
                            disabled={isReadMode}
                            {...register('department')}
                        >
                            {mapDepartment.map(option => (
                                <option key={option.value} value={option.value}>{option.name}</option>
                            ))}
                        </select>
                        <br />

                        <label>Rola</label><br />
                        <select
                            placeholder="Wybierz rolę"
                            disabled={isReadMode}
                            {...register('role')}
                        >
                            {mapRole.map(
                                (x, y) => <option key={y} value={x.value}>{x.name}</option>)
                            }
                        </select>
                        <br />

                        <label>Email</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz email"
                            disabled={isReadMode}
                            {...register('email')}
                        />
                        <br />

                        <label>Telefon</label><br />
                        <input
                            type="tel"
                            placeholder="Wpisz telefon"
                            disabled={isReadMode}
                            {...register('phone')}
                        />
                        <br /><br />
                    </div>
                </div>
                <div>
                    <Link tabIndex="-1" to={'..'}>
                        <button>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default UsersForm;