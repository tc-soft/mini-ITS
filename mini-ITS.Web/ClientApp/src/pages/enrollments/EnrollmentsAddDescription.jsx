import { useCallback, useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { enrollmentServices } from '../../services/EnrollmentServices';
import { enrollmentDescriptionServices } from '../../services/EnrollmentDescriptionServices';

const EnrollmentsAddDescription = () => {
    const navigate = useNavigate();
    const { enrollmentId } = useParams();
    const [enrollment, setEnrollment] = useState([]);
    const { handleSubmit, register, setFocus, formState: { errors } } = useForm();

    const resetAsyncForm = useCallback(async () => {
        try {
            const response = await enrollmentServices.edit(enrollmentId);
            if (response.ok) {
                const data = await response.json();

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
    }, [enrollmentId]);

    const handleErrorResponse = (response, errorMessage) => {
        if (!response.ok) throw errorMessage;
    };

    const onSubmit = async (values) => {
        try {
            const data = {
                EnrollmentId: enrollment.id,
                Description: values.description
            };

            handleErrorResponse(
                await enrollmentDescriptionServices.create(data),
                'Zapis [description] nie powiódł się!');

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
        setFocus('description');
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

export default EnrollmentsAddDescription;