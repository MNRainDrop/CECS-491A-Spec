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

        var sql = "INSERT INTO loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This is a test')";
        

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

        var sql = new SqlCommand("SELECT logID, logLevel, logCategory, logContext FROM loggingTable WHERE logID = 1;");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        var expectedReturnValueAmount = 1;
        Object[] expectedReturnValue = { 1, "Info", "Testing", "This is a test" };

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
        
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("SELECT logID, logLevel, logCategory, logContext FROM loggingTable WHERE logLevel LIKE '%Haha Funny Number lololol';");

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

        var sql = new SqlCommand("SELECT logLevel, logCategory, logContext FROM loggingTable WHERE logLevel LIKE '%Info%' and logCategory LIKE '%testing%';");

        // Expected 
        var expectedHasError = false;
        string ?expectedErrorMessage = null;
        Object[] expectedReturnValue = { "Info", "Testing", "" };

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
            if (response.ReturnValue.Count is 0)
            {
                Assert.Fail("response.ReturnValue should not be 0");
            }
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
        
        var dao = new SqlServerDAO();

        var sql = new SqlCommand("INSERT INTO dbo.loggingTable (logTime, logLevel, logCategory, logContext)" +
            "VALUES (SYSUTCDATETIME(), 'Info', 'Testing', 'This should not work')");

        // Expected 
        var expectedHasError = true;
        string expectedErrorMessage = "The INSERT permission was denied on the object 'loggingTable', database 'RideAlong', schema 'dbo'.";
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

        var sql = "UPDATE dbo.loggingTable " +
            "SET logContext = 'This is a test for updating'" +
            "WHERE logID = 112";


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
    public void DAO_ExecuteWriteOnly_ValidUpdateSqlCommandPassedInToReadOnly_OneRowUpdatedToDatabase_Fail()
    {
        // Arrange
        var timer = new Stopwatch();
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

        var sql = "DELETE FROM dbo.loggingTable " +
            "WHERE logID > 2";

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };
        // Expected values
        var expectedReturnValue = 2;

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

        var sql = "DELETE FROM dbo.loggingTable " +
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

        var sql = "DELETE FROM dbo.loggingTable";

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
}