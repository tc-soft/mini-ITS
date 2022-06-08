import React from 'react';
import { Routes, Route, Link } from 'react-router-dom';
import { useAuth } from './components/AuthProvider';
import { ReactComponent as BrandIcon } from "./images/mini-ITS.svg";
import { ReactComponent as LogOut } from "./images/LogOut.svg";

import './styles/main.scss';

function App() {
    const { currentUser, handleLogout } = useAuth();

    return (
        <main className="main">
            <header className="main__header">
                <nav>
                    <BrandIcon />

                    <ul>
                        <li className="main__header--link">{<Link to="/">Home</Link>}</li>
                        <li className="main__header--icon">{currentUser && <Link to='/' onClick={() => { handleLogout() }}><LogOut /></Link>}</li>
                    </ul>

                </nav>
            </header>

            <section className="main__section">
                <Routes>
                    <Route path="/" element={<p>Strona główna</p>}/>
                </Routes>
            </section>

            <footer className="main__footer">
                <p>©2022 mini-ITS</p>
            </footer>
        </main>
    );
}

export default App;