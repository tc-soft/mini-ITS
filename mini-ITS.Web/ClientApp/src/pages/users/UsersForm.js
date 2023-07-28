import React from 'react';
import { Link } from 'react-router-dom';

const UsersForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const title = { Create: 'Dodaj użytkownika', Detail: 'Szczegóły użytkownika', Edit: 'Edycja' };

    return (
        <>
            <div>
                <h3>{title[isMode]}</h3><br />
            </div>

            <div>
                <p>Użytkownik:</p><br />
            </div>

            <form>
                <div>
                    <div>
                        <label>Login</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz login"
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Imię</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz imię"
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Nazwisko</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz nazwisko"
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Dział</label><br />
                        <select
                            placeholder="Wybierz dział"
                            disabled={isReadMode}
                        >
                        </select>
                        <br />

                        <label>Rola</label><br />
                        <select
                            placeholder="Wybierz rolę"
                            disabled={isReadMode}
                        >
                        </select>
                        <br />

                        <label>Email</label><br />
                        <input
                            type="text"
                            placeholder="Wpisz email"
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Telefon</label><br />
                        <input
                            type="tel"
                            placeholder="Wpisz telefon"
                            disabled={isReadMode}
                        />
                        <br /><br />
                    </div>
                </div>
                <div>
                    <Link tabIndex="-1" to={'..'}>
                        <button>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default UsersForm;