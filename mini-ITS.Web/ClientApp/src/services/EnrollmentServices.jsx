import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/Enrollments';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const create = (values) => fetchWrapper.post(`${baseUrl}/Create`, values);
const edit = (id) => fetchWrapper.get(`${baseUrl}/Edit/${id}`);
const update = (id, values) => fetchWrapper.put(`${baseUrl}/Edit/${id}`, values);
const getMaxNumber = (value) => fetchWrapper.get(`${baseUrl}/GetMaxNumber`, { year: value });

export const enrollmentServices = {
    index,
    create,
    edit,
    update,
    getMaxNumber
};