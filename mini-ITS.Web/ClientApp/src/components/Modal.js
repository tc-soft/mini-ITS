import React from 'react';
import Modal from 'react-modal';

export default function ModalDialog(props) {
    const {
        modalDialogOpen,
        modalDialogType,
        modalDialogTitle,
        modalDialogMessage,
        handleModalConfirm,
        handleModalClose
    } = props;

    const modalDialogStyle = {
        content: {
            top: '10%',
            left: '50%',
            right: 'auto',
            bottom: 'auto',
            marginRight: '-50%',
            transform: 'translate(-50%, -50%)',
            backgroundColor: (() => {
                switch (modalDialogType) {
                    case 'Dialog': return '#F5F5FF';
                    case 'Information': return '#F5FFF5';
                    case 'Error': return '#FFF5F5';
                    default: return null;
                }
            })()
        }
    };

    const buttonComponent = modalDialogType === 'Dialog' ? (
        <div className='modalDialog'>
            <button className='modalDialog__button modalDialog__button--yes' onClick={handleModalConfirm}>Tak</button>
            <button className='modalDialog__button modalDialog__button--no' onClick={handleModalClose}>Nie</button>
        </div>
    ) : (
        <div>
            <button className='modalDialog__button modalDialog__button--ok' onClick={handleModalConfirm}>Ok</button>
        </div>
    );

    return (
        <Modal isOpen={modalDialogOpen} style={modalDialogStyle} ariaHideApp={false}>
            <h2>{modalDialogTitle}</h2>
            <p>{modalDialogMessage}</p>
            <br />
            {buttonComponent}
        </Modal>
    );
};