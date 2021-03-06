namespace mini_ITS.Web.Tests
{
    public static class ApiRoutes
    {
        private static readonly string _baseUrl = "https://localhost:44375/api/";

        public static class Users
        {
            private static readonly string _usersControllerUrl = string.Concat(_baseUrl, "Users");

            public static readonly string Login = string.Concat(_usersControllerUrl, "/Login");
            public static readonly string Logout = string.Concat(_usersControllerUrl, "/Logout");
            public static readonly string LoginStatus = string.Concat(_usersControllerUrl, "/LoginStatus");
            public static readonly string Index = string.Concat(_usersControllerUrl, "/Index");
            public static readonly string Create = string.Concat(_usersControllerUrl, "/Create");
            public static readonly string Edit = string.Concat(_usersControllerUrl, "/Edit");
            public static readonly string Delete = string.Concat(_usersControllerUrl, "/Delete");
            public static readonly string ChangePassword = string.Concat(_usersControllerUrl, "/ChangePassword");
        }
    }
}