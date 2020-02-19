using System;
using System.Collections.Generic;
using System.Text;

namespace S3Reader.Core
{
    public class AwsS3BucketOptions
    {
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string FolderPath { get; set; }
    }
}
