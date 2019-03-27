using System;
using System.Collections.Generic;

namespace Interactive_Internship_Application.Models
{
    public partial class ApplicationData
    {
        public int RecordId { get; set; }
        public int DataKeyId { get; set; }
        public string Value { get; set; }

        public virtual ApplicationTemplate DataKey { get; set; }
        public virtual StudentAppNum Record { get; set; }
    }
}
