import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/EnrollmentsDescription';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const create = (values) => fetchWrapper.post(`${baseUrl}/Create`, values);

export const enrollmentDescriptionServices = {
    index,
    create
};