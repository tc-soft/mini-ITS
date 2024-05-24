import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/EnrollmentsPicture';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const create = (values) => fetchWrapper.post(`${baseUrl}/Create`, values, true);
const edit = (id) => fetchWrapper.get(`${baseUrl}/Edit/${id}`);
const _delete = (id) => fetchWrapper.delete(`${baseUrl}/Delete/${id}`);

export const enrollmentPictureServices = {
    index,
    create,
    edit,
    delete: _delete
};