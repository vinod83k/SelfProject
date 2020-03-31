namespace Cosmos.DataGenerator
{
    public class ProtocolAlarm
    {
        public string id { get; set; }
        public int AlarmId { get; set; }
        public string EventOEMCode { get; set; }
        public string ProtocolId { get; set; }
        public int Active { get; set; }
        public string Name { get; set; }
        public string Severity { get; set; }
        public string PartitionKey { get; set; }
    }
}
