namespace TeamSpecs.RideAlong.TestingLibrary;

using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

public class LoggingLibraryShould
{
    private string GenerateRandomHash()
    {
        string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVQXYZ0123456789";
        int length = 64; //Length of a hash is 64
        StringBuilder sb = new StringBuilder(length);
        Random random = new Random();
        for (int i = 0; i < length; i++)
        {
            int index = random.Next(AllowedChars.Length);
            sb.Append(AllowedChars[index]);
        }
        return sb.ToString();
    }
    [Fact]
    public void LL_Log_CreateAndStoreOneLog_LogWillBeStoredToDataStore_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()), new HashService());

        // Expected values
        var expectedHasError = false;
        string? expectedErrorMessage = null;
        var expectedReturnValue = 1;

        #region Generating a test user
        var userHash = GenerateRandomHash();
        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" + $"VALUES ('LoggingTestUser', 123456, '{userHash}')";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        var dao = new SqlServerDAO();
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion

        // Act
        timer.Start();
        response = logService.CreateLog("Info", "View", "This is a test message", userHash);
        timer.Stop();

        #region Deleting the test user
        sql = "DELETE FROM UserAccount WHERE UserName = 'LoggingTestUser'";
        sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion

        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        if (response.ReturnValue is not null)
        {
            Assert.Equal(expectedReturnValue, response.ReturnValue.First());
        }
        else
        {
            Assert.Fail("response.ReturnValue should not be null");
        }
    }

    [Fact]
    public async void Async_LL_Log_CreateAndStoreOneLog_LogWillBeStoredToDataStore_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()), new HashService());


        // Expected values
        var expectedHasError = false;
        string? expectedErrorMessage = null;
        var expectedReturnValue = 1;

        #region Generating a test user
        var userHash = GenerateRandomHash();
        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" + $"VALUES ('LoggingTestUser', 123456, '{userHash}')";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        var dao = new SqlServerDAO();
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion

        // Act
        timer.Start();
        response = await logService.CreateLogAsync("Info", "View", "This is an Async test message", userHash);
        timer.Stop();
        
        #region Deleting the test user
        sql = "DELETE FROM UserAccount WHERE UserName = 'LoggingTestUser'";
        sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion
        
        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        if (response.ReturnValue is not null)
        {
            Assert.Equal(expectedReturnValue, response.ReturnValue.First());
        }
        else
        {
            Assert.Fail("response.ReturnValue should not be null");
        }
    }

    [Fact]
    public void LL_Log_CreateAndStoreMultipleLog_TwentyLogsWithDifferentLevelAndCategoryCombinationsWillBeStoredToDataStore_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()), new HashService());
        var responseList = new List<IResponse>();

        // Expected values
        var expectedHasError = false;
        string? expectedErrorMessage = null;
        var expectedNumberOfResponses = 20;
        var expectedReturnValue = 1;

        #region Generating a test user
        var userHash = GenerateRandomHash();
        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" + $"VALUES ('LoggingTestUser', 123456, '{userHash}')";
        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        var dao = new SqlServerDAO();
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion


        // 
        timer.Start();
        responseList.Add(logService.CreateLog("Info", "View", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Info", "Business", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Info", "Server", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Info", "Data", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Info", "Data Store", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Debug", "View", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Debug", "Business", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Debug", "Server", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Debug", "Data", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Debug", "Data Store", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Warning", "View", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Warning", "Business", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Warning", "Server", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Warning", "Data", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Warning", "Data Store", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Error", "View", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Error", "Business", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Error", "Server", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Error", "Data", "This tests all level/category combinations", userHash));
        responseList.Add(logService.CreateLog("Error", "Data Store", "This tests all level/category combinations", userHash));
        timer.Stop();

        #region Deleting the test user
        sql = "DELETE FROM UserAccount WHERE UserName = 'LoggingTestUser'";
        sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>() { KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null) };
        dao.ExecuteWriteOnly(sqlCommands);
        #endregion

        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        foreach (var response in responseList)
        {
            Assert.True(response.HasError == expectedHasError);
            Assert.True(response.ErrorMessage == expectedErrorMessage);
            if (response.ReturnValue is not null)
            {
                Assert.Equal(expectedReturnValue, response.ReturnValue.First());
            }
            else
            {
                Assert.Fail("response.ReturnValue should not be null");
            }
        }
        Assert.True(responseList.Count == expectedNumberOfResponses);
    }

    [Fact]
    public void LL_Log_CreateLogWithoutUserHash_LogWillBeWrittenToDataStore_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()), new HashService());

        // Expected values
        var expectedHasError = false;
        string? expectedErrorMessage = null;
        var expectedReturnValue = 1;

        // Act
        timer.Start();
        response = logService.CreateLog("Info", "View", "This is a test message but we don't know who created it (btw it was the admin)");
        timer.Stop();

        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        if (response.ReturnValue is not null)
        {
            Assert.Equal(expectedReturnValue, response.ReturnValue.First());
        }
        else
        {
            Assert.Fail("response.ReturnValue should not be null");
        }
    }
}
