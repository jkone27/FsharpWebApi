using CSharpWebApiSample.Domain;
using CSharpWebApiSample.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpWebApiSample
{
    public class PetsBackgroundJob : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<PetsBackgroundJob> _logger;
        private readonly PetsApiClient petsApiClient;
        private Timer _timer;

        public PetsBackgroundJob(ILogger<PetsBackgroundJob> logger,
            PetsApiClient petsApiClient)
        {
            _logger = logger;
            this.petsApiClient = petsApiClient;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                var count = Interlocked.Increment(ref executionCount);

                var result = petsApiClient.GetPetsAync().ConfigureAwait(false).GetAwaiter().GetResult();

                _logger.LogInformation($"Retrieved Pets! {result.Count()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Job is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}