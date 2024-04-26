using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.ServiceLog.Interfaces;

namespace TeamSpecs.RideAlong.ServiceLog
{
    public class ServiceLogManager
    {
        private readonly ILogService _logService;
        private readonly IServiceLogService _serviceLog;

        public ServiceLogManager(ILogService logService, IServiceLogService serviceLogService) 
        { 
            _logService = logService;
            _serviceLog = serviceLogService;
        }

        // Add Classes

        /// 
        /// For retrieve, should make seperate classes in order to log different requirements 
        /// Also for different logs needed and such// Checks
        /// 


    }
}
