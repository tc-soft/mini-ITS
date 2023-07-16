const login = (url, login, password) => {
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
};

const logout = (url) => {
    const requestOptions = {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    };

    return fetch(url, requestOptions);
};

const loginStatus = (url) => {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    };

    return fetch(url, requestOptions);
};

const get = (url, params) => {
    const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    };

    return fetch(`${url}${params ? encodeQueryString(params) : ''}`, requestOptions);
};

const post = (url, body) => {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };

    return fetch(url, requestOptions);
};

const put = (url, body) => {
    const requestOptions = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };

    return fetch(url, requestOptions);
};

const patch = (url, body) => {
    const requestOptions = {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };

    return fetch(url, requestOptions);
};

const _delete = (url) => {
    const requestOptions = {
        method: 'DELETE'
    };

    return fetch(url, requestOptions);
};

const encodeQueryString = (params) => {
    const paramsKeys = Object.keys(params);
    var results = "";

    if (paramsKeys.length) {
        results = "?" + paramsKeys
            .map(paramKey => {
                if (typeof params[paramKey] === 'object' && params[paramKey] !== null) {
                    return params[paramKey]
                        .map((key, index) => {
                            return `${paramKey}[${index}].name=${key.name}&` +
                                `${paramKey}[${index}].operator=${key.operator}&` +
                                `${paramKey}[${index}].value=${encodeURIComponent(key.value)}`
                        })
                        .join('&');
                }
                else {
                    return `${paramKey}=${params[paramKey]}`;
                };
            })
            .join('&');
    };

    return results;
};

export const fetchWrapper = {
    login,
    logout,
    loginStatus,
    get,
    post,
    put,
    patch,
    delete: _delete
};