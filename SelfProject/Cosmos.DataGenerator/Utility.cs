using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.DataGenerator
{
    public class Utility
    {
        public static List<T> PopulateObject<T>(Database database, string entityName)
        {
            var list = new List<T>();
            Container container = database.GetContainer(entityName);
            if (container == null) new List<T>();

            QueryDefinition queryDefinition = new QueryDefinition("select * from c");
            FeedIterator<T> feedIterator = container.GetItemQueryIterator<T>(queryDefinition);

            while (feedIterator.HasMoreResults)
            {
                foreach (var item in feedIterator.ReadNextAsync().Result)
                {
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }
    }
}
