import { fetchWrapper } from '../api/FetchWrapper';

export const usersServices = {
    login,
    logout,
    loginStatus,
    changePassword,
    index
}

const baseUrl = 'api/Users';

function login(login, password) {
    return fetchWrapper.login(`${baseUrl}/Login`, login, password);
}

function logout() {
    return fetchWrapper.logout(`${baseUrl}/Logout`);
}

function loginStatus() {
    return fetchWrapper.loginStatus(`${baseUrl}/LoginStatus`);
}

function changePassword(id, values) {
    return fetchWrapper.patch(`${baseUrl}/ChangePassword/${id}`, values);
}

function index(values) {
    return fetchWrapper.get(`${baseUrl}/Index`, values);
}