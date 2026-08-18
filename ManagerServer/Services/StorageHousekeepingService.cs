using Microsoft.Extensions.Hosting;
using Sentry;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ManagerServer.Services
{
    public sealed class StorageHousekeepingService(ApplicationData applicationData) : BackgroundService
    {
        private static readonly TimeSpan StartupDelay = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try { await Task.Delay(StartupDelay, stoppingToken); }
            catch (TaskCanceledException) { return; }

            try
            {
                await applicationData.Storage.Pack();
                await applicationData.Storage.Consolidate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StorageHousekeeping: {ex.GetType().Name}: {ex.Message}");
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
