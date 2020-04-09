using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.DataGenerator
{
    public class Protocol
    {
        public string id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string TenantId { get; set; }
        public string PartitionKey { get; set; }
    }
}
