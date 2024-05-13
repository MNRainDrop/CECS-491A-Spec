namespace TeamSpecs.RideAlong.CELibrary.Service
{
    public class SendEmail : ISendEmail
    {
        private readonly ISqlDbCETarget _target;
        private readonly ILogService;

        public SendEmail(ISqlDbCETarget target, ILogService logService)
        {
            _target = target;
            _logService = logService;
        }
    }
}
