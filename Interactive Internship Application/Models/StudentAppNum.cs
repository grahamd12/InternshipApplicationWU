using System;
using System.Collections.Generic;

namespace Interactive_Internship_Application.Models
{
    public partial class StudentAppNum
    {
        public StudentAppNum()
        {
            ApplicationData = new HashSet<ApplicationData>();
        }

        public int Id { get; set; }
        public string StudentEmail { get; set; }
        public int EmployerId { get; set; }

        public virtual EmployerLogin Employer { get; set; }
        public virtual StudentInformation StudentEmailNavigation { get; set; }
        public virtual ICollection<ApplicationData> ApplicationData { get; set; }
    }
}
