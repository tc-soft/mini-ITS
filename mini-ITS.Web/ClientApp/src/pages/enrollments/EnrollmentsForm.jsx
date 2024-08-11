import { useCallback, useState, useEffect, useRef } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';
import { useForm, Controller } from 'react-hook-form';
import { addDays, getYear, parseISO, format, isValid } from 'date-fns';
import DatePicker from 'react-datepicker';
import { v4 as uuidv4 } from 'uuid';
import { groupsServices } from '../../services/GroupsServices';
import { enrollmentServices } from '../../services/EnrollmentServices';
import { enrollmentPictureServices } from '../../services/EnrollmentPictureServices';
import { enrollmentDescriptionServices } from '../../services/EnrollmentDescriptionServices';
import EnrollmentsDescriptionForm from './EnrollmentsDescriptionForm';
import EnrollmentsDescriptionFormSetEndDate from './EnrollmentsDescriptionFormSetEndDate';
import ModalDialog from '../../components/Modal';

import 'react-datepicker/dist/react-datepicker.css';

const EnrollmentsForm = (props) => {
    const { isMode, groupsPagedQuery } = props;
    const { currentUser } = useAuth();
    const isReadMode = isMode === 'Detail' ? true : false;
    const navigate = useNavigate();
    const { enrollmentId } = useParams();
    const [subForm, setSubForm] = useState({ isSubForm: null, data: {} });
    
    const [enrollment, setEnrollment] = useState([]);
    const [mapDepartment, setMapDepartment] = useState([]);
    const [mapGroup, setMapGroup] = useState([]);
    const [mapEnrollmentsPicture, setMapEnrollmentsPicture] = useState([]);
    const [mapEnrollmentsDescription, setMapEnrollmentsDescription] = useState([]);

    const isEnrollmentRefLoaded = useRef(false);
    const isEnrollmentPicturesRefLoaded = useRef(false);
    const isEnrollmentDescriptionsRefLoaded = useRef(false);
    const [isEnrollmentLoaded, setIsEnrollmentLoaded] = useState(true);
    const [isEnrollmentPicturesLoaded, setIsEnrollmentPicturesLoaded] = useState(true);
    const [isEnrollmentDescriptionsLoaded, setIsEnrollmentDescriptionsLoaded] = useState(true);
    const isLoaded = isEnrollmentLoaded || isEnrollmentPicturesLoaded || isEnrollmentDescriptionsLoaded;
    const [isReady, setIsReady] = useState(false);

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
    const fileInputRef = useRef(null);
        
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

    const [modalDialogOpen, setModalDialogOpen] = useState(false);
    const [modalDialogType, setModalDialogType] = useState('');
    const [modalDialogTitle, setModalDialogTitle] = useState('');
    const [modalDialogMessage, setModalDialogMessage] = useState('');
    const [modalDialogPictureId, setModalDialogPictureId] = useState('');
    const [modalDialogDescriptionId, setModalDialogDescriptionId] = useState('');

    const convertDatesToUTC = (obj) => {
        const ensureUTC = (dateString) => {
            if (!dateString.endsWith('Z')) {
                return dateString + 'Z';
            }
            return dateString;
        };

        for (let key in obj) {
            if (typeof obj[key] === 'string' && isValid(parseISO(obj[key]))) {
                obj[key] = ensureUTC(obj[key]);
            } else if (typeof obj[key] === 'object' && obj[key] !== null) {
                convertDatesToUTC(obj[key]);
            }
        }
        return obj;
    };

    const setEndOfDay = (date) => {
        date.setHours(23);
        date.setMinutes(59);
        date.setSeconds(59);
        date.setMilliseconds(0);
        return date.toISOString();
    };

    const resetAsyncForm = useCallback(async () => {
        try {
            const response = await enrollmentServices.edit(enrollmentId);
            if (response.ok) {
                const data = await response.json();

                convertDatesToUTC(data);

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

                    isReadyForCloseDisabled: isReadMode || data.state === 'New' || data.state === 'Closed' || currentUser.department !== data.department,

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

    const handleModalClose = () => {
        setModalDialogType('');
        setModalDialogTitle('');
        setModalDialogMessage('')
        setModalDialogPictureId('');
        setModalDialogDescriptionId('');
        setModalDialogOpen(false);
    };

    const handleModalConfirm = async () => {
        switch (modalDialogType) {
            case 'Dialog':
                setModalDialogOpen(false);
                modalDialogPictureId !== '' && deletePicture(modalDialogPictureId);
                modalDialogDescriptionId !== '' && deleteDescription(modalDialogDescriptionId);
                break;
            case 'Information':
                handleModalClose();
                break;
            case 'Error':
                handleModalClose();
                break;
            default:
                break;
        };
    };

    const handleDeletePictureStage1 = (pictureId) => {
        setModalDialogType('Dialog');
        setModalDialogTitle('Usuwanie zdjęcia');
        setModalDialogMessage(`Czy na pewno chcesz usunąć zdjęcie ?`);
        setModalDialogPictureId(pictureId);
        setModalDialogOpen(true);
    };

    const handleDeleteDescriptionStage1 = (descriptionId) => {
        setModalDialogType('Dialog');
        setModalDialogTitle('Usuwanie wpisu');
        setModalDialogMessage(`Czy na pewno chcesz usunąć wpis ?`);
        setModalDialogDescriptionId(descriptionId);
        setModalDialogOpen(true);
    };

    const handleFileCountErrorStage1 = (errorMessage) => {
        setModalDialogType('Error');
        setModalDialogTitle('Ilość plików');
        setModalDialogMessage(errorMessage);
        setModalDialogOpen(true);
    };

    const handleFileSizeErrorStage1 = (errorMessage) => {
        setModalDialogType('Error');
        setModalDialogTitle('Rozmiar pliku');
        setModalDialogMessage(errorMessage);
        setModalDialogOpen(true);
    };

    const handleCloseErrorStage1 = (errorMessage) => {
        setModalDialogType('Error');
        setModalDialogTitle('Wymagana czynność');
        setModalDialogMessage(errorMessage);
        setModalDialogOpen(true);
    };

    const handleErrorResponse = (response, errorMessage) => {
        if (!response.ok) throw errorMessage;
    };

    const handleEnrollmentsDescriptionFormSetEndDateSubmit = async (data) => {
        if (data) {
            try {
                data.enrollmentId = enrollment.id,
                    data.description = data.acceptEndDateByDepartment
                        ? `Zgłoszenie przyjęto do realizacji.`
                        : `Ustalono datę zakończenia na ${format(new Date(data.dateEndDeclareByDepartment), 'dd.MM.yyyy')} r. Ustalono z ${data.userAcceptedEndDate}.`;

                setMapEnrollmentsDescription((prevDescriptions) =>
                    [...prevDescriptions,
                    {
                        ...data,
                        id: uuidv4(),
                        status: 'added',
                        dateAddDescription: new Date().toISOString(),
                        userAddDescription: currentUser.id,
                        userAddDescriptionFullName: `${currentUser.firstName} ${currentUser.lastName}`
                    }
                    ]
                );

                setValue('dateEndDeclareByDepartment', data.dateEndDeclareByDepartment);
                setValue('state', 'Assigned');
            }
            catch (error) {
                console.error(error);
            };
        }

        setSubForm({ isSubForm: null, data: {} });
    }

    const handleEnrollmentsDescriptionFormSubmit = (data) => {
        if (data) {
            try {
                if (data.status === 'edited') {
                    setMapEnrollmentsDescription((prevDescriptions) =>
                        prevDescriptions.map(description =>
                            description.id === data.id
                                ? {
                                    ...description,
                                    description: data.description,
                                    status: description.status === 'added' ? description.status : data.status
                                }
                                : description
                        )
                    );
                }
                else if (data.status === 'added') {
                    setMapEnrollmentsDescription((prevDescriptions) =>
                        [...prevDescriptions,
                        {
                            ...data,
                            id: uuidv4(),
                            dateAddDescription: new Date().toISOString(),
                            userAddDescription: currentUser.id,
                            userAddDescriptionFullName: `${currentUser.firstName} ${currentUser.lastName}`
                        }
                        ]
                    )
                };
            }
            catch (error) {
                console.error(error);
            };
        };

        setSubForm({ isSubForm: null, data: {} });
    };

    const state = watch('state');

    const handleStateChange = (event) => {
        const selectedState = event.target.value;
        const isReadyForClose = watch('readyForClose');
        const hasActionRequest = watch('actionRequest');
        const hasActionExecuted = mapEnrollmentsDescription.some(
            description => description.actionExecuted === 1 && description.status !== 'deleted'
        );

        const handleError = (message) => {
            handleCloseErrorStage1(message);
            setValue('state', state);
        };

        if (selectedState === 'Closed') {
            if (!isReadyForClose) {
                handleError('Zgłoszenie nie jest gotowe do zamknięcia przez osoby z dz. docelowego');
            }
            else if (hasActionRequest && !hasActionExecuted) {
                handleError('Nie wykonano dodatkowych czynności. Zgłoszenie nie może być zamknięte.');
            }
            else {
                setValue('state', selectedState);
            };
        } else {
            setValue('state', selectedState);
        };
    };

    const addPicture = (event) => {
        const files = [...event.target.files];
        const maxFileSize = 2 * 1024 * 1024;
        const maxFileCount = 10;
        const currentFileCount = mapEnrollmentsPicture.length;
        const formatFileSize = (size) => {
            if (size >= 1024 * 1024 * 1024) {
                return (size / (1024 * 1024 * 1024)).toFixed(2) + ' GB';
            } else if (size >= 1024 * 1024) {
                return (size / (1024 * 1024)).toFixed(2) + ' MB';
            } else if (size >= 1024) {
                return (size / 1024).toFixed(2) + ' kB';
            } else {
                return size + ' B';
            }
        };

        if (currentFileCount + files.length > maxFileCount) {
            handleFileCountErrorStage1(`Nie można dodać więcej niż ${maxFileCount} plików do zgłoszenia.`);
            event.target.value = null;
            return;
        };

        for (let i = 0; i < files.length; i++) {
            if (files[i].size > maxFileSize) {
                files.length == 1 && handleFileSizeErrorStage1(`Wybrany plik jest za duży. Maksymalny rozmiar pliku to ${formatFileSize(maxFileSize)}.`);
                files.length > 1 && handleFileSizeErrorStage1(`Jeden z wybranych plików jest za duży. Maksymalny rozmiar pliku to ${formatFileSize(maxFileSize)}.`);
                event.target.value = null;
                return;
            };
        };

        files.forEach(file => {
            const newPicture = {
                id: uuidv4(),
                file,
                pictureBytes: null,
                picturePath: file.name
            };
            setMapEnrollmentsPicture(prevPictures => [...prevPictures, newPicture]);

            const reader = new FileReader();
            reader.onload = (e) => {
                const base64 = e.target.result.split(',')[1];
                setMapEnrollmentsPicture(prevPictures => {
                    return prevPictures.map(picture => {
                        if (picture.file === file) {
                            return { ...picture, pictureBytes: base64 };
                        };

                        return picture;
                    });
                });
            };
            reader.readAsDataURL(file);
        });
    };

    const deletePicture = (id) => {
        setMapEnrollmentsPicture(currentPictures => currentPictures.filter(picture => picture.id !== id));
    };

    const deleteDescription = (id) => {
        setMapEnrollmentsDescription((prevDescriptions) => {
            const updatedDescriptions = prevDescriptions.filter(description =>
                description.id !== id || description.status !== 'added'
            ).map(description =>
                description.id === id && description.status !== 'added'
                    ? { ...description, status: 'deleted' }
                    : description
            );

            setTimeout(() => {
                if (updatedDescriptions.every(description => description.status === 'deleted')) {
                    setValue('dateEndDeclareByDepartment', null);
                    setValue('state', 'New');
                }
            }, 0);

            return updatedDescriptions;
        });
    };

    const renderSubForm = () => {
        if (subForm.isSubForm === 'createDescription') {
            return <EnrollmentsDescriptionForm
                onSubmit={handleEnrollmentsDescriptionFormSubmit}
                subForm={subForm}
            />;
        };

        if (subForm.isSubForm === 'createDescriptionSetEndDate') {
            return <EnrollmentsDescriptionFormSetEndDate
                onSubmit={handleEnrollmentsDescriptionFormSetEndDateSubmit}
                subForm={subForm}
            />;
        };

        if (subForm.isSubForm === 'editDescription') {
            return <EnrollmentsDescriptionForm
                onSubmit={handleEnrollmentsDescriptionFormSubmit}
                subForm={subForm}
            />;
        };

        return null;
    };

    const onSubmit = async (values) => {
        try {
            const updateEnrollmentPictures = async (enrollmentId) => {
                const picturesFromApiResponse = await enrollmentPictureServices.index({ id: enrollmentId });
                const picturesFromApi = picturesFromApiResponse.ok ? await picturesFromApiResponse.json() : [];
                const deletedPictures = picturesFromApi.filter(pictureFromApi =>
                    !mapEnrollmentsPicture.some(mapEnrollmentPicture => mapEnrollmentPicture.id === pictureFromApi.id)
                );

                for (const picture of deletedPictures) {
                    await enrollmentPictureServices.delete(picture.id);
                };

                const newPictures = mapEnrollmentsPicture.filter(picture => picture.file);

                if (newPictures.length > 0) {
                    const formData = new FormData();
                    formData.append('enrollmentId', enrollmentId);
                    newPictures.forEach(picture => {
                        formData.append('files', picture.file, picture.picturePath);
                    });

                    const uploadResponse = await enrollmentPictureServices.create(formData);
                    if (!uploadResponse.ok) {
                        throw new Error('Nie udało się zapisać grafik.');
                    };
                };
            };

            const updateEnrollmentDescriptions = async (enrollmentId) => {
                const addedDescriptions = mapEnrollmentsDescription.filter(description => description.status === 'added');
                for (const description of addedDescriptions) {
                    const response = await enrollmentDescriptionServices.create({
                        ...description,
                        enrollmentId: enrollmentId
                    });
                    handleErrorResponse(response, `Dodanie wpisu nie powiodło się!`);
                };

                const editedDescriptions = mapEnrollmentsDescription.filter(description => description.status === 'edited');
                for (const description of editedDescriptions) {
                    const response = await enrollmentDescriptionServices.update(description.id, description);
                    handleErrorResponse(response, `Aktualizacja wpisu ${description.id} nie powiodła się!`);
                };

                const deletedDescriptions = mapEnrollmentsDescription.filter(description => description.status === 'deleted');
                for (const description of deletedDescriptions) {
                    const response = await enrollmentDescriptionServices.delete(description.id);
                    handleErrorResponse(response, `Usunięcie wpisu ${description.id} nie powiodła się!`);
                };
            };

            let enrollmentId;

            if (isMode === 'Edit') {
                const response = await enrollmentServices.update(values.id, values);
                handleErrorResponse(response, 'Aktualizacja nie powiodła się!');
                enrollmentId = values.id;

                await updateEnrollmentPictures(enrollmentId);
                await updateEnrollmentDescriptions(enrollmentId);

            }
            else if (isMode === 'Create') {
                const response = await enrollmentServices.create(values);
                handleErrorResponse(response, 'Zapis nie powiódł się!');
                enrollmentId = await response.json();

                await updateEnrollmentPictures(enrollmentId);
                await updateEnrollmentDescriptions(enrollmentId);
            };

            navigate('/Enrollments');
        }
        catch (error) {
            console.error(error);
        };
    };

    useEffect(() => {
        if (!isEnrollmentLoaded || isEnrollmentRefLoaded.current) return;
        isEnrollmentRefLoaded.current = true;

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
                    await resetAsyncForm();
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

                    setValue('dateAddEnrollment', new Date().toISOString());
                    setValue('dateEndDeclareByUser', (() => {
                        const date = new Date();
                        date.setHours(23);
                        date.setMinutes(59);
                        date.setSeconds(59);
                        date.setMilliseconds(0);
                        return addDays(date, 7).toISOString();
                    })());
                    setValue('sMSToUserInfo', true);
                    setValue('mailToUserInfo', true);
                    setValue('state', 'New');

                    setTimeout(() => {
                        setFocus('department');
                    }, 0);
                };
            }
            catch (error) {
                console.error('Error fetching data:', error);
            }
            finally {
                setIsEnrollmentLoaded(false);
            };
        };

        fetchData();
    }, []);

    useEffect(() => {
        if (!isEnrollmentPicturesLoaded || isEnrollmentPicturesRefLoaded.current) return;
        isEnrollmentPicturesRefLoaded.current = true;

        const fetchData = async () => {
            if (isMode === 'Create') return;

            try {
                const enrollmentsPictureResponse = await enrollmentPictureServices.index({ id: enrollmentId });
                if (enrollmentsPictureResponse.ok) {
                    const enrollmentsPicture = await enrollmentsPictureResponse.json();
                    const mapEnrollmentsPicture = await Promise.all(enrollmentsPicture.map(async (enrollmentPicture) => {
                        try {
                            const enrollmentPictureResponse = await enrollmentPictureServices.edit(enrollmentPicture.id);
                            if (enrollmentPictureResponse.ok) {
                                const enrollmentPictureData = await enrollmentPictureResponse.json();
                                return {
                                    id: enrollmentPictureData.id,
                                    file: null,
                                    pictureBytes: enrollmentPictureData.pictureBytes,
                                    picturePath: enrollmentPictureData.picturePath
                                };
                            } else {
                                console.error(`Failed to load image data for picture ${enrollmentPicture.id}`);
                            };
                        }
                        catch (error) {
                            console.error(`Error loading image for picture ${enrollmentPicture.id}:`, error);
                        };
                    }));

                    setMapEnrollmentsPicture(mapEnrollmentsPicture);
                } else {
                    throw new Error('Network response was not ok for enrollment pictures');
                };
            } catch (error) {
                console.error('Error fetching data:', error);
            }
            finally {
                setIsEnrollmentPicturesLoaded(false);
            };
        };

        fetchData();
    }, []);

    useEffect(() => {
        if (!isEnrollmentDescriptionsLoaded || isEnrollmentDescriptionsRefLoaded.current) return;
        isEnrollmentDescriptionsRefLoaded.current = true;

        const fetchData = async () => {
            try {
                const response = await enrollmentDescriptionServices.index({ id: enrollmentId });
                if (response.ok) {
                    let enrollmentDescription = await response.json();
                    enrollmentDescription = enrollmentDescription.map(item => ({
                        ...item,
                        status: null
                    }));

                    setMapEnrollmentsDescription(enrollmentDescription);
                    convertDatesToUTC(enrollmentDescription);
                }
                else {
                    throw new Error('Network response was not ok for enrollment descriptions');
                };
            }
            catch (error) {
                console.error('Error fetching enrollment descriptions:', error);
            }
            finally {
                setIsEnrollmentDescriptionsLoaded(false);
            };
        };

        if (isMode !== 'Create') fetchData();
    }, []);

    useEffect(() => {
        if (!isLoaded) {
            setIsReady(true);
        };
    }, [isLoaded]);

    useEffect(() => {
        if (isReady) {
            const actionExecutedValue = mapEnrollmentsDescription.some(description => description.actionExecuted === 1 && description.status !== 'deleted') ? 1 : 0;
            setValue('actionExecuted', actionExecutedValue);
            setValue('actionFinished', actionExecutedValue === 1);
        };
    }, [isReady, mapEnrollmentsDescription]);

    return (
        <>
            <ModalDialog
                modalDialogOpen={modalDialogOpen}
                modalDialogType={modalDialogType}
                modalDialogTitle={modalDialogTitle}
                modalDialogMessage={modalDialogMessage}

                handleModalConfirm={handleModalConfirm}
                handleModalClose={handleModalClose}
            />
            {subForm.isSubForm
                ? renderSubForm()
                : (
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
                                            onChange={(date) => field.onChange(setEndOfDay(date))}
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
                            <div>
                                {mapEnrollmentsPicture.map((enrollmentsPicture) => (
                                    enrollmentsPicture && enrollmentsPicture.pictureBytes ? (
                                        <div key={enrollmentsPicture.id}
                                            style={{ display: 'flex', alignItems: 'center', marginBottom: '10px' }}>
                                            <img src={`data:image/jpeg;base64,${enrollmentsPicture.pictureBytes}`}
                                                alt={enrollmentsPicture.picturePath}
                                                style={{ width: '400px', height: 'auto', marginRight: '10px' }}
                                            />
                                            {
                                                !isReadMode && (isMode === 'Create' || currentUser.role === 'Administrator' ||
                                                    (
                                                        watch('userAddEnrollment') !== undefined &&
                                                        watch('userAddEnrollment') === currentUser.id)
                                                    ) &&
                                                        (
                                                        <button
                                                            tabIndex='-1'
                                                            type='button'
                                                            onClick={() => handleDeletePictureStage1(enrollmentsPicture.id)}
                                                        >
                                                            Usuń obraz
                                                        </button>
                                                    )
                                            }
                                        </div>
                                    ) : null
                                ))}
                            </div>

                            {isMode !== 'Detail' &&
                                <>
                                    <input
                                        ref={fileInputRef}
                                        type="file"
                                        accept=".jpg, .png"
                                        multiple
                                        onChange={addPicture}
                                        style={{ display: 'none' }}
                                    />
                                    <button
                                        tabIndex='18'
                                        type='button'
                                        onClick={() => fileInputRef.current.click()}
                                        disabled={
                                            isReadMode ||
                                            (currentUser.role !== 'Administrator' &&
                                                (
                                                    watch('userAddEnrollment') !== undefined &&
                                                    watch('userAddEnrollment') !== currentUser.id
                                                )
                                            )
                                        }
                                    >
                                            Dodaj zdjęcie
                                    </button>
                                    <br /><br />
                                </>
                            }
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
                                                <th>Operacje</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {mapEnrollmentsDescription
                                                .filter(enrollmentsDescription => enrollmentsDescription.status !== 'deleted')
                                                .map((enrollmentsDescription, index) => (
                                                <tr key={index}>
                                                    <td>{format(enrollmentsDescription.dateAddDescription, 'dd.MM.yyyy HH:mm')}</td>
                                                    <td>{enrollmentsDescription.description}</td>
                                                    <td>{enrollmentsDescription.userAddDescriptionFullName}</td>
                                                    <td>
                                                        {
                                                            !isReadMode && watch('state') !== 'Closed' &&
                                                            (currentUser.role === 'Administrator' || enrollmentsDescription.userAddDescription === currentUser.id) &&
                                                            <>
                                                                <button
                                                                    tabIndex='-1'
                                                                    type='button'
                                                                    onClick={() => {
                                                                        setSubForm({
                                                                            isSubForm: 'editDescription',
                                                                            data: {
                                                                                nr: enrollment.nr,
                                                                                year: enrollment.year,
                                                                                id: enrollmentsDescription.id,
                                                                                description: enrollmentsDescription.description,
                                                                                status: 'edited'
                                                                            }
                                                                        });
                                                                    }}
                                                                >
                                                                    Edycja
                                                                </button>
                                                                &nbsp;
                                                                <button
                                                                    type='button'
                                                                    onClick={() => handleDeleteDescriptionStage1(enrollmentsDescription.id)}
                                                                >
                                                                    Usuń
                                                                </button>
                                                            </>
                                                        }
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </>
                            }

                            {!isLoaded && isMode === 'Edit' &&
                                watch('state') === 'New' && (
                                    currentUser.role === 'Administrator' ||
                                    (
                                        watch('userAddEnrollment') !== currentUser.id &&
                                        (!watch('dateEndDeclareByDepartment') || watch('dateEndDeclareByDepartment') === '') &&
                                        (currentUser.role === 'Manager') &&
                                        currentUser.department === watch('department')
                                    )
                                ) && (
                                    <button
                                        tabIndex='19'
                                        type='button'
                                        title='Ustalenie daty zakończenia zgłoszenia'
                                        onClick={() => {
                                            setSubForm({
                                                isSubForm: 'createDescriptionSetEndDate',
                                                data: {
                                                    enrollment
                                                }
                                            });
                                        }}
                                    >
                                        Ustalenie daty zakończenia
                                    </button>
                                )
                            }

                            {!isLoaded && isMode === 'Edit' &&
                                watch('state') &&
                                watch('state') !== 'New' &&
                                watch('state') !== 'Closed' && (
                                    <button
                                        tabIndex='20'
                                        type='button'
                                        title='Dodanie adnotacji'
                                        onClick={() => {
                                            setSubForm({
                                                isSubForm: 'createDescription',
                                                data: {
                                                    nr: enrollment.nr,
                                                    year: enrollment.year,
                                                    status: 'added'
                                                }
                                            });
                                        }}
                                    >
                                        Dodaj adnotację
                                    </button>
                                )
                            }
                            &nbsp;
                            {!isLoaded &&
                                isMode === 'Edit' &&
                                watch('state') &&
                                watch('state') !== 'New' &&
                                watch('state') !== 'Closed' &&
                                watch('actionRequest') === 1 &&
                                currentUser.role === 'Administrator' && (
                                    <button
                                        tabIndex='21'
                                        type='button'
                                        title='Potwierdzenie dopuszczenia do użytku'
                                        disabled={watch('actionFinished')}
                                        onClick={() => {
                                            setSubForm({
                                                isSubForm: 'createDescription',
                                                data: {
                                                    nr: enrollment.nr,
                                                    year: enrollment.year,
                                                    status: 'added',
                                                    actionDescription: 1
                                                }
                                            });
                                        }}
                                    >
                                        Potwierdzenie dopuszczenia do użytku
                                    </button>
                                )
                            }
                        </div>
                        <div>
                            {isMode !== 'Create' && (
                                <>
                                    <br />
                                    <label>Wyk. dodatkowych czynności</label><br />
                                    <input
                                        tabIndex='22'
                                        type='checkbox'
                                        disabled={true}
                                        {...register('actionFinished')}
                                    />
                                    <br />

                                    <label>Gotowe do zamkn.</label><br />
                                    <input
                                        tabIndex='23'
                                        type='checkbox'
                                        disabled={formControls.isReadyForCloseDisabled}
                                        {...register('readyForClose')}
                                    />
                                    <br />

                                    <label>Status</label><br />
                                        <select
                                            tabIndex='24'
                                            placeholder='Wybierz status'
                                            disabled={formControls.isStateDisabled}
                                            {...register('state', { onChange: handleStateChange })}
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
                                        tabIndex='25'
                                        type='submit'
                                        disabled={isMode === 'Edit' && isLoaded}>
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
                )
            }
        </>
    );
};

export default EnrollmentsForm;