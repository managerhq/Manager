using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ManagerServer.Services
{
    public sealed class IdleShutdownService : BackgroundService
    {
        private readonly IdleShutdownOptions options;
        private readonly IdleTracker idleTracker;
        private readonly IHostApplicationLifetime lifetime;

        public IdleShutdownService(IdleShutdownOptions options, IdleTracker idleTracker, IHostApplicationLifetime lifetime)
        {
            this.options = options;
            this.idleTracker = idleTracker;
            this.lifetime = lifetime;
        }

        // Cost-control curve for tenant processes:
        //  - 0 to GraceHours: full InitialTimeout (don't punish fresh sessions).
        //  - past GraceHours: idle budget halves every HalfLifeHours.
        //  - past MaxAgeHours: hard cap, defends against keep-alive clients that the
        //    exponential curve alone only converges toward asymptotically.
        internal static TimeSpan ComputeIdleTimeout(TimeSpan age, IdleShutdownOptions options)
        {
            if (age.TotalHours >= options.MaxAgeHours) return TimeSpan.Zero;
            if (age.TotalHours <= options.GraceHours) return options.InitialTimeout;

            var decayHours = age.TotalHours - options.GraceHours;
            var factor = Math.Pow(0.5, decayHours / options.HalfLifeHours);
            return options.InitialTimeout * factor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var startTime = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Still alive at {DateTime.Now} ({ThreadPool.ThreadCount} + {ThreadPool.PendingWorkItemCount})");

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                var age = DateTime.UtcNow - startTime;
                var idleTimeout = ComputeIdleTimeout(age, options);

                if (idleTracker.Idle > idleTimeout)
                {
                    Console.WriteLine($"Idle shutdown: age={age.TotalHours:F1}h idle={idleTracker.Idle.TotalSeconds:F0}s > timeout={idleTimeout.TotalSeconds:F0}s");
                    lifetime.StopApplication();
                    return;
                }
            }
        }
    }
}
