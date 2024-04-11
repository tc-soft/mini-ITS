import { useCallback, useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { useForm, Controller } from 'react-hook-form';
import { addDays, getYear, parseISO, formatISO, format } from 'date-fns';
import DatePicker from 'react-datepicker';
import { groupsServices } from '../../services/GroupsServices';
import { enrollmentServices } from '../../services/EnrollmentServices';
import { enrollmentPictureServices } from '../../services/EnrollmentPictureServices';
import { enrollmentDescriptionServices } from '../../services/EnrollmentDescriptionServices';

import 'react-datepicker/dist/react-datepicker.css';

const EnrollmentsForm = (props) => {
    const { isMode, groupsPagedQuery } = props;
    const { currentUser } = useAuth();
    const isReadMode = isMode === 'Detail' ? true : false;
    const navigate = useNavigate();
    const { enrollmentId } = useParams();

    const [enrollment, setEnrollment] = useState([]);
    const [mapDepartment, setMapDepartment] = useState([]);
    const [mapGroup, setMapGroup] = useState([]);
    const [mapEnrollmentsPicture, setMapEnrollmentsPicture] = useState([]);
    const [mapEnrollmentsDescription, setMapEnrollmentsDescription] = useState([]);

    const { handleSubmit, register, reset, setValue, setFocus, control, watch, formState: { errors } } = useForm();
    const [formControls, setFormControls] = useState({
        isDateEndDeclareByUser: isMode === 'Create' ? false : true,
        isDateEndDeclareByDepartmentDisabled: true,
        isDepartmentDisabled: isMode === 'Create' ? false : true,
        isDescriptionDisabled: isMode === 'Create' ? false : true,
        isGroupDisabled: isMode === 'Create' ? false : true,
        isPriorityDisabled: isMode === 'Create' ? false : true,
        isReadyForCloseDisabled: true,
        isStateDisabled: true,
        isActionRequestDisabled: isMode === 'Create' ? false : true
    });

    const title = { Create: 'Nowe zgłoszenie', Detail: 'Szczegóły zgłoszenia', Edit: 'Edycja zgłoszenia' };

    const mapPriority = {
        '0': 'Normalny',
        '1': 'Wysoki',
        '2': 'Krytyczny'
    };

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

                setFormControls(prevFlags => ({
                    ...prevFlags,

                    isDateEndDeclareByUser: isReadMode ||
                        (currentUser.role !== 'Administrator' &&
                            (
                                data.state === 'Closed' ||
                                data.userAddEnrollment !== currentUser.id
                            )
                        ),

                    isDateEndDeclareByDepartmentDisabled: isReadMode || data.state === 'New' || data.state === 'Closed' || currentUser.role !== 'Administrator',

                    isDepartmentDisabled: isReadMode ||
                        (currentUser.role !== 'Administrator' &&
                            (
                                data.state === 'Closed' ||
                                data.userAddEnrollment !== currentUser.id
                            )
                        ),

                    isDescriptionDisabled: isReadMode ||
                        (currentUser.role !== 'Administrator' &&
                            (
                                data.state === 'Closed' ||
                                data.userAddEnrollment !== currentUser.id
                            )
                        ),

                    isGroupDisabled: isReadMode ||
                        (currentUser.role !== 'Administrator' && currentUser.role !== 'Manager' &&
                            (
                                data.state === 'Closed' ||
                                data.userAddEnrollment !== currentUser.id
                            )
                        ),

                    isPriorityDisabled: isReadMode ||
                        (currentUser.role !== 'Administrator' &&
                            (
                                data.state === 'Closed' ||
                                data.userAddEnrollment !== currentUser.id
                            )
                        ),

                    isReadyForCloseDisabled: isReadMode || data.state === 'New' || data.state === 'Closed',

                    isStateDisabled: isReadMode || currentUser.role !== 'Administrator',

                    isActionRequestDisabled: isReadMode ||
                        (currentUser.role !== 'Administrator' &&
                            (
                                data.state === 'Closed' ||
                                data.userAddEnrollment !== currentUser.id
                            ) ||
                            currentUser.role !== 'Manager'
                        ),
                }));

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

    const handleErrorResponse = (response, errorMessage) => {
        if (!response.ok) throw errorMessage;
    };

    const onSubmit = async (values) => {
        try {
            if (isMode === 'Edit') {
                handleErrorResponse(
                    await enrollmentServices.update(values.id, values),
                    'Aktualizacja nie powiodła się!');
            }
            else if (isMode === 'Create') {
                handleErrorResponse(
                    await enrollmentServices.create(values),
                    'Zapis nie powiódł się!');
            };

            navigate('/Enrollments');
        }
        catch (error) {
            console.error(error);
        };
    };

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
                
                if (isMode === 'Create') {
                    const response = await enrollmentServices.getMaxNumber(getYear(new Date()));

                    if (response.ok) {
                        const data = await response.json();
                        setEnrollment(prevState => ({
                            ...prevState,
                            nr: data.maxNumber + 1,
                            year: getYear(new Date())
                        }));
                    };

                    setValue('dateAddEnrollment', formatISO(new Date()));
                    setValue('dateEndDeclareByUser', formatISO(addDays(new Date(), 7)));
                    setValue('sMSToUserInfo', true);
                    setValue('mailToUserInfo', true);
                    setValue('state', 'New');
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

        if (isMode !== 'Create') fetchData();
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

        if (isMode !== 'Create') fetchData();
    }, [enrollmentId]);

    useEffect(() => {
        setFocus('department');
    }, [setFocus]);
    
    return (
        <>
            <div>
                <h2>{title[isMode]}</h2><br />
            </div>

            <div>
                <h4>Zgłoszenie {enrollment.nr}/{enrollment.year}</h4><br />
            </div>

            <form onSubmit={handleSubmit(onSubmit)}>
                <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    flexWrap: 'wrap'
                }}>
                    <div>
                        <label>Data zgłoszenia</label><br />
                        <Controller
                            control={control}
                            name='dateAddEnrollment'
                            rules={{
                                required: { value: true, message: 'Pole wymagane.' },
                            }}
                            render={({ field }) => (
                                <DatePicker
                                    tabIndex='1'
                                    selected={field.value ? new Date(field.value) : null}
                                    dateFormat='dd.MM.yyyy HH:mm'
                                    placeholderText='Wybierz datę'
                                    disabled={true}
                                    onChange={(date) => field.onChange(date.toISOString())}
                                    onBlur={field.onBlur}
                                />
                            )}
                        />
                        {errors.dateAddEnrollment ? <p style={{ color: 'red' }} >{errors.dateAddEnrollment?.message}</p> : <p>&nbsp;</p>}

                        {isMode !== 'Create' &&
                            <>
                                <label>Data ost. zmiany</label><br />
                                <Controller
                                    control={control}
                                    name='dateModEnrollment'
                                    rules={{
                                        required: { value: true, message: 'Pole wymagane.' },
                                    }}
                                    render={({ field }) => (
                                        <DatePicker
                                            tabIndex='2'
                                            selected={field.value ? new Date(field.value) : null}
                                            dateFormat='dd.MM.yyyy HH:mm'
                                            placeholderText='Wybierz datę'
                                            disabled={true}
                                            onChange={(date) => field.onChange(date.toISOString())}
                                            onBlur={field.onBlur}
                                        />
                                    )}
                                />
                                {errors.dateModEnrollment ? <p style={{ color: 'red' }} >{errors.dateModEnrollment?.message}</p> : <p>&nbsp;</p>}
                            </>
                        }

                        <label>Dział docelowy</label><br />
                        <select
                            tabIndex='3'
                            disabled={formControls.isDepartmentDisabled}
                            style={isReadMode ? { pointerEvents: 'none' } : null}
                            {...register('department', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: {
                                    value: /^(?!\s)(?=.*\S).*$/, message: 'Niedozwolony znak.' },
                                maxLength: { value: 50, message: 'Za duża ilośc znaków.' }
                            })}
                        >
                            <option value=''>-- Wybierz dział --</option>
                            {mapDepartment.map(option => (
                                <option key={option.value} value={option.value}>{option.name}</option>
                            ))}
                        </select>
                        {errors.department ? <p style={{ color: 'red' }} >{errors.department?.message}</p> : <p>&nbsp;</p>}

                        <label>Opis</label><br />
                        <textarea
                            tabIndex='4'
                            placeholder='Wpisz treść'
                            rows={4}
                            cols={50}
                            disabled={formControls.isDescriptionDisabled}
                            {...register('description', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: {
                                    value: /^[^\s](.|[\r\n])+[^\s]$/g, message: 'Niedozwolony znak.' },
                                maxLength: { value: 2048, message: 'Za duża ilośc znaków.' }
                            })}
                        />
                        {errors.description ? <p style={{ color: 'red' }} >{errors.description?.message}</p> : <p>&nbsp;</p>}

                        <label>Priorytet</label><br />
                        <select
                            tabIndex='5'
                            disabled={formControls.isPriorityDisabled}
                            {...register('priority', {
                                setValueAs: value => parseInt(value, 10)
                            })}
                        >
                            {Object.keys(mapPriority).map(key => (
                                <option key={key} value={key}>
                                    {mapPriority[key]}
                                </option>
                            ))}
                        </select>
                        <br />

                        {isMode === 'Create' &&
                            <>
                                <br />
                                <label>Czy wysłać SMS do kier. działu</label><br />
                                <input
                                    tabIndex='6'
                                    type='checkbox'
                                    disabled={false}
                                    {...register('sMSToUserInfo')}
                                />
                                <br />

                                <label>Czy wysłać SMS do kier. wszystkich działów</label><br />
                                <input
                                    tabIndex='7'
                                    type='checkbox'
                                    disabled={false}
                                    {...register('sMSToAllInfo')}
                                />
                                <br />

                                <label>Czy wysłac Email do kier. działu</label><br />
                                <input
                                    tabIndex='8'
                                    type='checkbox'
                                    disabled={false}
                                    {...register('mailToUserInfo')}
                                />
                                <br />

                                <label>Czy wysłac Email do kier. wszystkich działów</label><br />
                                <input
                                    tabIndex='9'
                                    type='checkbox'
                                    disabled={false}
                                    {...register('mailToAllInfo')}
                                />
                            </>
                        }

                        {isReadMode && (
                            <>
                                <label>Dodał/a</label><br />
                                <input
                                    tabIndex='10'
                                    type='text'
                                    placeholder='brak'
                                    disabled={isReadMode}
                                    {...register('userAddEnrollmentFullName')}
                                />
                                <br />

                                <label>Zakończył/a</label><br />
                                <input
                                    tabIndex='11'
                                    type='text'
                                    placeholder='brak'
                                    disabled={isReadMode}
                                    {...register('userEndEnrollmentFullName')}
                                />
                                <br />

                                <label>Data zakończenia</label><br />
                                <Controller
                                    control={control}
                                    name='dateEndEnrollment'
                                    render={({ field }) => (
                                        <DatePicker
                                            tabIndex='12'
                                            selected={field.value ? new Date(field.value) : null}
                                            dateFormat='dd.MM.yyyy HH:mm'
                                            placeholderText='brak'
                                            disabled={isReadMode}
                                            onChange={(date) => field.onChange(date.toISOString())}
                                            onBlur={field.onBlur}
                                        />
                                    )}
                                />
                                <br />

                                <label>Otworzył/a ponownie</label><br />
                                <input
                                    tabIndex='13'
                                    type='text'
                                    placeholder='brak'
                                    disabled={isReadMode}
                                    {...register('userEndEnrollmentFullName')}
                                />
                            </>
                        )}
                    </div>
                    <div>
                        <label>Data zak. w/g zgł.</label><br />
                        <Controller
                            control={control}
                            name='dateEndDeclareByUser'
                            rules={{
                                validate: value => {
                                    const selectedDate = new Date(value);
                                    const enrollmentDate = new Date(watch('dateAddEnrollment'));
                                    return selectedDate > enrollmentDate || 'Niewłaściwa data.';
                                }
                            }}
                            render={({ field }) => (
                                <DatePicker
                                    tabIndex='14'
                                    selected={field.value ? new Date(field.value) : null}
                                    dateFormat='dd.MM.yyyy'
                                    placeholderText='Wybierz datę'
                                    disabled={formControls.isDateEndDeclareByUser}
                                    onChange={(date) => field.onChange(date.toISOString())}
                                    onBlur={field.onBlur}
                                />
                            )}
                        />
                        {errors.dateEndDeclareByUser ? <p style={{ color: 'red' }} >{errors.dateEndDeclareByUser?.message}</p> : <p>&nbsp;</p>}

                        <label>Data zak. w/g działu</label><br />
                        <Controller
                            control={control}
                            name='dateEndDeclareByDepartment'
                            rules={{
                                validate: value => {
                                    if (!value) {
                                        return true;
                                    }

                                    const selectedDate = new Date(value);
                                    const enrollmentDate = new Date(watch('dateAddEnrollment'));
                                    return selectedDate > enrollmentDate || 'Niewłaściwa data.';
                                }
                            }}
                            render={({ field }) => (
                                <DatePicker
                                    tabIndex='15'
                                    selected={field.value ? new Date(field.value) : null}
                                    dateFormat='dd.MM.yyyy'
                                    placeholderText='brak'
                                    disabled={formControls.isDateEndDeclareByDepartmentDisabled}
                                    onChange={(date) => field.onChange(date.toISOString())}
                                    onBlur={field.onBlur}
                                />
                            )}
                        />
                        {errors.dateEndDeclareByDepartment ? <p style={{ color: 'red' }} >{errors.dateEndDeclareByDepartment?.message}</p> : <p>&nbsp;</p>}

                        <label>Grupa/Linia</label><br />
                        <select
                            tabIndex='16'
                            disabled={formControls.isGroupDisabled}
                            {...register('group', {
                                required: { value: true, message: 'Pole wymagane.' },
                                pattern: {
                                    value: /^(?!\s)(?=.*\S).*$/, message: 'Niedozwolony znak.'
                                },
                                maxLength: { value: 50, message: 'Za duża ilośc znaków.' }
                            })}
                        >
                            <option value=''>-- Wybierz grupę --</option>
                            {mapGroup?.results?.map((x, y) => (
                                <option key={y} value={x.groupName}>
                                    {x.groupName}
                                </option>
                            ))}
                        </select>
                        {errors.group ? <p style={{ color: 'red' }} >{errors.group?.message}</p> : <p>&nbsp;</p>}

                        <label>Żądanie dodatkowych czynności</label><br />
                        <Controller
                            name='actionRequest'
                            control={control}
                            render={({ field: { onChange, value } }) => (
                                <input
                                    tabIndex='17'
                                    type='checkbox'
                                    checked={value === 1}
                                    onChange={e => onChange(e.target.checked ? 1 : 0)}
                                    disabled={formControls.isActionRequestDisabled}
                                />
                            )}
                        />
                        <br />
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
                    {isMode !== 'Create' &&
                        <>
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
                        </>
                    }

                    {enrollment.state === 'New' &&
                        <>
                            <Link to={`AddDescriptionSetEndDate`}>
                                <button title='Ustalenie daty zakończenmia zgłoszenia'>
                                    <span>Ustalenie daty zakończenia</span>
                                </button>
                            </Link>
                        </>
                    }

                    {enrollment.state !== 'New' &&
                        <>
                            <Link to={`AddDescription`}>
                                <button title='Dodaj adnotację do zgłoszenia'>
                                    <span>Dodaj adnotację</span>
                                </button>
                            </Link>
                        </>
                    }
                </div>
                <div>
                    {isMode !== 'Create' && (
                        <>
                            <br />
                            <label>Wyk. dodatkowych czynności</label><br />
                            <input
                                tabIndex='18'
                                type='checkbox'
                                checked={watch('actionFinished') === 1 ? true : false}
                                disabled={true}
                                {...register('actionFinished')}
                            />
                            <br />

                            <label>Gotowe do zamkn.</label><br />
                            <input
                                tabIndex='19'
                                type='checkbox'
                                disabled={formControls.isReadyForCloseDisabled}
                                {...register('readyForClose')}
                            />
                            <br />

                            <label>Status</label><br />
                            <select
                                tabIndex='20'
                                placeholder='Wybierz status'
                                disabled={formControls.isStateDisabled}
                                {...register('state')}
                            >
                                {Object.keys(mapState).map(key => (
                                    <option key={key} value={key}>
                                        {mapState[key]}
                                    </option>
                                ))}
                            </select>
                        </>
                    )}
                </div>
                <div>
                    <br />
                    {(isMode === 'Edit' || isMode === 'Create') && (
                        <>
                            <button
                                tabIndex='21'
                                type='submit'>
                                Zapisz
                            </button>
                            &nbsp;
                        </>
                    )}
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