using System;
using System.Collections.Generic;

namespace Interactive_Internship_Application.Models
{
    public partial class StudentInformation
    {
        public StudentInformation()
        {
            EmployerLogin = new HashSet<EmployerLogin>();
            StudentAppNum = new HashSet<StudentAppNum>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? LastLogin { get; set; }

        public virtual ICollection<EmployerLogin> EmployerLogin { get; set; }
        public virtual ICollection<StudentAppNum> StudentAppNum { get; set; }
    }
}
