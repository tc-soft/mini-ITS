import { Routes, Route, Link } from 'react-router-dom';
import { useAuth } from './components/AuthProvider';
import RequireAuth from './pages/login/RequireAuth';
import Dashboard from './pages/dashboard/Dashboard';
import Login from './pages/login/Login';
import Enrollments from './pages/enrollments/Enrollments';
import Groups from './pages/groups/Groups';
import Users from './pages/users/Users';
import brandIcon from './images/mini-ITS.svg';
import logOut from './images/LogOut.svg';

import './styles/main.scss';

const App = () => {
    const { currentUser, handleLogout } = useAuth();

    return (
        <main className='main'>
            <header className='main-header'>
                <nav>
                    <img src={brandIcon} alt="brand Icon" />
                    <ul>
                        <li className='main-header__link'>
                            {currentUser && <Link to="/">Home</Link>}
                        </li>
                        <li className='main-header__link'>
                            {currentUser && <Link to="/Enrollments">Zgłoszenia</Link>}
                        </li>
                        <li className='main-header__link'>
                            {(currentUser && (currentUser.role === 'Administrator' || currentUser.role === 'Manager')) && <Link to='/Groups'>Grupy</Link>}
                        </li>
                        <li className='main-header__link'>
                            {currentUser && (currentUser.role === 'Administrator') && <Link to='/Users'>Użytkownicy</Link>}
                        </li>
                        <li className='main-header__icon'>
                            {currentUser && <Link to='/' onClick={handleLogout}>
                                <img src={logOut} alt="Log out" />
                            </Link>}
                        </li>
                    </ul>
                </nav>
            </header>

            <section className='main-section'>
                <Routes>
                    <Route path='/' element={
                        <RequireAuth>
                            <Dashboard />
                        </RequireAuth>}
                    />
                    <Route path='/Enrollments/*' element={
                        <RequireAuth>
                            <Enrollments />
                        </RequireAuth>}
                    />
                    <Route path='/Groups/*' element={
                        <RequireAuth>
                            <Groups />
                        </RequireAuth>}
                    />
                    <Route path='/Users/*' element={
                        <RequireAuth>
                            <Users />
                        </RequireAuth>}
                    />
                    <Route path='/Login' element={<Login />} />
                </Routes>
            </section>

            <footer className='main-footer'>
                <p>©2025 mini-ITS v{__APP_VERSION__}</p>
            </footer>
        </main>
    );
};

export default App;