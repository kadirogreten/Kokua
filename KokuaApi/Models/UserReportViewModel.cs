using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models
{
    public class UserReportViewModel
    {
        public string ReportToUsername { get; set; }
        public string Detail { get; set; }
        public ReportType ReportType { get; set; }
        public ReportSubject ReportSubject { get; set; }
        public ReportStatus ReportStatus { get; set; } = 0;
    }
}
