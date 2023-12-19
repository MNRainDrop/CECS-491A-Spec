using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.UserAdministration;

namespace TeamSpecs.RideAlong.TestingLibrary.UserAdministrationTests
{
    public class AccountRecoveryServiceShould
    {
        [Fact]
        public void AccountRecoveryService_EnableUserAccount_ValidUserNamePassedIn_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var AccountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            var testUsername = "RecoverTestEmail@gmail.com";

            #region Creating Test User and Test Claim
            var accountSql = $"INSERT INTO UserAccount (UserName, Salt) VALUES ('{testUsername}', 0)";
            _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
        });
            #endregion

            // Act
            try
            {
                response = AccountRecoveryService.EnableUserAccount(testUsername);
            }
            finally
            {
                #region Deleting Test User and Test Claims
                var sql = $"DELETE FROM UserAccount WHERE UserName = '{testUsername}'";
                _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) });
                #endregion
            }
            // Assert
            Assert.False(response.HasError);
            Assert.Null(response.ErrorMessage);
        }
        [Fact]
        public void AccountRecoveryService_DisableUserAccount_ValidUserNamePassedIn_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var AccountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            var testUsername = "RecoverTestEmail@gmail.com";

            #region Creating Test User 
            var accountSql = $"INSERT INTO UserAccount (UserName, Salt) VALUES ('{testUsername}', 0)";
            _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
        });
            #endregion

            // Act
            try
            {
                response = AccountRecoveryService.DisableUserAccount(testUsername);
            }
            finally
            {
                #region Deleting Test User 
                var sql = $"DELETE FROM UserAccount WHERE UserName = '{testUsername}'";
                _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) });
                #endregion
            }
            // Assert
            Assert.False(response.HasError);
            Assert.Null(response.ErrorMessage);
        }
        [Fact]
        public void AccountRecoveryService_RecoverUserAccount_ValidUserNamePassedIn_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var AccountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            var testUsername = "Modifytestemail@gmail.com";
            var testAltUsername = "testAltEmail@gmail.com";
            var testDateTime = DateTime.Now;

            
            #region Creating Test User and Test Profile
            var accountSql = $"INSERT INTO UserAccount (UserName, Salt) VALUES ('{testUsername}', 0)";
            var profileSql = $"INSERT INTO UserProfile (UserID, AlternateUserName, DateCreated) VALUES ((SELECT TOP 1 UserID FROM UserAccount Where UserName = '{testUsername}'), '{testUsername}', GETUTCDATE())";
            _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(accountSql, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(profileSql, null),
        });
            #endregion

            // Act
            try
            {
                response = AccountRecoveryService.RecoverUserAccount(testUsername);
            }
            finally
            {
                // Delete test user and data
                var sql = $"DELETE FROM UserAccount WHERE UserName = '{testUsername}'";
                _DAO.ExecuteWriteOnly(new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) });
            }

            // Assert
            Assert.False(response.HasError);
            Assert.Null(response.ErrorMessage);
            foreach (string o in response.ReturnValue)
            {
                Assert.True(o.Equals(testAltUsername));
            }
        }
        [Fact]
        public void AccountRecoveryService_EnableUserAccount_NullUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = null;

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.EnableUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }
        [Fact]
        public void AccountRecoveryService_EnableUserAccount_EmptyUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = "";

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.EnableUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }

        [Fact]
        public void AccountRecoveryService_EnableUserAccount_WhiteSpaceUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = "          ";

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.EnableUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }
        [Fact]
        public void AccountRecoveryService_DisableUserAccount_NullUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = null;

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.DisableUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }
        [Fact]
        public void AccountRecoveryService_DisableUserAccount_EmptyUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = "";

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.DisableUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }

        [Fact]
        public void AccountRecoveryService_DisableUserAccount_WhiteSpaceUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = "          ";

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.DisableUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }
        [Fact]
        public void AccountRecoveryService_RecoverUserAccount_NullUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = null;

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.RecoverUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }
        [Fact]
        public void AccountRecoveryService_RecoverUserAccount_EmptyUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = "";

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.RecoverUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }

        [Fact]
        public void AccountRecoveryService_RecoverUserAccount_WhiteSpaceUserNamePassedIn_ArguementExceptionThrown_Pass()
        {
            // Arrange
            IResponse response;
            var _DAO = new SqlServerDAO();
            var accountRecoveryService = new AccountRecoveryService(new SqlDbUserTarget(_DAO));
            string testUsername = "          ";

            // Act and Assert
            try
            {
                Assert.Throws<ArgumentException>(
                    () => response = accountRecoveryService.RecoverUserAccount(testUsername)
                );
            }
            catch
            {
                Assert.Fail("Should throw ArgumentException");
            }
        }
    }
}
