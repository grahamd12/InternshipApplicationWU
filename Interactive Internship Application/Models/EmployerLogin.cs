using System;
using System.Collections.Generic;

namespace Interactive_Internship_Application.Models
{
    public partial class EmployerLogin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public short? Pin { get; set; }
        public string StudentEmail { get; set; }

        public virtual StudentInformation StudentEmailNavigation { get; set; }
    }
}
