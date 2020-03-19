using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cosmos.DataGenerator
{
    class Program
    {
        public static string EndpointUrl = "";
        public static string PrimaryKey = "";
        public static string DatabaseName = "wenco-cosmoDB";
        public static string _siteId = "7509c107-ce13-498e-9ae4-755549491604".ToLower();
        public static string FilePath = @"F:\Vinod\Projects\SelfProject\SelfProject\SelfProject\Cosmos.DataGenerator\csv\";
        private static string tenantId = "1a64190c-0859-4b80-8ace-1ee7ed6be4af";
        private static string _partitionKey = $"{tenantId}-{_siteId}";
        private static Database database;

        public static List<string> Ids => new List<string> {
            "Site", "ProtocolAlarm", "ProtocolSensor", "AssetModel", "AssetType", "Asset", "Asset", "Event", "Protocol" };

        public static List<string> CsvFileNames => new List<string> {
            "SITE","ALARM_PROTOCOL","SENSOR_PROTOCOL","EQUIP_MODEL","EQUIP_TYPE","EQUIP_HEALTH_PROTOCOL","EQUIP","EVENT","HEALTH_PROTOCOL_TYPE" };


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            CosmosClient client = new CosmosClient(EndpointUrl, PrimaryKey);

            database = client.GetDatabase(DatabaseName);

            Container container = database.CreateContainerIfNotExistsAsync(
                     "ProtocolAlarm",
                     "/PartitionKey",
                     400).Result;

            container = database.CreateContainerIfNotExistsAsync(
                     "Event",
                     "/PartitionKey",
                     400).Result;

            //Container container = database.CreateContainerIfNotExistsAsync(
            //         "Protocol",
            //         "/PartitionKey",
            //         400).Result;

            //CreateDatabase().Wait();
            UpsertCollectionData().Wait();
        }

        public static async Task CreateDatabase()
        {
            CosmosClient client = new CosmosClient(EndpointUrl, PrimaryKey);

            database = await client.CreateDatabaseIfNotExistsAsync(DatabaseName);

            for (int i = 0; i < WencoContainer.Ids.Count; i++) {
                Container container = await database.CreateContainerIfNotExistsAsync(
                    WencoContainer.Ids[i],
                    "/PartitionKey",
                    400);
            }

            await UpsertCollectionData();
        }

        private static async Task UpsertCollectionData()
        {
            //await CreateSiteItems();

            await CreateAlarmProtocolItems();

            ////await CreateSensorProtocolItems();

            //await CreateAssetModelItems();

            //await CreateAssetTypeItems();

            //await CreateAssetItems();

            await CreateEventItems();

            //await CreateHealthProtocolsItems();
        }

        private static async Task CreateSiteItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[0]);
            if (container == null) return;

            // Create an item
            dynamic item = new
            {
                id = _siteId,
                Name = "Kansanshi Mines",
                CreatedBy = "",
                DateCreated = DateTime.UtcNow,
                TenantId = "1a64190c-0859-4b80-8ace-1ee7ed6be4af",
                OrganisationId = "01c0679c-9716-4140-b574-9a3ef50e0ac0",
                LocationId = "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                //GeoZones = new object[],
                timeZone = "",
                PartitionKey = tenantId
            };

            await CreateReadyLineItem(container, item);
        }

        static List<ProtocolAlarm> protocolAlarms = new List<ProtocolAlarm>();

        private class ProtocolAlarm
        {
            public string id { get; set; }
            public int AlarmId { get; set; }
            public string ProtocolId { get; set; }
            public int Active { get; set; }
            public string Name { get; set; }
            public string Severity { get; set; }
            public string PartitionKey { get; set; }
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

        private static async Task CreateAlarmProtocolItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[1]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[1]}.csv";

            var csvData = File.ReadAllLines(fileName);
            
            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                var item = new ProtocolAlarm
                {
                    id = Guid.NewGuid().ToString().ToLower(),
                    AlarmId = Convert.ToInt32(values[2]),
                    //event_oem_code = Convert.ToString(values[1]),
                    ProtocolId = Convert.ToString(values[0]).ToLower(),
                    Active = Convert.ToString(values[6]) == "Y" ? 1 : 0,
                    Name = Convert.ToString(values[7]),
                    Severity = GetSeverity(Convert.ToString(values[5])),
                    PartitionKey = _partitionKey
                };

                //dynamic item = new
                //{
                //    id = Guid.NewGuid().ToString().ToLower(),
                //    alarmId = Convert.ToInt32(values[2]),
                //    //event_oem_code = Convert.ToString(values[1]),
                //    protocolId = Convert.ToString(values[0]),
                //    active = Convert.ToString(values[6]) == "Y" ? 1 : 0,
                //    name = Convert.ToString(values[7]),
                //    severity = Convert.ToString(values[5]),
                //    PartitionKey = _partitionKey
                //    //protocol = "HCM",
                //};

                await CreateReadyLineItem(container, item);
                protocolAlarms.Add(item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateSensorProtocolItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[2]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[2]}.csv";

            var csvData = File.ReadAllLines(fileName);

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                dynamic item = new
                {
                    id = Guid.NewGuid(),
                    protocolid = Convert.ToString(values[0]),
                    active = Convert.ToString(values[7]) == "Y" ? 1 : 0,
                    name = Convert.ToString(values[11]),
                    description = Convert.ToString(values[3]),
                    uom = Convert.ToString(values[6]),
                    bitlen = Convert.ToInt32(values[8]),
                    scale = Convert.ToInt32(values[9]),
                    offset = Convert.ToInt32(values[10]),
                    protocol = "HCM",
                    PartitionKey = _partitionKey
                };

                await CreateReadyLineItem(container, item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateAssetModelItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[3]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[3]}.csv";

            var csvData = File.ReadAllLines(fileName);

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                dynamic item = new
                {
                    id = Convert.ToString(values[0]).ToLower(),
                    code = Convert.ToString(values[1]),
                    name = Convert.ToString(values[2]),
                    number = Convert.ToString(values[1]),
                    assetTypeId = Guid.Parse("79DD41BD-0102-4416-8F16-32E2E28FC664").ToString().ToLower(),
                    PartitionKey = _partitionKey
                };

                await CreateReadyLineItem(container, item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateAssetTypeItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[4]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[4]}.csv";

            var csvData = File.ReadAllLines(fileName);

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                dynamic item = new
                {
                    id = Convert.ToString(values[0]).ToLower(),
                    Value = Convert.ToString(values[1]),
                    Label = Convert.ToString(values[2]),
                    Icon = Convert.ToString(values[9]),
                    TenantId = "1a64190c-0859-4b80-8ace-1ee7ed6be4af",
                    PartitionKey = Convert.ToString(values[0]).ToLower()
                };

                await CreateReadyLineItem(container, item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateAssetItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[6]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[6]}.csv";

            var csvData = File.ReadAllLines(fileName);

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                dynamic item = new
                {
                    id = Convert.ToString(values[0]).ToLower(),
                    siteId = _siteId,
                    assetModelId = Guid.Parse("664B1437-E3C8-4AFE-AC59-20CBB46A66FE").ToString().ToLower(),
                    assetTypeId = Guid.Parse("79DD41BD-0102-4416-8F16-32E2E28FC664").ToString().ToLower(),
                    name = Convert.ToString(values[2]),
                    description = Convert.ToString(values[2]),
                    status = "Active",
                    TenantId = tenantId,
                    PartitionKey = _partitionKey
                };

                await CreateReadyLineItem(container, item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateEventItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[7]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[7]}.csv";

            var csvData = File.ReadAllLines(fileName);

            var rand = new Random();

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                var protocolAlarmId = protocolAlarms.FirstOrDefault(x => x.AlarmId == Convert.ToInt32(values[5])).id;

                var startTime = Convert.ToDateTime(values[4]);
                var durationMinutes = rand.Next(21, 150);
                var endTime = startTime.AddMinutes(durationMinutes);

                // Create an item
                dynamic item = new
                {
                    id = Guid.NewGuid().ToString().ToLower(),
                    SiteId = _siteId,
                    AssetId = Convert.ToString(values[0]).ToLower(),
                    ProtocolAlarmId = protocolAlarmId,
                    ProtocolId = Convert.ToString(values[7]).ToLower(),
                    Type = ((Convert.ToInt32(values[5]) % 10000) < 10) ? "BOUNDRY" :
                        ((Convert.ToInt32(values[5]) % 10000) > 10 && (Convert.ToInt32(values[5]) % 10000) < 20) ? "VIRTUAL" :
                          ((Convert.ToInt32(values[5]) % 10000) > 20 && (Convert.ToInt32(values[5]) % 10000) < 40) ? "SAFETY" :
                          "OEM",
                    CreatedOn = Convert.ToDateTime(values[4]),
                    EndTime = endTime,
                    Active = Convert.ToInt32(values[6]) == 1 ? true : false,
                    Status = "Un-Ack",
                    PartitionKey = _partitionKey
                };

                await CreateReadyLineItem(container, item);
            }

            Console.WriteLine($"{fileName} processed successfully.");
        }

        private static async Task CreateHealthProtocolsItems()
        {
            Container container = database.GetContainer(WencoContainer.Ids[8]);
            if (container == null) return;

            string fileName = $"{FilePath}{WencoContainer.CsvFileNames[8]}.csv";

            var csvData = File.ReadAllLines(fileName);

            foreach (string row in csvData.Skip(1).ToList())
            {
                string[] values = row.Split(',');

                // Create an item
                dynamic item = new
                {
                    id = Convert.ToString(values[0]).ToLower(),
                    active = 1,
                    name = Convert.ToString(values[1]),
                    description = Convert.ToString(values[3]),
                    version = Convert.ToString(values[2]),
                    PartitionKey = _partitionKey
                };

                await CreateReadyLineItem(container, item);
            }

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
            "Site", "ProtocolAlarm", "ProtocolSensor", "AssetModel", "AssetType", "Asset", "Asset", "Event", "Protocol" };

        public static List<string> CsvFileNames => new List<string> {
            "SITE","ALARM_PROTOCOL","SENSOR_PROTOCOL","EQUIP_MODEL","EQUIP_TYPE","EQUIP_HEALTH_PROTOCOL","EQUIP","EVENT","HEALTH_PROTOCOL_TYPE" };

        //public static List<string> PartitionKeyPaths => new List<string> {
        //    "/id","/protocolid","/protocolid","/desc","/name","/siteid","/siteid","/id","/id" };
    }
}
