import { useCallback, useState, useEffect } from 'react';
import { useParams , Link, useNavigate } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { format, formatISO } from 'date-fns';
import DatePicker from 'react-datepicker';
import { enrollmentServices } from '../../services/EnrollmentServices';
import { enrollmentDescriptionServices } from '../../services/EnrollmentDescriptionServices';

import 'react-datepicker/dist/react-datepicker.css';

const EnrollmentsAddDescriptionSetEndDate = () => {
    const navigate = useNavigate();
    const { enrollmentId } = useParams();
    const [enrollment, setEnrollment] = useState([]);
    const [mapUsers, setMapUsers] = useState([]);

    const { handleSubmit, register, reset, setValue, setFocus, control, watch, clearErrors, trigger, formState: { errors } } = useForm();
    const [isDisabled, setIsDisabled] = useState(false);

    const resetAsyncForm = useCallback(async () => {
        try {
            const usersResponse = await fetch('/Users.json');
            const usersData = await usersResponse.json();
            setMapUsers(usersData);

            const response = await enrollmentServices.edit(enrollmentId);
            if (response.ok) {
                const data = await response.json();

                if (data.dateEndDeclareByUser) {
                    data.dateEndDeclareByDepartment = data.dateEndDeclareByUser;
                };

                reset(data);
                setEnrollment(data);
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

    const handleEndDateAcceptedChange = async (e) => {
        const isChecked = e.target.checked;
        setIsDisabled(isChecked);

        if (isChecked) {
            setValue('dateEndDeclareByDepartment', formatISO(enrollment.dateEndDeclareByUser));
            setValue('userAcceptedEndDate', '');
        };
        
        await trigger('userAcceptedEndDate');
        await clearErrors('userAcceptedEndDate');
        
    };

    const handleErrorResponse = (response, errorMessage) => {
        if (!response.ok) throw errorMessage;
    };

    const onSubmit = async (values) => {
        try {
            const description = values.acceptEndDateByDepartment
                ? `Zgłoszenie przyjęto do realizacji.`
                : `Ustalono datę zakończenia na ${format(new Date(values.dateEndDeclareByDepartment), 'dd.MM.yyyy')} r. Ustalono z ${values.userAcceptedEndDate}.`;

            const data = {
                EnrollmentId: enrollment.id,
                Description: description
            };

            handleErrorResponse(
                await enrollmentDescriptionServices.create(data),
                'Zapis [description] nie powiódł się!');

            enrollment.dateEndDeclareByDepartment = values.dateEndDeclareByDepartment;
            enrollment.state = 'Assigned';

            handleErrorResponse(
                await enrollmentServices.update(enrollment.id, enrollment),
                'Zapis [enrollment] nie powiódł się!2');

            navigate(`/Enrollments/Edit/${enrollment.id}`);
        }
        catch (error) {
            console.error(error);
        };
    };

    useEffect(() => {
        const fetchData = async () => {
            try {
                resetAsyncForm();
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        fetchData();
    }, [resetAsyncForm]);

    useEffect(() => {
        setFocus('acceptEndDateByDepartment');
    }, [setFocus]);

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
                            onChange={(date) => field.onChange(date)}
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
                        type='submit'>
                        Zapisz
                    </button>
                    &nbsp;
                    <Link tabIndex='-1' to={`/Enrollments/Edit/${enrollmentId}`}>
                        <button>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default EnrollmentsAddDescriptionSetEndDate;