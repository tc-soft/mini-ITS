import { fetchWrapper } from '../api/FetchWrapper';

export const usersServices = {
    login
}

const baseUrl = 'api/Users';

function login(login, password) {
    return fetchWrapper.login(`${baseUrl}/Login`, login, password);
}