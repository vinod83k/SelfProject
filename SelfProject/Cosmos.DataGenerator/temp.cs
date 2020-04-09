using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.DataGenerator
{
    class Program
    {
        public static string DatabaseName = "Wenco-CosmoData"; // "wenco-cosmoDB";
        public static string _siteId = "7509c107-ce13-498e-9ae4-755549491604".ToLower();
        public static string FilePath = @"F:\Vinod\Projects\SelfProject\SelfProject\SelfProject\Cosmos.DataGenerator\csv\";
        private static string tenantId = "1a64190c-0859-4b80-8ace-1ee7ed6be4af";
        private static string _partitionKey = $"{tenantId}-{_siteId}";
        private static Database database;

        public static List<string> Ids => new List<string> {
            "Site", "ProtocolAlarm", "ProtocolSensor", "AssetModel", "AssetType", "Asset", "Asset", "Event", "Protocol" };

        public static List<string> CsvFileNames => new List<string> {
            "SITE","ALARM_PROTOCOL","SENSOR_PROTOCOL","EQUIP_MODEL","EQUIP_TYPE","EQUIP_HEALTH_PROTOCOL","EQUIP","EVENT","HEALTH_PROTOCOL_TYPE" };

        static List<ProtocolAlarm> protocolAlarms = new List<ProtocolAlarm>();
        static List<Protocol> protocols = new List<Protocol>();
        static List<Site> _sites;
        static List<Asset> _assets;
        static List<AssetType> _assetTypes;

        static void Main(string[] args)
        {
            CosmosClient client = new CosmosClient(EndpointUrl, PrimaryKey);

            database = client.GetDatabase(DatabaseName);

            Site.PopulateCollection(database);
            _sites = Site.GetCollection();
            tenantId = _sites[0].TenantId.ToString();

            Asset.PopulateCollection(database);
            _assets = Asset.GetCollection();

            AssetType.PopulateCollection(database);
            _assetTypes = AssetType.GetCollection();

            Container container = database.CreateContainerIfNotExistsAsync(
                     "Protocol_Test",
                     "/PartitionKey",
                     400).Result;

            container = database.CreateContainerIfNotExistsAsync(
                     "ProtocolAlarm_Test",
                     "/PartitionKey",
                     400).Result;

            container = database.CreateContainerIfNotExistsAsync(
                     "Event_Test",
                     "/PartitionKey",
                     400).Result;

            //CreateDatabase().Wait();
            UpsertCollectionData().Wait();
        }

        public static async Task CreateDatabase()
        {
            CosmosClient client = new CosmosClient(EndpointUrl, PrimaryKey);

            database = await client.CreateDatabaseIfNotExistsAsync(DatabaseName);

            for (int i = 0; i < WencoContainer.Ids.Count; i++)
            {
                Container container = await database.CreateContainerIfNotExistsAsync(
                    WencoContainer.Ids[i],
                    "/PartitionKey",
                    400);
            }

            await UpsertCollectionData();
        }

        private static async Task UpsertCollectionData()
        {
            await CreateProtocolTypes();

            await CreateAlarmProtocolItems();

            await CreateEventItems();

            //await CreateHealthProtocolsItems();
        }


        private static string GetSeverity(string severityCode)
        {
            string severity;
            switch (severityCode)
            {
                case "C": severity = "Critical"; break;
                case "W": severity = "Warning"; break;
                case "A": severity = "Alarm"; break;
                default: severity = "Information"; break;
            }

            return severity;
        }

        private static async Task CreateProtocolTypes()
        {
            Container container = database.GetContainer(WencoContainer.Ids[8]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[8]}.csv";

            var csvData = File.ReadAllLines(fileName);

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                var item = new Protocol
                {
                    id = Convert.ToString(values[0]),
                    Type = Convert.ToString(values[1]),
                    Description = Convert.ToString(values[3]),
                    Version = Convert.ToString(values[2]),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = string.Empty,
                    TenantId = tenantId,
                    PartitionKey = tenantId
                };

                await CreateReadyLineItem(container, item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateAlarmProtocolItems()
        {
            Container container = database.GetContainer("ProtocolAlarm_Test");
            if (container == null) return;

            string fileName = $"{FilePath}ALARM_PROTOCOL.csv";

            var csvData = File.ReadAllLines(fileName);

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                var item = new ProtocolAlarm
                {
                    id = Guid.NewGuid().ToString().ToLower(),
                    Code = Convert.ToInt32(values[2]),
                    EventOEMCode = Convert.ToString(values[1]),
                    ProtocolId = Convert.ToString(values[0]),
                    Active = Convert.ToString(values[6]) == "Y" ? 1 : 0,
                    Name = Convert.ToString(values[7]),
                    Severity = GetSeverity(Convert.ToString(values[5])),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = string.Empty,
                    TenantId = tenantId,
                    PartitionKey = tenantId
                };

                await CreateReadyLineItem(container, item);
                protocolAlarms.Add(item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateEventItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[7]);
            if (container == null) return;

            container.ReadContainerAsync().Result.Resource.IndexingPolicy.CompositeIndexes.Add(
                new Collection<CompositePath> {
                    new CompositePath() { Path = "/CeatedOn", Order = CompositePathSortOrder.Ascending },
                    new CompositePath() { Path = "/Type", Order = CompositePathSortOrder.Ascending },
                    new CompositePath() { Path = "/Status", Order = CompositePathSortOrder.Ascending }
                });

            string fileName = $"{FilePath}Event.csv";

            var csvData = File.ReadAllLines(fileName);

            var insertedProtocolAlarms = new List<EventWithAlarmId>();
            var eventDetails = new List<EventDetail>();
            var events = new List<Event>();

            var rand = new Random();

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');
                int assetIndex = rand.Next(0, 10);

                var protocolAlarm = protocolAlarms.FirstOrDefault(x => x.Code == Convert.ToInt32(values[5]));

                if (insertedProtocolAlarms.Exists(x => x.AlarmId == protocolAlarm.Code)) {
                    var eventSummaryObj = insertedProtocolAlarms.FirstOrDefault(x => x.AlarmId == protocolAlarm.Code);
                    if (Convert.ToInt32(values[6]) == 0) { // in-active event
                        eventDetails.Where(x => x.AlarmId == protocolAlarm.Code).OrderByDescending(x => x.EventStartTime).Take(1).FirstOrDefault().EventEndTime = Convert.ToDateTime(values[4]);
                    }
                    else // active event
                    {
                        eventDetails.Add(new EventDetail
                        {
                            id = Guid.NewGuid(),
                            EventStartTime = Convert.ToDateTime(values[4])
                        });
                    }
                }
                else
                {
                    var asset = _assets.ElementAt(assetIndex);
                    var site = _sites.ElementAt(0);
                    var eventid = Guid.NewGuid().ToString().ToLower();

                    // Create an Event item
                    events.Add(new Event
                    {
                        id = eventid,
                        DocumentType = "Event",
                        AssetId = asset.id,
                        Site = new Site
                        {
                            id = site.id,
                            Name = site.Name,
                            OrganizationCode = site.OrganizationCode,
                            TenantId = site.TenantId
                        },
                        Asset = new Asset
                        {
                            id = asset.id,
                            Name = asset.Name,
                            Description = asset.Description,
                            AssetModelId = asset.AssetModelId,
                            AssetTypeId = asset.AssetTypeId,
                            SiteId = site.id
                        },
                        AssetModel = new AssetModel
                        {
                            id = asset.AssetModelId,
                            Code = asset.AssetModelId,
                            Name = asset.AssetModelId
                        },
                        AssetType = new AssetType
                        {
                            id = _assetTypes.FirstOrDefault(x => x.id == asset.AssetTypeId).id,
                            Value = _assetTypes.FirstOrDefault(x => x.id == asset.AssetTypeId).Value,
                            Label = _assetTypes.FirstOrDefault(x => x.id == asset.AssetTypeId).Label
                        },
                        ProtocolAlarm = new ProtocolAlarm
                        {
                            id = protocolAlarm.id.ToLower(),
                            Code = protocolAlarm.Code,
                            Name = protocolAlarm.Name,
                            Severity = protocolAlarm.Severity,
                            EventOEMCode = protocolAlarm.EventOEMCode,
                            ProtocolId = protocolAlarm.ProtocolId,
                            Active = protocolAlarm.Active
                        },
                        ProtocolId = protocolAlarm.ProtocolId,
                        ProtocolType = proto
                        EventOEMCode = protocolAlarm.EventOEMCode,
                        Type = ((Convert.ToInt32(values[5]) % 10000) < 10) ? "BOUNDRY" :
                            ((Convert.ToInt32(values[5]) % 10000) > 10 && (Convert.ToInt32(values[5]) % 10000) < 20) ? "VIRTUAL" :
                              ((Convert.ToInt32(values[5]) % 10000) > 20 && (Convert.ToInt32(values[5]) % 10000) < 40) ? "SAFETY" :
                              "OEM",
                        CreatedOn = Convert.ToDateTime(values[4]),
                        Active = Convert.ToInt32(values[6]) == 1 ? true : false,
                        Status = "UN-ACK",
                        PartitionKey = $"{tenantId}-{asset.id}",
                        EventDetails = new List<EventDetail>()
                    });

                    insertedProtocolAlarms.Add(new EventWithAlarmId { EventId = eventid, AlarmId = protocolAlarm.AlarmId });

                    eventDetails.Add(new EventDetail
                    {
                        id = Guid.NewGuid(),
                        AlarmId = protocolAlarm.AlarmId,
                        DocumentType = "EventDetail",
                        EventId = eventid,
                        EventStartTime = Convert.ToDateTime(values[4])
                    });

                    //await CreateReadyLineItem(container, item);
                }
            }

            foreach (var evnt in events)
            {
                var eventDtls = eventDetails.Where(x => x.EventId == evnt.id).ToList();
                evnt.EventDetails = eventDtls;

                await CreateReadyLineItem(container, evnt);
            }

            //foreach (var eventDetail in eventDetails)
            //{

            //    await CreateReadyLineItem(container, eventDetail);
            //}

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateReadyLineItem(Container container, dynamic item)
        {
            await container.CreateItemAsync(item);
        }
    }

    public class WencoContainer
    {
        //public static List<string> Ids => new List<string> { 
        //    "Site", "ProtocolAlarm", "ProtocolSensor", "AssetModel", "AssetType", "Asset", "Asset", "Event", "Protocol" };

        //public static List<string> CsvFileNames => new List<string> {
        //    "SITE","ALARM_PROTOCOL","SENSOR_PROTOCOL","EQUIP_MODEL","EQUIP_TYPE","EQUIP_HEALTH_PROTOCOL","EQUIP","EVENT","HEALTH_PROTOCOL_TYPE" };

        //public static List<string> PartitionKeyPaths => new List<string> {
        //    "/id","/protocolid","/protocolid","/desc","/name","/siteid","/siteid","/id","/id" };

        public static List<string> Ids => new List<string> {
            "Site", "ProtocolAlarm_Test", "ProtocolSensor", "AssetModel", "AssetType", "Asset", "Asset", "Event_Test", "Protocol_Test" };

        public static List<string> CsvFileNames => new List<string> {
            "SITE","ALARM_PROTOCOL","SENSOR_PROTOCOL","EQUIP_MODEL","EQUIP_TYPE","EQUIP_HEALTH_PROTOCOL","EQUIP","EVENT","HEALTH_PROTOCOL_TYPE" };

        //public static List<string> PartitionKeyPaths => new List<string> {
        //    "/id","/protocolid","/protocolid","/desc","/name","/siteid","/siteid","/id","/id" };
    }
}
