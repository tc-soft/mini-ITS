using System;

namespace mini_ITS.Core.Models
{
    public class Enrollments
    {
        public Guid Id { get; set; }
        public int Nr { get; set; }
        public int Year { get; set; }

        public DateTime? DateAddEnrollment { get; set; }
        public DateTime? DateEndEnrollment { get; set; }
        public DateTime? DateLastChange { get; set; }
        public DateTime? DateEndDeclareByUser { get; set; }
        public DateTime? DateEndDeclareByDepartment { get; set; }
        public Guid DateEndDeclareByDepartmentUser { get; set; }
        public string DateEndDeclareByDepartmentUserFullName { get; set; }

        public string Department { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }

        public PriorityValues Priority { get; set; }
        public bool SMSToUserInfo { get; set; }
        public bool SMSToAllInfo { get; set; }
        public bool MailToUserInfo { get; set; }
        public bool MailToAllInfo { get; set; }

        public int ActionRequest { get; set; }
        public int ActionExecuted { get; set; }
        public bool ActionFinished { get; set; }

        public bool ReadyForClose { get; set; }
        public string State { get; set; }

        public Guid UserAddEnrollment { get; set; }
        public string UserAddEnrollmentFullName { get; set; }
        public Guid UserEndEnrollment { get; set; }
        public string UserEndEnrollmentFullName { get; set; }
        public Guid UserReeEnrollment { get; set; }
        public string UserReeEnrollmentFullName { get; set; }

        public Enrollments() { }

        public Enrollments(
            Guid id,
            int nr,
            int year,
            DateTime? dateAddEnrollment,
            DateTime? dateEndEnrollment,
            DateTime? dateLastChange,
            DateTime? dateEndDeclareByUser,
            DateTime? dateEndDeclareByDepartment,
            Guid dateEndDeclareByDepartmentUser,
            string dateEndDeclareByDepartmentUserFullName,
            string department,
            string description,
            string group,
            PriorityValues priority,
            bool sMSToUserInfo,
            bool sMSToAllInfo,
            bool mailToUserInfo,
            bool mailToAllInfo,
            int actionRequest,
            int actionExecuted,
            bool actionFinished,
            bool readyForClose,
            string state,
            Guid userAddEnrollment,
            string userAddEnrollmentFullName,
            Guid userEndEnrollment,
            string userEndEnrollmentFullName,
            Guid userReeEnrollment,
            string userReeEnrollmentFullName)
        {
            Id = id;
            Nr = nr;
            Year = year;
            DateAddEnrollment = dateAddEnrollment;
            DateEndEnrollment = dateEndEnrollment;
            DateLastChange = dateLastChange;
            DateEndDeclareByUser = dateEndDeclareByUser;
            DateEndDeclareByDepartment = dateEndDeclareByDepartment;
            DateEndDeclareByDepartmentUser = dateEndDeclareByDepartmentUser;
            DateEndDeclareByDepartmentUserFullName = dateEndDeclareByDepartmentUserFullName;
            Department = department;
            Description = description;
            Group = group;
            Priority = priority;
            SMSToUserInfo = sMSToUserInfo;
            SMSToAllInfo = sMSToAllInfo;
            MailToUserInfo = mailToUserInfo;
            MailToAllInfo = mailToAllInfo;
            ActionRequest = actionRequest;
            ActionExecuted = actionExecuted;
            ActionFinished = actionFinished;
            ReadyForClose = readyForClose;
            State = state;
            UserAddEnrollment = userAddEnrollment;
            UserAddEnrollmentFullName = userAddEnrollmentFullName;
            UserEndEnrollment = userEndEnrollment;
            UserEndEnrollmentFullName = userEndEnrollmentFullName;
            UserReeEnrollment = userReeEnrollment;
            UserReeEnrollmentFullName = userReeEnrollmentFullName;
        }
    }
}