const login = async (url, login, password) => {
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({
            login: login,
            password: password
        }),
        signal: new AbortController().signal
    };

    return await fetch(url, requestOptions);
};

const logout = async (url) => {
    const requestOptions = {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        signal: new AbortController().signal
    };

    return await fetch(url, requestOptions);
};

const loginStatus = async (url) => {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        signal: new AbortController().signal
    };

    return await fetch(url, requestOptions);
};

const get = async (url, params) => {
    const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' },
        signal: new AbortController().signal
    };

    return await fetch(`${url}${params ? encodeQueryString(params) : ''}`, requestOptions);
};

const post = async (url, body, isFormData = false) => {
    const requestOptions = {
        method: 'POST',
        headers: isFormData ? {} : { 'Content-Type': 'application/json' },
        body: isFormData ? body : JSON.stringify(body),
        signal: new AbortController().signal
    };

    if (isFormData) {
        delete requestOptions.headers['Content-Type'];
    };

    return await fetch(url, requestOptions);
};

const put = async (url, body) => {
    const requestOptions = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
        signal: new AbortController().signal
    };

    return await fetch(url, requestOptions);
};

const patch = async (url, body) => {
    const requestOptions = {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
        signal: new AbortController().signal
    };

    return await fetch(url, requestOptions);
};

const _delete = async (url) => {
    const requestOptions = {
        method: 'DELETE',
        signal: new AbortController().signal
    };

    return await fetch(url, requestOptions);
};

const encodeQueryString = (params) => {
    const paramsKeys = Object.keys(params);
    var results = '';

    if (paramsKeys.length) {
        results = '?' + paramsKeys
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