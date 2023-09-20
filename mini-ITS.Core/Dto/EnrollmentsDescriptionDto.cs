using System;

namespace mini_ITS.Core.Dto
{
    public class EnrollmentsDescriptionDto
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
    }
}