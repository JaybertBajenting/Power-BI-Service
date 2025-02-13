using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_BI_Service.Request
{
    public class UpdateParameterRequest
    {
        public string ParameterName { get; set; }
        public string NewValue { get; set; }
    }
}


