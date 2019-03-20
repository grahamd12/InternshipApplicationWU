using System;
using System.Collections.Generic;

namespace Interactive_Internship_Application.Models
{
    public partial class StudentInformation
    {
        public StudentInformation()
        {
            ApplicationData = new HashSet<ApplicationData>();
            EmployerLogin = new HashSet<EmployerLogin>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? LastLogin { get; set; }

        public virtual ICollection<ApplicationData> ApplicationData { get; set; }
        public virtual ICollection<EmployerLogin> EmployerLogin { get; set; }
    }
}
