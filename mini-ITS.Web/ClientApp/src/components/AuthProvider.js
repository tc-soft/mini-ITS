import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { usersServices } from '../services/UsersServices';

const AuthContext = createContext();

const AuthProvider = ({ children }) => {
    const [currentUser, setCurrentUser] = useState(null);
    const [loginStatus, setLoginStatus] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const checkLoginStatus = async () => {
            try {
                const response = await usersServices.loginStatus();
                if (response.ok) {
                    const data = await response.json();
                    setCurrentUser(data);
                };

                setLoginStatus(true);
            }
            catch (error) {
                console.error('Error loginStatus:', error);
            };
        };

        checkLoginStatus();
    }, []);

    const handleLogin = (user) => {
        try {
            setCurrentUser(user);
        }
        catch (error) {
            console.error('Error logging:', error);
        };
    };

    const handleLogout = async () => {
        try {
            const response = await usersServices.logout();
            if (response.ok) {
                const data = await response.json();
                setCurrentUser(null);
                navigate('/');
                console.info(data);
            }
            else {
                const data = await response.json();
                console.warn(data);
            };
        }
        catch (error) {
            console.error('Error logout:', error);
        };
    };

    return (
        <AuthContext.Provider value={{ loginStatus, currentUser, handleLogin, handleLogout, navigate }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
export default AuthProvider;