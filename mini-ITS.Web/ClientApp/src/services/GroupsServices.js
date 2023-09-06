import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/Groups';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const create = (values) => fetchWrapper.post(`${baseUrl}/Create`, values);
const edit = (id) => fetchWrapper.get(`${baseUrl}/Edit/${id}`);
const update = (id, values) => fetchWrapper.put(`${baseUrl}/Edit/${id}`, values);

export const groupsServices = {
    index,
    create,
    edit,
    update
};