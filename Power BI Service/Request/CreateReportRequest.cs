using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_BI_Service.Request
{
    public class CreateReportRequest
    {
        public string ReportName { get; set; }
        public string DatasetId { get; set; }
    }

}
