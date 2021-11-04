namespace mini_ITS.Web.Models.UsersController
{
    public class ChangePassword
    {
        public string Login { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}