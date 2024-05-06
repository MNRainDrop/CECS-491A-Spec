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

namespace TeamSpecs.RideAlong.TestingLibrary.AccountCreationTests
{
    public class VerifyUserShould
    {
        private static readonly IConfigServiceJson configService = new ConfigServiceJson();
        private static readonly IGenericDAO dao = new SqlServerDAO(configService);
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
        public void VerifyUser_UnknownUserPassedIn_CreateUser_Pass()
        {
            #region Arrange
            var timer = new Stopwatch();
            var userName = "jmichael272@yahoo.com";
            IResponse response;
            #endregion

            #region Act
            timer.Start();
            response = accountCreationService.verifyUser(userName);
            timer.Stop();

            Thread.Sleep(1000); // Waiting for Log Async Task to finish

            #region Clean Up DB
            // Delete from Log table using UserName
            var undoLogInsert = $"DELETE FROM Log WHERE UserHash IN (SELECT UserHash FROM UserAccount WHERE UserName = @UserName)";
            var logParams = new HashSet<SqlParameter>()
            {
                new SqlParameter("@UserName", userName)
            };
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoLogInsert, logParams)
            });

            // Delete from OTP table using UserName
            var undoOtpInsert = $"DELETE FROM OTP WHERE UID IN (SELECT UID FROM UserAccount WHERE UserName = @UserName)";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoOtpInsert, logParams)
            });

            // Delete from UserAccount table using UserName
            var undoUserAccountInsert = $"DELETE FROM UserAccount WHERE UserName = @UserName";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoUserAccountInsert, logParams)
            });
            #endregion

            #endregion

            #region Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response);
            Assert.True(!response.HasError);
            Assert.True(response.ReturnValue != null && response.ReturnValue.Contains(2));
            #endregion
        }


    }
}
