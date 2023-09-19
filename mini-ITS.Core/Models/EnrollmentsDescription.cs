using System;

namespace mini_ITS.Core.Models
{
    public class EnrollmentsDescription
    {
        public Guid Id { get; set; }
        public Guid EnrollmentId { get; set; }
        public DateTime DateAddDescription { get; set; }
        public DateTime DateModDescription { get; set; }

        public Guid UserAddDescription { get; set; }
        public string UserAddDescriptionFullName { get; set; }
        public Guid UserModDescription { get; set; }
        public string UserModDescriptionFullName { get; set; }

        public string Description { get; set; }

        public int ActionExecuted { get; set; }

        public EnrollmentsDescription() { }

        public EnrollmentsDescription(
            Guid id,
            Guid enrollmentId,
            DateTime dateAddDescription,
            DateTime dateModDescription,
            Guid userAddDescription,
            string userAddDescriptionFullName,
            Guid userModDescription,
            string userModDescriptionFullName,
            string description,
            int actionExecuted)
        {
            Id = id;
            EnrollmentId = enrollmentId;
            DateAddDescription = dateAddDescription;
            DateModDescription = dateModDescription;
            UserAddDescription = userAddDescription;
            UserAddDescriptionFullName = userAddDescriptionFullName;
            UserModDescription = userModDescription;
            UserModDescriptionFullName = userModDescriptionFullName;
            Description = description;
            ActionExecuted = actionExecuted;
        }
    }
}