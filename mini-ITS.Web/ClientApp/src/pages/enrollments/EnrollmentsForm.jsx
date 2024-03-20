import { useCallback, useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { parseISO, format } from 'date-fns';
import DatePicker from 'react-datepicker';
import { groupsServices } from '../../services/GroupsServices';
import { enrollmentServices } from '../../services/EnrollmentServices';
import { enrollmentPictureServices } from '../../services/EnrollmentPictureServices';
import { enrollmentDescriptionServices } from '../../services/EnrollmentDescriptionServices';

const EnrollmentsForm = (props) => {
    const { isMode, groupsPagedQuery } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const { enrollmentId } = useParams();

    const [enrollment, setEnrollment] = useState([]);
    const [mapDepartment, setMapDepartment] = useState([]);
    const [mapGroup, setMapGroup] = useState([]);
    const [mapEnrollmentsPicture, setMapEnrollmentsPicture] = useState([]);
    const [mapEnrollmentsDescription, setMapEnrollmentsDescription] = useState([]);
    
    const { register, reset, watch } = useForm();
    const title = { Create: 'Dodaj zgłoszenie', Detail: 'Szczegóły zgłoszenia', Edit: 'Edycja' };

    const mapState = {
        'New': 'Nowy',
        'Assigned': 'W trakcie',
        'Closed': 'Zamknięte',
        'ReOpened': 'Otwarte ponownie'
    };

    const resetAsyncForm = useCallback(async () => {
        try {
            const response = await enrollmentServices.edit(enrollmentId);
            if (response.ok) {
                const data = await response.json();
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

    useEffect(() => {
        const fetchData = async () => {
            try {
                const departmentResponse = await fetch('/Department.json');
                const departmentData = await departmentResponse.json();
                setMapDepartment(departmentData);

                const response = await groupsServices.index(groupsPagedQuery);
                if (response.ok) {
                    const data = await response.json();
                    setMapGroup(data);
                }
                else {
                    throw new Error('Network response was not ok');
                };

                if (isMode === 'Detail' || isMode === 'Edit') {
                    resetAsyncForm();
                };
            }
            catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        fetchData();
    }, [resetAsyncForm]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const enrollmentsPictureResponse = await enrollmentPictureServices.index({ id: enrollmentId });
                if (enrollmentsPictureResponse.ok) {
                    const enrollmentsPicture = await enrollmentsPictureResponse.json();
                    const mapEnrollmentsPicture = await Promise.all(enrollmentsPicture.map(async (enrollmentPicture) => {
                        try {
                            const enrollmentPictureResponse = await enrollmentPictureServices.edit(enrollmentPicture.id);
                            if (enrollmentPictureResponse.ok) {
                                const enrollmentPictureData = await enrollmentPictureResponse.json();
                                return enrollmentPictureData;
                            } else {
                                console.error(`Failed to load image data for picture ${enrollmentPicture.id}`);
                                return enrollmentPicture;
                            };
                        } catch (imageError) {
                            console.error(`Failed to load image data for picture ${enrollmentPicture.id}:`, imageError);
                            return enrollmentPicture;
                        };
                    }));

                    setMapEnrollmentsPicture(mapEnrollmentsPicture);
                } else {
                    throw new Error('Network response was not ok for enrollment pictures');
                };
            } catch (error) {
                console.error('Error fetching data:', error);
            };
        };

        fetchData();
    }, [enrollmentId]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await enrollmentDescriptionServices.index({ id: enrollmentId });
                if (response.ok) {
                    const enrollmentDescription = await response.json();
                    setMapEnrollmentsDescription(enrollmentDescription);
                } else {
                    throw new Error('Network response was not ok for enrollment descriptions');
                };
            } catch (error) {
                console.error('Error fetching enrollment descriptions:', error);
            };
        };

        fetchData();
    }, [enrollmentId]);

    return (
        <>
            <div>
                <h2>{title[isMode]}</h2><br />
            </div>

            <div>
                <h4>Dane zgłoszenia { enrollment.nr }/{ enrollment.year }</h4><br />
            </div>

            <form>
                <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    flexWrap: 'wrap'
                }}>
                    <div>
                        <label>Data zgłoszenia</label><br />
                        <DatePicker
                            tabIndex='1'
                            selected={watch('dateAddEnrollment') ? parseISO(watch('dateAddEnrollment')) : null}
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText={watch('DateAddEnrollment') ? undefined : 'Wybierz datę'}
                            disabled={isReadMode}
                            {...register('dateAddEnrollment', {
                                required: { value: true, message: 'Pole wymagane.' }
                            })}
                        />

                        <label>Data ost. zmiany</label><br />
                        <DatePicker
                            tabIndex='2'
                            selected={watch('dateModEnrollment') ? parseISO(watch('dateModEnrollment')) : null}
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText={watch('DateModEnrollment') ? undefined : 'Wybierz datę'}
                            disabled={isReadMode}
                            {...register('dateModEnrollment', {
                                required: { value: true, message: 'Pole wymagane.' }
                            })}
                        />

                        <label>Dział</label><br />
                        <select
                            tabIndex='3'
                            placeholder='Wybierz dział'
                            disabled={isReadMode}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('department')}
                        >
                            {mapDepartment.map(option => (
                                <option key={option.value} value={option.value}>{option.name}</option>
                            ))}
                        </select>
                        <br />

                        <label>Opis</label><br />
                        <textarea
                            tabIndex='4'
                            placeholder='Wpisz treść'
                            rows={4}
                            cols={50}
                            disabled={isReadMode}
                            {...register('description')}
                        />
                        <br />

                        <label>Priorytet</label><br />
                        <input
                            tabIndex='5'
                            type='text'
                            placeholder='Wybierz priorytet'
                            disabled={isReadMode}
                            {...register('priority')}
                        />
                        <br />

                        <label>Gotowe do zamkn.</label><br />
                        <input
                            tabIndex='6'
                            type='checkbox'
                            checked={watch('readyForCloseValue')}
                            disabled={isReadMode}
                            {...register('readyForClose')}
                        />
                        <br />

                        <label>Status</label><br />
                        <select
                            tabIndex='7'
                            placeholder='Wybierz status'
                            disabled={isReadMode}
                            {...register('state')}
                        >
                            {Object.keys(mapState).map(key => (
                                <option key={key} value={key}>
                                    {mapState[key]}
                                </option>
                            ))}
                        </select>
                        <br />

                        <label>Dodał/a</label><br />
                        <input
                            tabIndex='8'
                            type='text'
                            disabled={isReadMode}
                            {...register('userAddEnrollmentFullName')}
                        />
                        <br />

                        <label>Zakończył/a</label><br />
                        <input
                            tabIndex='9'
                            type='text'
                            disabled={isReadMode}
                            {...register('userEndEnrollmentFullName')}
                        />
                        <br />

                        <label>Data zakończenia</label><br />
                        <DatePicker
                            tabIndex='10'
                            selected={watch('DateEndEnrollment') ? parseISO(watch('DateEndEnrollment')) : null}
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText={watch('DateEndEnrollment') ? undefined : 'brak'}
                            disabled={isReadMode}
                            {...register('DateEndEnrollment')}
                        />

                        <label>Otworzył/a ponownie</label><br />
                        <input
                            tabIndex='11'
                            type='text'
                            value={watch('userReeEnrollmentFullName') || 'brak'}
                            disabled={isReadMode}
                        />
                    </div>
                    <div>
                        <label>Data zak. w/g zgł.</label><br />
                        <DatePicker
                            tabIndex='12'
                            selected={watch('dateEndDeclareByUser') ? parseISO(watch('dateEndDeclareByUser')) : null}
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText={watch('dateEndDeclareByUser') ? undefined : 'Wybierz datę'}
                            disabled={isReadMode}
                            {...register('dateEndDeclareByUser', {
                                required: { value: true, message: 'Pole wymagane.' }
                            })}
                        />

                        <label>Data zak. w/g działu</label><br />
                        <DatePicker
                            tabIndex='13'
                            selected={watch('dateEndDeclareByDepartment') ? parseISO(watch('dateEndDeclareByDepartment')) : null}
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText={watch('dateEndDeclareByDepartment') ? undefined : 'Wybierz datę'}
                            disabled={isReadMode}
                            {...register('dateEndDeclareByDepartment', {
                                required: { value: true, message: 'Pole wymagane.' }
                            })}
                        />

                        <label>Grupa/Linia</label><br />
                        <select
                            tabIndex='14'
                            placeholder='Wybierz grupę'
                            disabled={isReadMode}
                            {...register('group')}
                        >
                            {mapGroup?.results?.map((x, y) => (
                                <option key={y} value={x.groupName}>
                                    {x.groupName}
                                </option>
                            ))}
                        </select>
                        <br /><br />

                        <label>Żądanie dodatkowych czynności</label><br />
                        <input
                            tabIndex='15'
                            type='checkbox'
                            checked={watch('actionRequest') === 1 ? true : false}
                            disabled={isReadMode}
                            {...register('actionRequest')}
                        />
                        <br />

                        <label>Wyk. dodatkowych czynności</label><br />
                        <input
                            tabIndex='16'
                            type='checkbox'
                            checked={watch('actionFinished') === 1 ? true : false}
                            disabled={isReadMode}
                            {...register('actionFinished')}
                        />
                        
                    </div>
                </div>
                <br />
                <div>
                    {mapEnrollmentsPicture.map((enrollmentsPicture) => (
                        <div key={enrollmentsPicture.id}
                            style={{ display: 'flex', alignItems: 'center', marginBottom: '10px' }}>
                            <img src={`data:image/jpeg;base64,${enrollmentsPicture.pictureBytes}`}
                                alt={enrollmentsPicture.picturePath}
                                style={{ width: '400px', height: 'auto', marginRight: '10px' }}
                            />
                        </div>
                    ))}
                </div>
                <div>
                    <table>
                        <thead>
                            <tr>
                                <th>Data wpr.</th>
                                <th>Adnotacja</th>
                                <th>Dodał</th>
                            </tr>
                        </thead>
                        <tbody>
                            {mapEnrollmentsDescription.map((enrollmentsDescription, index) => (
                                <tr key={index}>
                                    <td>{format(parseISO(enrollmentsDescription.dateAddDescription), 'dd.MM.yyyy HH:mm')}</td>
                                    <td>{enrollmentsDescription.description}</td>
                                    <td>{enrollmentsDescription.userAddDescriptionFullName}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
                <div>
                    <br />
                    <Link tabIndex='-1' to={'..'}>
                        <button>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default EnrollmentsForm;