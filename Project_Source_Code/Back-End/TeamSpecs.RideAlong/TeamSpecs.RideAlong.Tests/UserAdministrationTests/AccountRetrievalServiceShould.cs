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
using System.Diagnostics;
using TeamSpecs.RideAlong.Model;
using Microsoft.Data.SqlClient;
using TeamSpecs.RideAlong.VehicleMarketplace;

namespace TeamSpecs.RideAlong.TestingLibrary
{
    public class AccountRetrievalServiceShould
    {
        private static readonly IConfigServiceJson configService = new ConfigServiceJson();
        private static readonly ISqlServerDAO dao = new SqlServerDAO(configService);
        private static readonly JsonFileDAO dao2 = new JsonFileDAO();
        private static readonly IHashService hashService = new HashService();
        private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
        private static readonly ILogService logService = new LogService(logTarget, hashService);
        private static readonly IRandomService randomService = new RandomService();
        private static readonly IMailKitService mailKitService = new MailKitService(configService);

        private static readonly IPepperTarget pepperTarget = new FilePepperTarget(dao2);
        private static readonly IPepperService pepperService = new PepperService(pepperTarget, randomService);

        private static readonly ISqlDbUserCreationTarget sqlTarget = new SqlDbUserCreationTarget(dao);
        private static readonly IAccountCreationService accountCreationService = new AccountCreationService
            (sqlTarget, pepperService, hashService, logService, randomService, mailKitService);

        [Fact]
        public void AccountRetrievalServiceShould_UidPassedIn_RetrieveAll_Pass()
        {
            //Arrange 
            var timer = new Stopwatch();
            ConfigServiceJson configService = new ConfigServiceJson();
            var dao = new SqlServerDAO(configService);
            var Jsondao = new JsonFileDAO();
            var _target = new SqlDbUserRetrievalTarget(dao);
            var _mailKitService = new MailKitService(configService);


            IResponse response;

            //Parameters 
            /* string VIN = "VIN2";
            int view = 1;
            string description = "This is test case 1";
            int status = 1;*/

            var uid = 0;


            //Service 
            AccountRetrievalService View = new AccountRetrievalService(_target,_mailKitService);

            //Act 
            timer.Start();
            response = View.RetrieveAccount(uid);
            timer.Stop();


            //Assert 
            Assert.True(timer.Elapsed.TotalSeconds <= 5);
            Assert.True(response.HasError == false);


        }


    }
}
