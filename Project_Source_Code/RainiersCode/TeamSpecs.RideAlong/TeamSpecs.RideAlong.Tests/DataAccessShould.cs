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
    public void DAO_ExecuteWriteOnly_ValidInsertSqlCommandPassedIn_OneRowWrittenToDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This is a test')");
        
            // Expected values
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedReturnValue = 1;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
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
    public void DAO_ExecuteWriteOnly_SqlCommandWithNoContextPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand();

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "ExecuteNonQuery: CommandText property has not been initialized";
        ICollection<object> ?expectedReturnValue = null;

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
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.NotImplementedTable (something)" +
            "VALUES ('This should not work')");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "Invalid object name 'dbo.NotImplementedTable'.";
        ICollection<object> ?expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.Contains(expectedErrorMessage, response.ErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandWithVariableLimitExceededPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSDATETIMEOFFSET(), 'ThisLogLevelHasMoreThanTwentyCharactersAndShouldNotWork', 'ThisLogCategoryHasMoreThanTwentyCharactersAndShouldNotWork', 'ThisLogContextIsUnderOneHundredCharactersButItShouldNotWorkStill')");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "String or binary data would be truncated in table 'RideAlong.dbo.loggingTable', column 'logLevel'. Truncated value: 'ThisLogLevelHasMoreT'.";
        ICollection<object> ?expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.Contains(expectedErrorMessage, response.ErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandWithNullValuesPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext) " +
            "VALUES (SYSDATETIMEOFFSET(), null, null, null)");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "Cannot insert the value NULL into column 'logLevel', table 'RideAlong.dbo.loggingTable'; column does not allow nulls. INSERT fails.";
        ICollection<object> ?expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.Contains(expectedErrorMessage, response.ErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    
    // Read Operations
    [Fact]
    public void DAO_ExecuteReadOnly_SelectRowFromOneLogIDSqlCommandPassedIn_OneResultReturned_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logID, logLevel, logCategory, logContext FROM loggingTable WHERE logID = 1;");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
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
        if (response.ReturnValue is not null)
        {
            Assert.True(response.ReturnValue.Count == expectedReturnValueAmount);
        }
        else
        {
            Assert.Fail("response.ReturnValue should not be null");
        }
        
        foreach (object[] obj in response.ReturnValue)
        {
            for (int i = 0; i < obj.Length - 1; i++)
            {
                Assert.True(obj[i].Equals(expectedReturnValue[i]));
            }
        }
    }

    [Fact]
    public void DAO_ExecuteReadOnly_SelectRowFromOneLogIDSqlCommandPassedIn_NoResultsReturned_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logID, logLevel, logCategory, logContext FROM loggingTable WHERE logLevel = 'Haha Funny Number lololol';");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedReturnValueAmount = 0;

        // Act
        timer.Start();
        response = dao.ExectueReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        if (response.ReturnValue is not null)
        {
            Assert.True(response.ReturnValue.Count == expectedReturnValueAmount);
        }
        else
        {
            Assert.Fail("response.ReturnValue should not be null");
        }
    }

    [Fact]
    public void DAO_ExecuteReadOnly_SelectRowFromLogLevelSqlCommandPassedIn_ManyResultsReturned_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logLevel, logCategory, logContext FROM loggingTable WHERE logLevel = 'Info';");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        Object[] expectedReturnValue = { "Info", "Testing", "This is a test" };

        // Act
        timer.Start();
        response = dao.ExectueReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        if (response.ReturnValue is not null)
        {
            foreach (object[] obj in response.ReturnValue)
            {
                for (int i = 0; i < obj.Length - 1; i++)
                {
                    Assert.True(obj[i].Equals(expectedReturnValue[i]));
                }
            }
        }
        else
        {
            Assert.Fail("response.ReturnValue should not be null");
        }
    }

    [Fact]
    public void DAO_ExecuteReadOnly_WriteSqlCommandPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This should not work')");

        // Expected 
        var expectedHasError = true;
        string expectedErrorMessage = "The INSERT permission was denied on the object 'loggingTable', database 'RideAlong', schema 'dbo'.";
        ICollection<object> ?expectedReturnValueAmount = null;

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
    [Fact]
    public void DAO_ExecuteWriteOnly_ValidUpdateSqlCommandPassedIn_OneRowUpdatedToDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("UPDATE dbo.loggingTable " +
            "SET logContext = 'This is a test for updating'" +
            "WHERE logID = 2");

        // Expected values
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedReturnValue = 1;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
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
    public void DAO_ExecuteWriteOnly_ValidUpdateSqlCommandPassedInToReadOnly_OneRowUpdatedToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("UPDATE dbo.loggingTable " +
            "SET logContext = 'This is a test for updating'" +
            "WHERE logID = 2");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "The UPDATE permission was denied on the object 'loggingTable', database 'RideAlong', schema 'dbo'.";
        ICollection<object> ?expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    // Delete Operations
    [Fact]
    public void DAO_ExecuteWriteOnly_ValidDeleteSqlCommandPassedIn_MultipleRowsDeletedInDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("DELETE FROM dbo.loggingTable " +
            "WHERE logID > 2");

        // Expected values
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedMinimumReturnValue = 1;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        if (response.ReturnValue is not null)
        {
            foreach (int items in response.ReturnValue)
            {
                Assert.True(items >= expectedMinimumReturnValue);
            }
        }
        else
        {
            Assert.Fail("response.ReturnValue should not be null");
        }
        
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_ValidDeleteSqlCommandPassedIn_NoRowsDeletedInDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("DELETE FROM dbo.loggingTable " +
            "WHERE logID = null");

        // Expected values
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedReturnValue = 0;

        // Act
        timer.Start();
        response = dao.ExectueWriteOnly(sql);
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
    public void DAO_ExecuteWriteOnly_ValidDeleteAllRowsSqlCommandPassedIn_AllRowsDeletedInDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        IResponse response;
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("DELETE FROM dbo.loggingTable");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "The DELETE permission was denied on the object 'loggingTable', database 'RideAlong', schema 'dbo'.";
        ICollection<object> ?expectedReturnValue = null;

        // Act
        timer.Start();
        response = dao.ExectueReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.Contains(expectedErrorMessage, response.ErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValue);
    }
}