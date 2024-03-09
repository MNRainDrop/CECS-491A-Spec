namespace TeamSpecs.RideAlong.TestingLibrary;

using Microsoft.Data.SqlClient;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;


public class DataAccessShould
{
    // Create Operations
    [Fact]
    public void DAO_ExecuteWriteOnly_ValidInsertSqlCommandPassedIn_OneRowWrittenToDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = "INSERT INTO Log (logTime, logLevel, logCategory, logContext, UserHash) " +
          "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This is a test', 'Hash')";



        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };
            // Expected values
        var expectedReturnValue = 1;

        // Act
        timer.Start();
        var response = dao.ExecuteWriteOnly(sqlCommands);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response == expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_SqlCommandWithNoContextPassedIn_WriteToDatabase_Fail()
    {
        // Arrange=
        var dao = new SqlServerDAO();
        var sql = "";
        

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };

        // Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => dao.ExecuteWriteOnly(sqlCommands)
            );
        }
        catch
        {
            Assert.Fail("Should throw Exception");
        }
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertToInvalidTableSqlCommandPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var dao = new SqlServerDAO();
        var sql = "INSERT INTO dbo.NotImplementedTable (something)" +
            "VALUES ('This should not work')";

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };


        // Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => dao.ExecuteWriteOnly(sqlCommands)
            );
        }
        catch
        {
            Assert.Fail("Should throw Exception");
        }        
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandWithVariableLimitExceededPassedIn_WriteToDatabase_Fail()
    {
        // Arrange        
        var dao = new SqlServerDAO();
        var sql = "INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSDATETIMEOFFSET(), 'ThisLogLevelHasMoreThanTwentyCharactersAndShouldNotWork', 'ThisLogCategoryHasMoreThanTwentyCharactersAndShouldNotWork', 'ThisLogContextIsUnderOneHundredCharactersButItShouldNotWorkStill')";

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };

        // Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => dao.ExecuteWriteOnly(sqlCommands)
            );
        }
        catch
        {
            Assert.Fail("Should throw Exception");
        }
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_InsertSqlCommandWithNullValuesPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = "INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext) " +
            "VALUES (SYSDATETIMEOFFSET(), null, null, null)";


        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };

        // Act and Assert
        try
        {
            Assert.ThrowsAny<Exception>(
                () => dao.ExecuteWriteOnly(sqlCommands)
            );
        }
        catch
        {
            Assert.Fail("Should throw Exception");
        }
    }

    
    // Read Operations
    [Fact]
    public void DAO_ExecuteReadOnly_SelectRowFromOneLogIDSqlCommandPassedIn_OneResultReturned_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logID, logLevel, logCategory, logContext FROM Log WHERE logID = 1;");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedReturnValueAmount = 1;
        Object[] expectedReturnValue = { 2, "Info", "Testing", "This is a test" };

        // Act
        timer.Start();
        var response = dao.ExecuteReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);
        foreach (object[] i in response.ReturnValue)
        {
            foreach (var j in i)
            {
                Assert.True(expectedReturnValue.Contains(j));
            }
        }
    }

    [Fact]
    public void DAO_ExecuteReadOnly_SelectRowFromOneLogIDSqlCommandPassedIn_NoResultsReturned_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logID, logLevel, logCategory, logContext FROM Log WHERE logLevel LIKE '%Haha Funny Number lololol';");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedReturnValueAmount = 0;

        // Act
        timer.Start();
        var response = dao.ExecuteReadOnly(sql);
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
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logLevel, logCategory, logContext FROM Log WHERE logLevel LIKE '%Info%' and logCategory LIKE '%testing%';");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        Object[] expectedReturnValue = { "Info", "Testing", "This is a test" };

        // Act
        timer.Start();
        var response = dao.ExecuteReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        Assert.True(response.ErrorMessage == expectedErrorMessage);

        foreach (object[] i in response.ReturnValue)
        {
            foreach (var j in i)
            {
                Assert.True(expectedReturnValue.Contains(j));
            }
        }
    }

    [Fact]
    public void DAO_ExecuteReadOnly_WriteSqlCommandPassedIn_WriteToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO Log (logTime, logLevel, logCategory, logContext, userHash)" +
            "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This should not work', 'hash')");

        // Expected 
        var expectedHasError = true;
        string expectedErrorMessage = "The INSERT permission was denied on the object 'Log', database 'RideAlongTest', schema 'dbo'.";
        ICollection<object> ?expectedReturnValueAmount = null;

        // Act
        timer.Start();
        var response = dao.ExecuteReadOnly(sql);
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
        var dao = new SqlServerDAO();

        var sql = "UPDATE dbo.Log " +
            "SET logContext = 'This is a test for updating'" +
            "WHERE logID = 112";


        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };

        // Expected values
        var expectedReturnValue = 0;

        // Act
        timer.Start();
        var response = dao.ExecuteWriteOnly(sqlCommands);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response >= expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_ValidUpdateSqlCommandPassedInToReadOnly_OneRowUpdatedToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("UPDATE dbo.Log " +
            "SET logContext = 'This is a test for updating'" +
            "WHERE logID = 2");

        // Expected values
        var expectedHasError = true;
        string expectedErrorMessage = "The UPDATE permission was denied on the object 'Log', database 'RideAlongTest', schema 'dbo'.";
        ICollection<object> ?expectedReturnValue = null;

        // Act
        timer.Start();
        var response = dao.ExecuteReadOnly(sql);
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
        
        var dao = new SqlServerDAO();

        var sql = "DELETE FROM dbo.Log " +
            "WHERE logID > 2";

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };
        // Expected values
        var expectedReturnValue = 0;

        // Act
        timer.Start();
        var response = dao.ExecuteWriteOnly(sqlCommands);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response >= expectedReturnValue);
        
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_ValidDeleteSqlCommandPassedIn_NoRowsDeletedInDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = "DELETE FROM dbo.Log " +
            "WHERE logID = null";

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };

        // Expected values
        var expectedReturnValue = 0;

        // Act
        timer.Start();
        var response = dao.ExecuteWriteOnly(sqlCommands);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response == expectedReturnValue);
    }

    [Fact]
    public void DAO_ExecuteWriteOnly_ValidDeleteAllRowsSqlCommandPassedIn_AllRowsDeletedInDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        // Expected values
        var minimumExpectedReturnValue = 0;
        var sql = "DELETE FROM dbo.Log";

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };

        // Act and Assert
        try
        {
            var returnValue = dao.ExecuteWriteOnly(sqlCommands);
            Assert.True(returnValue >= minimumExpectedReturnValue);
        }
        catch
        {
            Assert.Fail("Should throw Exception");
        }
    }
}



