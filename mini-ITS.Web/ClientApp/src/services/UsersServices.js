import { fetchWrapper } from '../api/FetchWrapper';

export const usersServices = {
    login,
    logout
}

const baseUrl = 'api/Users';

function login(login, password) {
    return fetchWrapper.login(`${baseUrl}/Login`, login, password);
}

function logout() {
    return fetchWrapper.logout(`${baseUrl}/Logout`);
}