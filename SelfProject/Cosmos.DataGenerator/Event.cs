using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.DataGenerator
{
    public class EventWithAlarmId
    {
        public int AlarmId { get; set; }
        public string EventId { get; set; }
    }

    public class EventDetail
    {
        public Guid id { get; set; }
        public int AlarmId { get; set; }
        public string DocumentType { get; set; }
        public string EventId { get; set; }
        public DateTime EventStartTime { get; set; }
        public DateTime? EventEndTime { get; set; }
    }

    public class Event
    {
        public string id { get; set; }
        public string DocumentType { get; set; }
        public string SiteId { get; set; }
        public string AssetId { get; set; }
        public string ProtocolAlarmId { get; set; }
        public string EventOEMCode { get; set; }
        public Site Site { get; set; }
        public Asset Asset { get; set; }
        public AssetModel AssetModel { get; set; }
        public AssetType AssetType { get; set; }
        public ProtocolAlarm ProtocolAlarm { get; set; }
        public string ProtocolId { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public string PartitionKey { get; set; }
        public List<EventDetail> EventDetails { get; set; }
    }

}
