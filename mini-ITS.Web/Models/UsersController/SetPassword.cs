using System;

namespace mini_ITS.Web.Models.UsersController
{
    public class SetPassword
    {
        public Guid Id { get; set; }
        public string NewPassword { get; set; }
    }
}