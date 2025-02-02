namespace mini_ITS.SchedulerService.Options
{
    public class SchedulerOptions
    {
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool ActiveOnHolidays { get; set; }
        public string Schedule { get; set; }
        public int Days { get; set; }

        public bool InfoBySMS { get; set; }
        public bool InfoByMail { get; set; }

        public bool InfoToCreateUser { get; set; }
        public bool InfoToDepartmentUsers { get; set; }
        public bool InfoToDepartmentManagers { get; set; }
        public bool InfoToAdmins { get; set; }

        public string InfoToSendByEmail1 { get; set; }
        public string InfoToSendByEmail2 { get; set; }
        public string InfoToSendByEmail3 { get; set; }

        public string InfoToSendBySMS1 { get; set; }
        public string InfoToSendBySMS2 { get; set; }
        public string InfoToSendBySMS3 { get; set; }
    }
}