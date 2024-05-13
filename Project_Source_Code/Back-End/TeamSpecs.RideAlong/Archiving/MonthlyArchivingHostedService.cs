using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.Archiving
{
    public class MonthlyArchivingHostedService : IReoccurringArchivingHostedService
    {
        private ILogService _logger;
        private IArchivingService _aService;
        public MonthlyArchivingHostedService(ILogService logger, IArchivingService aService)
        {
            _logger = logger;
            _aService = aService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
