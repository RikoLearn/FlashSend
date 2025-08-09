namespace Server.Services
{
    public class ExpiredDataCleanupService : BackgroundService
    {

        private readonly ITimedCleanupService _timedCleanupService;

        public ExpiredDataCleanupService(ITimedCleanupService timedCleanupService)
        {
            _timedCleanupService = timedCleanupService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _timedCleanupService.Cleanup(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
