import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/Groups';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);

export const groupsServices = {
    index
};