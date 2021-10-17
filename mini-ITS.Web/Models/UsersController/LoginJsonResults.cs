namespace mini_ITS.Web.Models.UsersController
{
    public class LoginJsonResults
    {
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public bool isLogged { get; set; }
    }
}