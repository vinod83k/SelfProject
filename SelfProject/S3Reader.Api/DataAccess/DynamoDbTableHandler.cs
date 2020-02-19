using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace S3Reader.Core.DataAccess
{
    public class DynamoDbTableHandler
    {
        private readonly AmazonDynamoDBClient client;
        private static string tableName = "Alerts";
        private string bucketName;
        private string awsAccessKey;
        private string awsSecretKey;
        private string folterPathPrefix;

        public DynamoDbTableHandler(AwsS3BucketOptions _options)
        {
            bucketName = _options.BucketName;
            awsAccessKey = _options.AccessKey;
            awsSecretKey = _options.SecretKey;
            folterPathPrefix = _options.FolderPath;
            client = new AmazonDynamoDBClient(awsAccessKey, awsSecretKey, RegionEndpoint.APSouth1);
        }

        public async Task CreateExampleTable()
        {
            Console.WriteLine("\n*** Creating table ***");
            var request = new CreateTableRequest
            {
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = "N"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "CreatedDateTime",
                        AttributeType = "N"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = "HASH" //Partition key
                    },
                    new KeySchemaElement
                    {
                        AttributeName = "CreatedDateTime",
                        KeyType = "RANGE" //Sort key
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 6
                },
                TableName = tableName
            };

            var response = await client.CreateTableAsync(request);

            var tableDescription = response.TableDescription;
            Console.WriteLine("{1}: {0} \t ReadsPerSec: {2} \t WritesPerSec: {3}",
                      tableDescription.TableStatus,
                      tableDescription.TableName,
                      tableDescription.ProvisionedThroughput.ReadCapacityUnits,
                      tableDescription.ProvisionedThroughput.WriteCapacityUnits);

            string status = tableDescription.TableStatus;
            Console.WriteLine(tableName + " - " + status);

            WaitUntilTableReady(tableName);
        }

        private async Task WaitUntilTableReady(string tableName)
        {
            string status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var res = await client.DescribeTableAsync(new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                    Console.WriteLine("Table name: {0}, status: {1}",
                              res.Table.TableName,
                              res.Table.TableStatus);
                    status = res.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                }
            } while (status != "ACTIVE");
        }
    }
}
