using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models.Responses
{
    public class ChangeUserReportStatusViewModel
    {
        public string ReportId { get; set; }
        public ReportStatus ReportStatus { get; set; }
    }
}
