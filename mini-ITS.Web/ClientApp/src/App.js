import React from 'react';
import { Routes, Route, Link } from 'react-router-dom';
import { useAuth } from './components/AuthProvider';
import Login from './pages/login/Login';
import RequireAuth from './pages/login/RequireAuth';
import { ReactComponent as BrandIcon } from "./images/mini-ITS.svg";
import { ReactComponent as LogOut } from "./images/LogOut.svg";

import './styles/main.scss';

const App = () => {
    const { currentUser, handleLogout } = useAuth();

    return (
        <main className="main">
            <header className="main-header">
                <nav>
                    <BrandIcon />
                    <ul>
                        <li className="main-header__link">{currentUser && <Link to="/">Home</Link>}</li>
                        <li className="main-header__link">{currentUser && <Link to="/Test">Użytkownicy</Link>}</li>
                        <li className="main-header__icon">{currentUser && <Link to='/' onClick={() => { handleLogout() }}><LogOut /></Link>}</li>
                    </ul>
                </nav>
            </header>

            <section className="main-section">
                <Routes>
                    <Route path="/" element={
                        <RequireAuth>
                            <p>Strona główna</p>
                        </RequireAuth>}
                    />
                    <Route path="/Login" element={<Login />} />
                </Routes>
            </section>

            <footer className="main-footer">
                <p>©2023 mini-ITS</p>
            </footer>
        </main>
    );
};

export default App;