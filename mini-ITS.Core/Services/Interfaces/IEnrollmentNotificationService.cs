using System.Threading.Tasks;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Services
{
    public interface IEnrollmentNotificationService
    {
        Task EnrollmentEvent1(Enrollments enrollment);
        Task EnrollmentEvent2(Enrollments oldEnrollment, Enrollments newEnrollment);
    }
}