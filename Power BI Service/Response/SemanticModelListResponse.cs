using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Power_BI_Service.Response
{
    public class SemanticModelListResponse
    {
        [JsonPropertyName("value")]
        public List<SemanticModel> Value { get; set; }
    }

}
