using System;

namespace mini_ITS.Core.Models
{
    public class Users
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

        public Users() { }

        public Users(Guid id, string login, string firstName, string lastName, string department, string email, string phone, string role, string passwordHash)
        {
            Id = id;
            Login = login;
            FirstName = firstName;
            LastName = lastName;
            Department = department;
            Email = email;
            Phone = phone;
            Role = role;
            PasswordHash = passwordHash;
        }
    }
}