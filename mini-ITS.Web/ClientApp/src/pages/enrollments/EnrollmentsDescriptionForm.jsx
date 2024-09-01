import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import iconEdit from '../../images/iconEdit.svg';
import iconEnrollment from '../../images/iconEnrollment.svg';
import iconSave from '../../images/iconSave.svg';
import iconCancel from '../../images/iconCancel.svg';

import '../../styles/pages/Enrollments.scss';

const EnrollmentsDescriptionForm = ({ onSubmit, subForm }) => {
    const { handleSubmit, register, reset, setValue, setFocus, formState: { errors } } = useForm();

    useEffect(() => {
        const fetchDescription = async () => {
            try {
                if (subForm.data.actionDescription === 1) {
                    const descriptionResponse = await fetch('/Description.json');
                    if (!descriptionResponse.ok) {
                        throw new Error('Network response was not ok');
                    };
                    const descriptionData = await descriptionResponse.json();
                    const descriptionObject = descriptionData.find(item => item.name === "Description1");

                    if (descriptionObject) {
                        setValue('description', descriptionObject.value);
                        setValue('actionExecuted', 1);
                    };
                };

                setTimeout(() => {
                    setFocus('description');
                }, 0);
            }
            catch (error) {
                console.error('Fetch error:', error);
            };
        };

        if (subForm.data) {
            reset(subForm.data);
        };

        fetchDescription();
    }, []);

    return (
        <>
            <div className='enrollmentsForm-title'>
                <img src={iconEdit} height='17px' alt='iconEdit' />
                <p>Edycja adnotacji</p>
            </div>

            <div className='enrollmentsForm-enrollmentsInfo'>
                <img src={iconEnrollment} alt='iconEnrollment' />
                <p>Do zgłoszenia:<span>{subForm.data.nr}/{subForm.data.year}</span></p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)}>
                <div className='enrollmentsForm-detail'>
                    <div className='enrollmentsForm-detail-block'>
                        <label>Treść adnotacji</label><br />
                        <textarea className='enrollmentsForm-detail-section__input'
                            tabIndex='1'
                            placeholder='Wpisz treść'
                            rows={4}
                            cols={50}
                            disabled={false}
                            {...register('description', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: {
                                    value: /^[^\s](.|[\r\n])+[^\s]$/g, message: 'Niedozwolony znak.'
                                },
                                maxLength: { value: 2048, message: 'Za duża ilośc znaków.' }
                            })}
                        />
                        {errors.description ? <p style={{ color: 'red' }} >{errors.description?.message}</p> : <p>&nbsp;</p>}
                    </div>
                </div>
                <div className='enrollmentsForm-submit'>
                    <button
                        tabIndex='2'
                        type='submit'
                        className='enrollmentsForm-submit__button enrollmentsForm-submit__button--saveButton'>
                        <img src={iconSave} alt='iconSave' />
                        Zapisz
                    </button>
                    <button
                        tabIndex='3'
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

export default EnrollmentsDescriptionForm;