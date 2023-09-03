import React from 'react';
import { Link } from 'react-router-dom';

const GroupsForm = (props) => {
    const { isMode } = props;
    const isReadMode = isMode === 'Detail' ? true : false;
    const title = { Create: 'Dodaj grupę', Detail: 'Szczegóły grupy', Edit: 'Edycja' };

    return (
        <>
            <div>
                <h3>{title[isMode]}</h3><br />
            </div>

            <div>
                <p>Grupa:</p><br />
            </div>

            <form>
                <div>
                    <div>
                        <label>Nazwa grupy</label><br />
                        <input
                            type='text'
                            placeholder='Wpisz nazwę grupy'
                            disabled={isReadMode}
                        />
                        <br /><br />
                    </div>
                </div>
                <div>
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

export default GroupsForm;