using System;

namespace mini_ITS.Core.Dto
{
    public class UsersDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string PasswordHash { get; set; }        
    }
}