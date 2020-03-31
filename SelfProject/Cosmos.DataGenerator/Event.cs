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
}
