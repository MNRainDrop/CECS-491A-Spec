using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;
using TeamSpecs.RideAlong.UserAdministration;
using TeamSpecs.RideAlong.UserAdministration.Interfaces;
using TeamSpecs.RideAlong.UserAdministration.Services;

namespace TeamSpecs.RideAlong.TestingLibrary.UserAdminstrationTests
{
    public class PostUpdateUserShould
    {
        private static readonly IConfigServiceJson configService = new ConfigServiceJson();
        private static readonly ISqlServerDAO dao = new SqlServerDAO(configService);
        private static readonly IUserTarget userTarget = new SqlDbUserTarget(dao);

        private static readonly IHashService hashService = new HashService();
        private static readonly ILogTarget logTarget = new SqlDbLogTarget(dao);
        private static readonly ILogService logService = new LogService(logTarget, hashService);

        private static readonly IPostAccountUpdateService postService = new PostAccountUpdateService(userTarget, logService);
        [Fact]
        public void UserAdministartionPostUpdateUserServiceShould()
        {
            //Arrange
            var timer = new Stopwatch();


            IResponse response;

            //Parameters 
            var user = new AccountUserModel("test")
            {
                UserId = 5,                       // Sample user ID
                Salt = 123456,                         // Sample salt value
                UserHash = "123"           // Sample user hash (hashed password)
            };

            string address = "123 W. 456St. Narnia CA 90220";
            string name = "John Doe";
            string phone = "(310) 123-4567";
            string accounttype = "Default User";

            var accountSql = $"INSERT INTO UserAccount (UserName, Userhash, Salt) VALUES ('{user.UserName}', '{user.UserHash}', {user.Salt})";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null)
        });
            var userDetails = $"INSERT INTO UserDetails (UID, AccountType) VALUES ({user.UserId}, '{accounttype}')";
            dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(userDetails, null)
        });
            //Act
            //Service
            try
            {
                timer.Start();
                response = postService.UpdateUserAccount(user, address, name, phone, accounttype);
                timer.Stop();
            }
            finally
            {
                var undoInsert = $"DELETE FROM UserAccount WHERE UserHash = '{user.UserHash}'; DELETE FROM UserDetails WHERE UID = {user.UserId}";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
            }
            //Assert
            Assert.True(response is not null);
            Assert.True(response.HasError == false);
            Assert.True(response.ReturnValue is not null);
            Assert.True(timer.ElapsedMilliseconds < 3000);
            //Assert.True(response.ReturnValue.First() == );
        }
    }
}
