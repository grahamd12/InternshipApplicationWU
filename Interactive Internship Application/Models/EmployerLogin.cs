using System;
using System.Collections.Generic;

namespace Interactive_Internship_Application.Models
{
    public partial class EmployerLogin
    {
        public EmployerLogin()
        {
            StudentAppNum = new HashSet<StudentAppNum>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public short? Pin { get; set; }
        public string StudentEmail { get; set; }
        public DateTime LastLogin { get; set; }

        public virtual StudentInformation StudentEmailNavigation { get; set; }
        public virtual ICollection<StudentAppNum> StudentAppNum { get; set; }
    }
}
