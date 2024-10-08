import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/EnrollmentsDescription';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const create = (values) => fetchWrapper.post(`${baseUrl}/Create`, values);
const edit = (id) => fetchWrapper.get(`${baseUrl}/Edit/${id}`);
const update = (id, values) => fetchWrapper.put(`${baseUrl}/Edit/${id}`, values);
const _delete = (id) => fetchWrapper.delete(`${baseUrl}/Delete/${id}`);

export const enrollmentDescriptionServices = {
    index,
    create,
    edit,
    update,
    delete: _delete
};