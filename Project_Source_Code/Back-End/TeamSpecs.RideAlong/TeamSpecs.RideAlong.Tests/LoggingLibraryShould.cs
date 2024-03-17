namespace TeamSpecs.RideAlong.TestingLibrary;

using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

public class LoggingLibraryShould
{
    [Fact]
    public void LL_Log_CreateAndStoreOneLog_LogWillBeStoredToDataStore_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
  
        var _logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()));

        // Expected values
        var expectedHasError = false;
        string? expectedErrorMessage = null;
        var expectedReturnValue = 1;

        // Act
        timer.Start();
        response = _logService.CreateLog("Info", "Warning", "This is a test", "Hash");
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

    [Fact]
    public void LL_Log_CreateAndStoreMultipleLog_TwentyLogsWithDifferentLevelAndCategoryCombinationsWillBeStoredToDataStore_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()));
        var responseList = new List<IResponse>();

        // Expected values
        var expectedHasError = false;
        string? expectedErrorMessage = null;
        var expectedNumberOfResponses = 20;
        var expectedReturnValue = 1;

        // Act
        timer.Start();
        responseList.Add(logService.CreateLog("Info", "View", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Info", "Business", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Info", "Server", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Info", "Data", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Info", "Data Store", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Debug", "View", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Debug", "Business", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Debug", "Server", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Debug", "Data", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Debug", "Data Store", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Warning", "View", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Warning", "Business", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Warning", "Server", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Warning", "Data", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Warning", "Data Store", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Error", "View", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Error", "Business", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Error", "Server", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Error", "Data", "This tests all level/category combinations", "Admin"));
        responseList.Add(logService.CreateLog("Error", "Data Store", "This tests all level/category combinations", "Admin"));
        timer.Stop();

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
        var logService = new LogService(new SqlDbLogTarget(new SqlServerDAO()));

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
