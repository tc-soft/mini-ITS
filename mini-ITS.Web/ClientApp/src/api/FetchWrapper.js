export const fetchWrapper = {
    login,
    logout,
    loginStatus
}

function login(url, login, password) {
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({
            login: login,
            password: password
        })
    };

    return fetch(url, requestOptions);
}

function logout(url) {
    const requestOptions = {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    };

    return fetch(url, requestOptions);
}

function loginStatus(url) {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
    };

    return fetch(url, requestOptions);
}