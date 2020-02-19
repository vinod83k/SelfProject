using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using S3Reader.Core;

namespace S3Reader.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        static LowFrequencyDataReader lowFrequencyDataReader;

        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            var awsSettingOptions = _configuration.GetSection("Settings:AWS");
            var options = awsSettingOptions.Get<AwsS3BucketOptions>();

            lowFrequencyDataReader = new LowFrequencyDataReader(options);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);

                
            //}

            await lowFrequencyDataReader.GetObject();
        }
    }
}
