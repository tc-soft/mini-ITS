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
            public static readonly string SetPassword = string.Concat(_usersControllerUrl, "/SetPassword");
        }
        public static class Groups
        {
            private static readonly string _groupsControllerUrl = string.Concat(_baseUrl, "Groups");

            public static readonly string Index = string.Concat(_groupsControllerUrl, "/Index");
            public static readonly string Create = string.Concat(_groupsControllerUrl, "/Create");
            public static readonly string Edit = string.Concat(_groupsControllerUrl, "/Edit");
            public static readonly string Delete = string.Concat(_groupsControllerUrl, "/Delete");
        }
        public static class Enrollments
        {
            private static readonly string _enrollmentsControllerUrl = string.Concat(_baseUrl, "Enrollments");

            public static readonly string Index = string.Concat(_enrollmentsControllerUrl, "/Index");
            public static readonly string Create = string.Concat(_enrollmentsControllerUrl, "/Create");
            public static readonly string Edit = string.Concat(_enrollmentsControllerUrl, "/Edit");
            public static readonly string Delete = string.Concat(_enrollmentsControllerUrl, "/Delete");
        }
        public static class EnrollmentsDescription
        {
            private static readonly string _enrollmentsDescriptionControllerUrl = string.Concat(_baseUrl, "EnrollmentsDescription");

            public static readonly string Index = string.Concat(_enrollmentsDescriptionControllerUrl, "/Index");
            public static readonly string Create = string.Concat(_enrollmentsDescriptionControllerUrl, "/Create");
            public static readonly string Edit = string.Concat(_enrollmentsDescriptionControllerUrl, "/Edit");
            public static readonly string Delete = string.Concat(_enrollmentsDescriptionControllerUrl, "/Delete");
        }
        public static class EnrollmentsPicture
        {
            private static readonly string _enrollmentsPictureControllerUrl = string.Concat(_baseUrl, "EnrollmentsPicture");

            public static readonly string Index = string.Concat(_enrollmentsPictureControllerUrl, "/Index");
            public static readonly string Create = string.Concat(_enrollmentsPictureControllerUrl, "/Create");
            public static readonly string Edit = string.Concat(_enrollmentsPictureControllerUrl, "/Edit");
            public static readonly string Delete = string.Concat(_enrollmentsPictureControllerUrl, "/Delete");
        }
    }
}