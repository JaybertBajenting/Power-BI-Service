using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Power_BI_Service.Response
{
    public class SemanticModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("webUrl")]
        public string WebUrl { get; set; }

        [JsonPropertyName("configuredBy")]
        public string ConfiguredBy { get; set; }

        [JsonPropertyName("isEffectiveIdentityRequired")]
        public bool IsEffectiveIdentityRequired { get; set; }

        [JsonPropertyName("isEffectiveIdentityRolesRequired")]
        public bool IsEffectiveIdentityRolesRequired { get; set; }

        [JsonPropertyName("isOnPremGatewayRequired")]
        public bool IsOnPremGatewayRequired { get; set; }
    }

}
