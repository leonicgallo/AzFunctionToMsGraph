using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMeetGenHttp.Models
{
    public class Meeting
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Subject { get; set; }
        public string PatientEmail { get; set; }
        public string DoctorEmail { get; set; }
        public string TenantId { get; set; }
    }
}
