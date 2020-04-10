using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Pirat.Services.Mail;

namespace Pirat.Services.Schedule
{
    public class ScheduledNotificationService : IScheduledNotificationService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private System.Timers.Timer _timer;


        public ScheduledNotificationService(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Every day at 9:00 (hopefully 9 in Germany, no idea if the time zone is correctly configured...)
            CronExpression cronExpression = CronExpression.Parse("0 9 * * *");
            var next = cronExpression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
            if (next.HasValue)
            {
                TimeSpan delay = next.Value - DateTimeOffset.Now;
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        using (IServiceScope scope = _scopeFactory.CreateScope())
                        {
                            ISubscriptionService subscriptionService =
                                scope.ServiceProvider.GetRequiredService<ISubscriptionService>();
                            await subscriptionService.SendEmailsAsync();
                        }
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await StartAsync(cancellationToken);
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }
    }
}