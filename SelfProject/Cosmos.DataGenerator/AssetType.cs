using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.DataGenerator
{
    public class AssetType
    {
        public string id { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }

        public static List<AssetType> _assetTypes { get; set; }

        public static List<AssetType> GetCollection()
        {
            return _assetTypes;
        }

        public static void PopulateCollection(Database database)
        {
            _assetTypes = Utility.PopulateObject<AssetType>(database, "AssetType");
        }
    }
}
