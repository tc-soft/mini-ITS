import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/EnrollmentsDescription';

const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);

export const enrollmentDescriptionServices = {
    index
};