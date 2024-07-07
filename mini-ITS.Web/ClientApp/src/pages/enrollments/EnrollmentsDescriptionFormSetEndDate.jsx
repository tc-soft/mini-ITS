import { useState, useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import DatePicker from 'react-datepicker';

import 'react-datepicker/dist/react-datepicker.css';

const EnrollmentsDescriptionFormSetEndDate = ({ onSubmit, subForm }) => {
    const enrollment = subForm.data.enrollment;
    const [mapUsers, setMapUsers] = useState([]);
    const [isLoading, setIsLoading] = useState(true);

    const { handleSubmit, register, reset, setValue, setFocus, control, watch, clearErrors, trigger, formState: { errors } } = useForm();
    const [isDisabled, setIsDisabled] = useState(false);

    const setEndOfDay = (date) => {
        date.setHours(23);
        date.setMinutes(59);
        date.setSeconds(59);
        date.setMilliseconds(0);
        return date.toISOString();
    };

    const handleEndDateAcceptedChange = async (e) => {
        const isChecked = e.target.checked;
        setIsDisabled(isChecked);

        if (isChecked && enrollment?.dateEndDeclareByUser) {
            setValue('dateEndDeclareByDepartment', enrollment.dateEndDeclareByUser);
            setValue('userAcceptedEndDate', '');
        };

        await trigger('userAcceptedEndDate');
        await clearErrors('userAcceptedEndDate'); 
    };

    useEffect(() => {
        (async () => {
            try {
                const usersResponse = await fetch('/Users.json');
                const usersData = await usersResponse.json();
                setMapUsers(usersData);

                if (!enrollment) {
                    throw new Error('Enrollment is null');
                };

                if (enrollment.dateEndDeclareByUser) {
                    enrollment.dateEndDeclareByDepartment = enrollment.dateEndDeclareByUser;
                };

                reset(enrollment);
                
                setTimeout(() => {
                    setFocus('acceptEndDateByDepartment');    
                }, 0);
            }
            catch (error) {
                console.error('Error fetching data:', error);
            }
            finally {
                setIsLoading(false);
            };
        })();
    }, []);

    return (
        <>
            <div>
                <h2>Ustalenie daty zakończenia</h2><br />
            </div>

            <div>
                <h4>Do zgłoszenia : {enrollment.nr}/{enrollment.year}</h4><br />
            </div>
            
            <form onSubmit={handleSubmit(onSubmit)}>
                <label>Ustalona data zakończenia</label><br />
                <Controller
                    control={control}
                    name='dateEndDeclareByDepartment'
                    rules={{
                        validate: value => {
                            const selectedDate = new Date(value);
                            const enrollmentDate = new Date(watch('dateEndDeclareByUser'));
                            return selectedDate >= enrollmentDate || 'Niewłaściwa data.';
                        }
                    }}
                    render={({ field }) => (
                        <DatePicker
                            tabIndex='1'
                            selected={field.value ? new Date(field.value) : null}
                            dateFormat='dd.MM.yyyy'
                            placeholderText='Wybierz datę'
                            disabled={isDisabled}
                            onChange={(date) => field.onChange(setEndOfDay(date))}
                            onBlur={field.onBlur}
                        />
                    )}
                />
                {errors.dateEndDeclareByDepartment ? <p style={{ color: 'red' }} >{errors.dateEndDeclareByDepartment?.message}</p> : <p>&nbsp;</p>}
                <br />

                <label>Ustalono z</label><br />
                <select
                    tabIndex='2'
                    disabled={isDisabled}
                    {...register('userAcceptedEndDate', {
                        validate: value => watch('acceptEndDateByDepartment') || value !== '' || 'Pole wymagane'
                    })}
                >
                    <option value=''>-- Wybierz osobę --</option>
                    {mapUsers.map(option => (
                        <option key={option.value} value={option.value}>{option.name}</option>
                    ))}
                </select>
                {errors.userAcceptedEndDate ? <p style={{ color: 'red' }} >{errors.userAcceptedEndDate?.message}</p> : <p>&nbsp;</p>}
                <br />

                <label>Akceptacja daty zakończenia w/g zgłaszającego</label><br />
                <input
                    tabIndex='3'
                    type='checkbox'
                    disabled={false}
                    {...register('acceptEndDateByDepartment')}
                    onChange={handleEndDateAcceptedChange}
                />
                <br />

                <div>
                    <br />    
                    <button
                        tabIndex='4'
                        type='submit'
                        disabled={isLoading}>
                        Zapisz
                    </button>
                    &nbsp;
                    <button
                        tabIndex='5'
                        type='button'
                        onClick={() => onSubmit(null)}>
                        Anuluj
                    </button>
                </div>
            </form>
        </>
    );
};

export default EnrollmentsDescriptionFormSetEndDate;