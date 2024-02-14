const ErrorMessage = ({ errors }) => {
    return (
        <>
            {errors ?
                (
                    <div className='login__errorMessage'>
                        <svg xmlns='http://www.w3.org/2000/svg'
                            height='13px' version='1.1'
                            fill='none'
                            viewBox='0 0 30 30'
                        >
                            <g>
                                <path d='M15 0C6.72 0 0 6.72 0 15C0 23.28 6.72 30 15 30C23.28 30 30 23.28 30 15C30 6.72 23.28 0 15 0ZM15 16.5C14.175 16.5 13.5 15.825 13.5 15V9C13.5 8.175 14.175 7.5 15 7.5C15.825 7.5 16.5 8.175 16.5 9V15C16.5 15.825 15.825 16.5 15 16.5ZM16.5 22.5H13.5V19.5H16.5V22.5Z'
                                    fill='#FF0000' />
                            </g>
                        </svg>

                        <p>{errors}</p>
                    </div>
                )
                :
                (
                    <div className='login__errorMessage'>
                        <p></p>
                    </div>
                )
            }
        </>
    );
}

export default ErrorMessage;