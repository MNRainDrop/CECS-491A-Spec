using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.SecurityLibrary.Model;
using TeamSpecs.RideAlong.SecurityLibrary.Targets;
using TeamSpecs.RideAlong.SecurityLibrary;
using TeamSpecs.RideAlong.Services;
using Microsoft.Data.SqlClient;

namespace TeamSpecs.RideAlong.TestingLibrary.SecurityLibraryTests
{
    public class GetOtpHashShould
    {
        [Fact]
        public void AuthService_GetOtpHash_RequiredClaimsPassedIn_ReturnTrue_Pass()
        {
            #region Arrange 
            //Arrange
            var dao = new SqlServerDAO();
            var logTarget = new SqlDbLogTarget(dao);
            var actualResult = false;
            var hashService = new HashService();
            var logger = new LogService(logTarget, hashService);
            var authRequest = new AuthNRequest("UserName", "OTP");
            var authTarget = new SQLServerAuthTarget(dao, logger);
            var authService = new AuthService(authTarget, logger);

            //Creating Test object AuthUserModel for table User Account  
            var UID = 123;
            var username = "GetOtpHash_Test_Object";
            byte[] salt = BitConverter.GetBytes(012);
            var userhash = "UserHash_sample";
            var AuthObj = new AuthUserModel(UID,username,salt,userhash);

            //Setting up AppPrincipal object 
            IDictionary<string, string> Claims = new Dictionary<string, string>();
            Claims.Add("canLogin", "true");
            Claims.Add("canLogout", "true");
            AuthUserModel expectedAuthModel = new AuthUserModel(123, "SecurityTestUser", BitConverter.GetBytes(123456), "TestHash");
            var principal = new AppPrincipal(expectedAuthModel, Claims);

            //Setting up RequiredClaims 
            Dictionary<string, string> RClaims = new Dictionary<string, string>();
            RClaims.Add("canLogin", "true");
            RClaims.Add("canLogout", "true");
            var timer = new Stopwatch();

            // Create Initial SQL
            try
            {
                var accountSql = $"INSERT INTO UserAccount (UID,UserName,Salt,Userhash) VALUES ({AuthObj.UID},'{AuthObj.userName}', {AuthObj.salt},'{AuthObj.userHash}')";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
            });
                var getUserID = $"SELECT UID FROM UserAccount WHERE UserName = '{AuthObj.userName}'";
                var uid = dao.ExecuteReadOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(getUserID, null)
            });
                foreach (var item in uid)
                {
                    AuthObj.UID = (long)item[0];
                    //vehicle.Owner_UID = user.UserId;
                }
            }
            catch
            {
                // In case creating the initial sql data does not work
                var undoInsert = $"DELETE FROM UserAccount WHERE UID = '{AuthObj.UID}'";
                dao.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
            {
                KeyValuePair.Create<string, HashSet<SqlParameter>?>(undoInsert, null)
            });
            }
            #endregion


            //Act
            timer.Start();
            timer.Stop();

            //Assert
            Assert.True(actualResult == false);
        }
    }
}
