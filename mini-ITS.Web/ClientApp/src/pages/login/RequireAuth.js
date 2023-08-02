import React from 'react';
import { useLocation, Navigate } from 'react-router-dom';
import { useAuth } from '../../components/AuthProvider';

const RequireAuth = ({ children }) => {
    const { currentUser, loginStatus } = useAuth();
    let location = useLocation();

    if (!loginStatus) {
        return (
            <p>Czekamy...</p>
        );
    };

    return (
        currentUser
            ? (children)
            : (<Navigate to='/Login' state={{ from: location }} />)
    );
};

export default RequireAuth;