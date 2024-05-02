using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.DonateYourCarLibrary;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary.DonateYourCarTests
{
    public class DonateYourCarServiceShould
    {
        [Fact]

        public void DonateYourCar_ReturnCharities()
        {
            //Arrange
            var timer = new Stopwatch();

            IResponse response;

            var config = new ConfigServiceJson();
            var dao = new SqlServerDAO(config);
            var charityTarget = new SqlDbCharityTarget(dao);

            var hashService = new HashService();
            var logTarget = new SqlDbLogTarget(dao);
            var logService = new LogService(logTarget, hashService);

            var retrievalService = new CharityRetrievalService(charityTarget, logService);

            var user = new AccountUserModel("testUser")
            {
                Salt = 0,
                UserHash = "testUserHash",
            };


            try
            {
                var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                {
                    KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null)
                });
            }
            catch
            {
                // In case creating the initial sql data does not work
                var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                {
                    KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
                });
            }

            //Act
            try
            {
                timer.Start();
                response = retrievalService.RetrieveCharities(user);
                timer.Stop();
            }
            finally
            {
                var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = ' {user.UserHash}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
                {
                    KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
                });
            }

            //Assert
            Assert.True(timer.Elapsed.TotalSeconds <= 3);
            Assert.NotNull(response.ReturnValue);
            Assert.True(response.ReturnValue.Count == 4);

        }
    }
}

