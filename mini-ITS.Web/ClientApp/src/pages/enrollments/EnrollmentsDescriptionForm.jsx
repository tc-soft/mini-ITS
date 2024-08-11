import { useEffect } from 'react';
import { useForm } from 'react-hook-form';

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
            <div>
                <h2>Edycja adnotacji</h2><br />
            </div>

            <div>
                <h4>Do zgłoszenia : {subForm.data.nr}/{subForm.data.year}</h4><br />
            </div>

            <form onSubmit={handleSubmit(onSubmit)}>
                <label>Treść adnotacji</label><br />
                <textarea
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

                <div>
                    <br />
                    <button
                        tabIndex='2'
                        type='submit'>
                        Zapisz
                    </button>
                    &nbsp;
                    <button
                        tabIndex='3'
                        type='button'
                        onClick={() => onSubmit(null)}>
                        Anuluj
                    </button>
                </div>
            </form>
        </>
    );
};

export default EnrollmentsDescriptionForm;