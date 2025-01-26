import { useState, useEffect } from 'react';
import { useForm, Controller } from 'react-hook-form';
import DatePicker from 'react-datepicker';
import pl from "date-fns/locale/pl";
import iconEdit from '../../images/iconEdit.svg';
import iconEnrollment from '../../images/iconEnrollment.svg';
import iconSave from '../../images/iconSave.svg';
import iconCancel from '../../images/iconCancel.svg';

import 'react-datepicker/dist/react-datepicker.css';
import '../../styles/pages/Enrollments.scss';

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
            <div className='enrollmentsForm-title'>
                <img src={iconEdit} height='17px' alt='iconEdit' />
                <p>Ustalenie daty zakończenia</p>
            </div>

            <div className='enrollmentsForm-enrollmentsInfo'>
                <img src={iconEnrollment} alt='iconEnrollment' />
                <p>Do zgłoszenia:<span>{enrollment.nr}/{enrollment.year}</span></p>
            </div>
            
            <form onSubmit={handleSubmit(onSubmit)}>
                <div className='enrollmentsForm-detail'>
                    <div className='enrollmentsForm-detail-block'>
                        <label className='enrollmentsForm-detail-section__label'>Ustalona data zakończenia</label>
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
                                    className='enrollmentsForm-detail-section__datePicker'
                                    dayClassName={(date) => date.getDay() === 0 ? 'enrollmentsForm-detail-section__datePicker--highlightedSunday' : undefined}
                                    locale={pl}
                                    
                                />
                            )}
                        />
                        {errors.dateEndDeclareByDepartment ? <p style={{ color: 'red' }} >{errors.dateEndDeclareByDepartment?.message}</p> : <p>&nbsp;</p>}

                        <label className='enrollmentsForm-detail-section__label'>Ustalono z</label>
                        <select className='enrollmentsForm-detail-section__input'
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

                        <label className='enrollmentsForm-detail-section__label'>Akceptacja daty zakończenia w/g zgłaszającego</label>
                        <input
                            tabIndex='3'
                            type='checkbox'
                            disabled={false}
                            className='enrollmentsForm-detail-section__input'
                            {...register('acceptEndDateByDepartment')}
                            onChange={handleEndDateAcceptedChange}
                        />
                    </div>
                </div>
                <div className='enrollmentsForm-submit'>
                    <button
                        tabIndex='4'
                        type='submit'
                        disabled={isLoading}
                        className='enrollmentsForm-submit__button enrollmentsForm-submit__button--saveButton'>
                        <img src={iconSave} alt='iconSave' />
                        Zapisz
                    </button>
                    <button
                        tabIndex='5'
                        type='button'
                        onClick={() => onSubmit(null)}
                        className='enrollmentsForm-submit__button enrollmentsForm-submit__button--cancelButton'>
                        <img src={iconCancel} alt='iconCancel' />
                        Anuluj
                    </button>
                </div>
            </form>
        </>
    );
};

export default EnrollmentsDescriptionFormSetEndDate;