namespace TeamSpecs.RideAlong.Tests;

using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

public class DataAccessShould
{
    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandPassedIn_WriteToDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO loggingTable (logTime, logLevel, logCategory, logContext) VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This is a test')");
        
            // Expected values
        var expectedHasError = false;
        string expectedErrorMessage = null;
        var expectedReturnValue = 1;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        Assert.True(response.ReturnValue.FirstOrDefault().Equals(expectedReturnValue));
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO NotImplementedTable (something) VALUES ('This should not work')");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "Invalid object name 'NotImplementedTable'.";
        ICollection<object> expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteReadOnly_SelectOneColumnFromOneItemSqlCommandPassedIn_OneArgumentReturned_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logID, logLevel, logCategory, logContext FROM loggingTable WHERE logID = 1;");

        // Expected 
        var expectedHasError = false;
        string expectedErrorMessage = null;
        var expectedReturnValueAmount = 1;
        Object[] expectedReturnValue = { 1, "Info", "Testing", "This is a test" };

        // Act
        timer.Start();
        response = dao.ExectueReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        Assert.True(response.ReturnValue.Count() == expectedReturnValueAmount);
        Assert.True(response.ReturnValue.First() == expectedReturnValue);
    }
}