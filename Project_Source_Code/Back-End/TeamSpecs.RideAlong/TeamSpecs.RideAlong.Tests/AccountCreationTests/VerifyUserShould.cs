using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.ServiceLog.Interfaces;
using TeamSpecs.RideAlong.ServiceLog;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Services;

namespace TeamSpecs.RideAlong.TestingLibrary.AccountCreationTests
{
    public class VerifyUserShould
    {
        private static readonly IConfigServiceJson configService = new ConfigServiceJson();
        private static readonly IGenericDAO dao = new SqlServerDAO(configService);
        private static readonly IHashService hashService = new HashService();
        private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
        private static readonly ILogService logService = new LogService(logTarget, hashService);
        private static readonly IRandomService randomService = new RandomService();
        private static readonly IMailKitService mailKitService = new MailKitService(configService);

        private static readonly IPepperTarget pepperTarget = new FilePepperTarget(dao);
        private static readonly IPepperService pepperService = new PepperService(pepperTarget, randomService);

        private static readonly ISqlDbUserCreationTarget sqlTarget = new SqlDbUserCreationTarget(dao);
        private static readonly IAccountCreationService accountCreationService = new AccountCreationService
            (sqlTarget, pepperService, hashService, logService, randomService, mailKitService);


    }
}
