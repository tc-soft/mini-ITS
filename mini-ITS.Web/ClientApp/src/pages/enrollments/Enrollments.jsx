import { Routes, Route } from 'react-router-dom';
import EnrollmentsList from './EnrollmentsList';

const Enrollments = () => {

    return (
        <Routes>
            <Route path="/" element={<EnrollmentsList />} />
        </Routes>
    );
};

export default Enrollments;