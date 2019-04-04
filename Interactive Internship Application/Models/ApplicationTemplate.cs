using System;
using System.Collections.Generic;

namespace Interactive_Internship_Application.Models
{
    public partial class ApplicationTemplate
    {
        public ApplicationTemplate()
        {
            ApplicationData = new HashSet<ApplicationData>();
        }

        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldDescription { get; set; }
        public string Entity { get; set; }
        public string ControlType { get; set; }
        public string ProperName { get; set; }
        public bool? Deleted { get; set; }
        public bool? RequiredField { get; set; }

        public virtual ICollection<ApplicationData> ApplicationData { get; set; }
    }
}
