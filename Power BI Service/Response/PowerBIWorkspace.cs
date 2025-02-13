using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Power_BI_Service.Response
{
    public class PowerBIWorkspace
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonPropertyName("isReadOnly")]
        public bool IsReadOnly { get; set; }

        [JsonPropertyName("isOnDedicatedCapacity")]
        public bool IsOnDedicatedCapacity { get; set; }

        [JsonPropertyName("capacityId")]
        public string CapacityId { get; set; }

        [JsonPropertyName("workspaceState")]
        public string WorkspaceState { get; set; }

        [JsonPropertyName("dataflowStorageId")]
        public string DataflowStorageId { get; set; }

        [JsonPropertyName("users")]
        public List<PowerBIUser> Users { get; set; }
    }

    


    public class PowerBIUser
    {
        public string Identifier { get; set; }

        [JsonPropertyName("groupUserAccessRight")]
        public string GroupUserAccessRight { get; set; }

        public string PrincipalType { get; set; }
    }

}
