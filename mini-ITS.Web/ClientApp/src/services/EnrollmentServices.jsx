import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/Enrollments';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const edit = (id) => fetchWrapper.get(`${baseUrl}/Edit/${id}`);

export const enrollmentServices = {
    index,
    edit
};