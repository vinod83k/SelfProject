using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.DataGenerator
{
    public class Asset
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssetModelId { get; set; }
        public string AssetTypeId { get; set; }
        public string SiteId { get; set; }

        public static List<Asset> _assets { get; set; }

        public static List<Asset> GetCollection()
        {
            return _assets;
        }

        public static void PopulateCollection(Database database)
        {
            _assets = Utility.PopulateObject<Asset>(database, "Asset");
        }
    }

    public class AssetModel {
        public string id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
