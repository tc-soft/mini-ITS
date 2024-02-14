import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/api/Users';

const login = (login, password) => fetchWrapper.login(`${baseUrl}/Login`, login, password);
const logout = () => fetchWrapper.logout(`${baseUrl}/Logout`);
const loginStatus = () => fetchWrapper.loginStatus(`${baseUrl}/LoginStatus`);
const changePassword = (values) => fetchWrapper.patch(`${baseUrl}/ChangePassword`, values);
const setPassword = (values) => fetchWrapper.patch(`${baseUrl}/SetPassword`, values);
const index = (values) => fetchWrapper.get(`${baseUrl}/Index`, values);
const create = (values) => fetchWrapper.post(`${baseUrl}/Create`, values);
const edit = (id) => fetchWrapper.get(`${baseUrl}/Edit/${id}`);
const update = (id, values) => fetchWrapper.put(`${baseUrl}/Edit/${id}`, values);
const _delete = (id) => fetchWrapper.delete(`${baseUrl}/Delete/${id}`);

export const usersServices = {
    login,
    logout,
    loginStatus,
    changePassword,
    setPassword,
    index,
    create,
    edit,
    update,
    delete: _delete
};