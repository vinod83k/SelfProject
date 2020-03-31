using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.DataGenerator
{
    public class Site
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string OrganizationCode { get; set; }
        public Guid TenantId { get; set; }

        public static List<Site> _sites { get; set; }

        public static List<Site> GetCollection()
        {
            return _sites;
        }

        public static void PopulateCollection(Database database)
        {
            _sites = Utility.PopulateObject<Site>(database, "Site");
            //_sites = new List<Site>();
            //Container container = database.GetContainer("Site");
            //if (container == null) return;

            //QueryDefinition queryDefinition = new QueryDefinition("select * from c");
            //FeedIterator<Site> feedIterator = container.GetItemQueryIterator<Site>(queryDefinition);

            //while (feedIterator.HasMoreResults)
            //{
            //    foreach (var item in feedIterator.ReadNextAsync().Result)
            //    {
            //        {
            //            _sites.Add(item);
            //        }
            //    }
            //}
        }
    }
}
