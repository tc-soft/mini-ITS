using System;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Dto
{
    public class EnrollmentsDto
    {
        private int _nr;
        private int _year;
        private string _number;

        public Guid Id { get; set; }

        public int Nr
        {
            get { return _nr; }

            set
            {
                _nr = value;
                _number = null;
            }
        }

        public int Year
        {
            get { return _year; }

            set
            {
                _year = value;
                _number = null;
            }
        }

        public string Number
        {
            get
            {
                if (_number is null)
                {
                    _number = _nr.ToString(Constants.NUMBER_FORMAT_STRING) + Constants.NUMBER_SEPARATOR + _year.ToString(Constants.NUMBER_FORMAT_STRING);
                }

                return _number;
            }
        }

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
    }
}