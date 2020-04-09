using System;

namespace Cosmos.DataGenerator
{
    public class ProtocolAlarm
    {
        public string id { get; set; }
        public int Code { get; set; }
        public string EventOEMCode { get; set; }
        public string ProtocolId { get; set; }
        public int Active { get; set; }
        public string Name { get; set; }
        public string Severity { get; set; }
        public string PartitionKey { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string TenantId { get; set; }
    }
}
