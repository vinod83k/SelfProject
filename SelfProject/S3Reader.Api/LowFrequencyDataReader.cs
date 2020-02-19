using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using S3Reader.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace S3Reader.Core
{
    public class LowFrequencyDataReader : ILowFrequencyDataReader
    {
        private string bucketName;
        private string awsAccessKey;
        private string awsSecretKey;
        private string folterPathPrefix;

        private readonly IAmazonS3 client;
        private readonly DynamoDbTableHandler dynamoDbTableHandler;

        public LowFrequencyDataReader(AwsS3BucketOptions _options)
        {
            bucketName = _options.BucketName;
            awsAccessKey = _options.AccessKey;
            awsSecretKey = _options.SecretKey;
            folterPathPrefix = _options.FolderPath;
            client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.APSouth1);
            dynamoDbTableHandler = new DynamoDbTableHandler(_options);
        }

        public async Task GetObject()
        {
            ListObjectsRequest request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = folterPathPrefix
            };

            ListObjectsResponse response = await client.ListObjectsAsync(request);
            foreach (S3Object obj in response.S3Objects)
            {
                Console.WriteLine(obj.Key);
                await ReadObjectDataAsync(obj.Key);
            }
        }

        public async Task ReadObjectDataAsync(string keyName)
        {
            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    responseBody = reader.ReadToEnd(); // Now you process the response body.
                    Console.WriteLine(responseBody);
                }

                await dynamoDbTableHandler.CreateExampleTable();
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
    }
}
