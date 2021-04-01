using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebinarTelegram.TelegramBot.Services;

namespace WebinarTelegram
{
    internal class Worker : BackgroundService
    {
        private readonly IBot _bot;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IBot bot)
        {
            _logger = logger;
            _bot = bot;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical("Stop Service");
            return base.StopAsync(cancellationToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start Service");
            _bot.Init();
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}