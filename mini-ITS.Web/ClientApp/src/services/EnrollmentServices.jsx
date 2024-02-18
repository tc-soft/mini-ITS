import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/Enrollments';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);

export const enrollmentServices = {
    index
};