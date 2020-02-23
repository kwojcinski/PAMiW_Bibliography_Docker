using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Lib.AspNetCore.ServerSentEvents;

namespace PAMiW_291118.Services
{
    internal class HeartbeatService : BackgroundService
    {
        private const string HEARTBEAT_MESSAGE_FORMAT = "PAMiW_291118 Heartbeat ({0} UTC)";

        private readonly IServerSentEventsService _serverSentEventsService;
        public HeartbeatService(IServerSentEventsService serverSentEventsService)
        {
            _serverSentEventsService = serverSentEventsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _serverSentEventsService.SendEventAsync(String.Format(HEARTBEAT_MESSAGE_FORMAT, DateTime.UtcNow));

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
