import { Link } from 'react-router-dom';
import DatePicker from 'react-datepicker';

const EnrollmentsForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const title = { Create: 'Dodaj zgłoszenie', Detail: 'Szczegóły zgłoszenia', Edit: 'Edycja' };

    return (
        <>
            <div>
                <h2>{title[isMode]}</h2><br />
            </div>

            <div>
                <p>Dane zgłoszenia</p><br />
            </div>

            <form>
                <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    flexWrap: 'wrap'
                }}>
                    <div>
                        <label>Data zgłoszenia</label><br />
                        <DatePicker
                            tabIndex='1'
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText='Wybierz datę'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Data ost. zmiany</label><br />
                        <DatePicker
                            tabIndex='2'
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText='Wybierz datę'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Dział</label><br />
                        <select
                            tabIndex='3'
                            placeholder='Wybierz dział'
                            disabled={isReadMode}
                        >
                        </select>
                        <br />

                        <label>Opis</label><br />
                        <textarea
                            tabIndex='4'
                            placeholder='Wpisz treść'
                            rows={4}
                            cols={50}
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Priorytet</label><br />
                        <input
                            tabIndex='5'
                            type='text'
                            placeholder='Wybierz priorytet'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Gotowe do zamkn.</label><br />
                        <input
                            tabIndex='6'
                            type='checkbox'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Status</label><br />
                        <select
                            tabIndex='7'
                            placeholder='Wybierz status'
                            disabled={isReadMode}
                        >
                        </select>
                        <br />

                        <label>Dodał/a</label><br />
                        <input
                            tabIndex='8'
                            type='text'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Zakończył/a</label><br />
                        <input
                            tabIndex='9'
                            type='text'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Data zakończenia</label><br />
                        <DatePicker
                            tabIndex='10'
                            dateFormat='dd.MM.yyyy HH:mm'
                            placeholderText='brak'
                            disabled={isReadMode}
                        />

                        <label>Otworzył/a ponownie</label><br />
                        <input
                            tabIndex='11'
                            type='text'
                            disabled={isReadMode}
                        />
                    </div>
                    <div>
                        <label>Data zak. w/g zgł.</label><br />
                        <input
                            tabIndex='12'
                            type='text'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Data zak. w/g działu</label><br />
                        <input
                            tabIndex='13'
                            type='text'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Grupa/Linia</label><br />
                        <select
                            tabIndex='14'
                            placeholder='Wybierz grupę'
                            disabled={isReadMode}
                        >
                        </select>
                        <br /><br />

                        <label>Żądanie dodatkowych czynności</label><br />
                        <input
                            tabIndex='15'
                            type='checkbox'
                            disabled={isReadMode}
                        />
                        <br />

                        <label>Wyk. dodatkowych czynności</label><br />
                        <input
                            tabIndex='16'
                            type='checkbox'
                            disabled={isReadMode}
                        />
                        
                    </div>
                </div>
                <br />
                <div>
                    Picture
                </div>
                <div>
                    <table>
                        <thead>
                            <tr>
                                <th>Data wpr.</th>
                                <th>Adnotacja</th>
                                <th>Dodał</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div>
                    <br />
                    <Link tabIndex='-1' to={'..'}>
                        <button>
                            Anuluj
                        </button>
                    </Link>
                </div>
            </form>
        </>
    );
};

export default EnrollmentsForm;