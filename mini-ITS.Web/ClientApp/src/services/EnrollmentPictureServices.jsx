import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/EnrollmentsPicture';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const edit = (id) => fetchWrapper.get(`${baseUrl}/Edit/${id}`);

export const enrollmentPictureServices = {
    index,
    edit
};