namespace TeamSpecs.RideAlong.Tests;

using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;

public class DataAccessShould
{
    // Create Operations
    [Fact]
    public void DAO_ExecuteWriteOnly_ValidInsertSqlCommandPassedIn_WriteToDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This is a test')");
        
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
    public void DAO_ExecuteWriteOnly_SqlCommandWithNoContextPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand();

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "ExecuteNonQuery: CommandText property has not been initialized";
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
    public void DAO_ExecuteWriteOnly_InsertToInvalidTableSqlCommandPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.NotImplementedTable (something)" +
            "VALUES ('This should not work')");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "Invalid object name 'dbo.NotImplementedTable'.";
        ICollection<object> expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage.Contains(expectedErrorMessage));
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandWithVariableLimitExceededPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSDATETIMEOFFSET(), 'ThisLogLevelHasMoreThanTwentyCharactersAndShouldNotWork', 'ThisLogCategoryHasMoreThanTwentyCharactersAndShouldNotWork', 'ThisLogContextIsUnderOneHundredCharactersButItShouldNotWorkStill')");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "String or binary data would be truncated in table 'RideAlong.dbo.loggingTable', column 'logLevel'. Truncated value: 'ThisLogLevelHasMoreT'.";
        ICollection<object> expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage.Contains(expectedErrorMessage));
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandWithNullValuesPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext) " +
            "VALUES (SYSDATETIMEOFFSET(), null, null, null)");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "Cannot insert the value NULL into column 'logLevel', table 'RideAlong.dbo.loggingTable'; column does not allow nulls. INSERT fails.";
        ICollection<object> expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage.Contains(expectedErrorMessage));
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    
    // Read Operations
    [Fact]
    public void DAO_ExecuteReadOnly_SelectRowFromOneLogIDSqlCommandPassedIn_OneArgumentReturned_Pass()
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
        
        foreach (object[] obj in response.ReturnValue)
        {
            for (int i = 0; i < obj.Length - 1; i++)
            {
                Assert.True(obj[i].Equals(expectedReturnValue[i]));
            }
        }
    }

    [Fact]
    public void DAO_ExecuteReadOnly_SelectRowFromLogLevelSqlCommandPassedIn_ManyArgumentReturned_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logLevel, logCategory, logContext FROM loggingTable WHERE logLevel = 'Info';");

        // Expected 
        var expectedHasError = false;
        string expectedErrorMessage = null;
        Object[] expectedReturnValue = { "Info", "Testing", "This is a test" };

        // Act
        timer.Start();
        response = dao.ExectueReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        foreach (object[] obj in response.ReturnValue)
        {
            for (int i = 0; i < obj.Length - 1; i++)
            {
                Assert.True(obj[i].Equals(expectedReturnValue[i]));
            }
        }
    }

    [Fact]
    public void DAO_ExecuteReadOnly_WriteSqlCommandPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        Response response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This should not work')");

        // Expected 
        var expectedHasError = true;
        string expectedErrorMessage = "The INSERT permission was denied on the object 'loggingTable', database 'RideAlong', schema 'dbo'.";
        ICollection<object> expectedReturnValueAmount = null;

        // Act
        timer.Start();
        response = dao.ExectueReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValueAmount);
    }


    // Update Operations
    

    // Delete Operations
}