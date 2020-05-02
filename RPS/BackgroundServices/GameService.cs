using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RPS.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RPS.BackgroundServices
{
    public class GameService : BackgroundService
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly ILogger<GameService> _logger;

        public GameService(IHubContext<GameHub> hubContext, ILogger<GameService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // do something
                await Task.Delay(5000);
            }
        }
    }
}
