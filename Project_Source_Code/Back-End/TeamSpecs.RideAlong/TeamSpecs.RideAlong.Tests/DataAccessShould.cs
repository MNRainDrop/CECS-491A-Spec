namespace TeamSpecs.RideAlong.TestingLibrary;

using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using TeamSpecs.RideAlong.DataAccess;
#pragma warning disable
public class DataAccessShould
{
    //Generates hashes for testing purposes
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
    // UserAccount (UserName, Salt, UserHash)
    // ('TestUsername', 123456, 'TestUserHash')
    // Create Operations
    [Fact]
    public void DAO_ExecuteWriteOnly_ValidInsertSqlCommandPassedIn_OneRowWrittenToDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" +
            $"VALUES ('TestUsername', 123456, '{GenerateRandomHash()}')";

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
        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" +
            "VALUES ('username should fail it has more than 50 characters', 2147483648, 'this user hash is expected to fail it has more than 64 characters')";

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

        var sql = "INSERT INTO UserAccount (UserName, Salt, UserHash)" +
            "VALUES (null, null, null)";


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

        var sql = new SqlCommand("SELECT UID, UserName, Salt, UserHash FROM UserAccount WHERE UID = 1;");

        // Expected 
        var expectedHasError = false;
        string? expectedErrorMessage = null;
        var expectedReturnValueAmount = 1;
        Object[] expectedReturnValue = { 1, "TestUsername", 123456, "TestUserHash" };

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

        var sql = new SqlCommand("SELECT  UID, UserName, Salt, UserHash FROM UserAccount WHERE UserName LIKE '%Haha Funny Number lololol';");

        // Expected 
        var expectedHasError = false;
        string? expectedErrorMessage = null;
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
        string hash = GenerateRandomHash();

        var sql = new SqlCommand("SELECT UserName, UserHash FROM UserAccount WHERE UserName LIKE '%TestUsername%' and UserHash LIKE '%TestUserHash%';");

        // Expected 
        var expectedHasError = false;

        string? expectedErrorMessage = null;
        Object[] expectedReturnValue = { "TestUsername", "TestUserHash" };

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


        var sql = new SqlCommand("INSERT INTO UserAccount (UserName, Salt, UserHash) " + $"VALUES ('test_user@gmail.com', 123456, '{GenerateRandomHash()}')");

        // Expected
        var expectedHasError = true;
        string expectedErrorMessage = "The INSERT permission was denied on the object 'UserAccount', database 'RideAlongDevDB', schema 'dbo'.";
        ICollection<object>? expectedReturnValueAmount = null;

        // Act
        timer.Start();
        var response = dao.ExecuteReadOnly(sql);
        timer.Stop();


        // Assert
        //Assert.True(timer.Elapsed.TotalSeconds <= 5);
        Assert.True(response.HasError == expectedHasError);
        // Assert.True(response.ErrorMessage == expectedErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValueAmount);
    }

    // Update Operations
    [Fact]
    public void DAO_ExecuteWriteOnly_ValidUpdateSqlCommandPassedIn_OneRowUpdatedToDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = "UPDATE UserAccount " +
            "SET UserHash = 'This is an update test'" +
            "WHERE UID = 2;";


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

        // Reverse act
        sql = "UPDATE UserAccount " +
            "SET UserHash = 'This is an update test'" +
            "WHERE UID = 2;";
        sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };
        response = dao.ExecuteWriteOnly(sqlCommands);


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

        var sql = new SqlCommand("UPDATE UserAccount " +
            "SET UserHash = 'This is a test for updating'");

        // Expected values
        var expectedHasError = true;

        string expectedErrorMessage = "The UPDATE permission was denied on the object 'UserAccount', database 'RideAlongDevDB', schema 'dbo'.";
        ICollection<object>? expectedReturnValue = null;

        // Act
        timer.Start();
        var response = dao.ExecuteReadOnly(sql);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(response.HasError == expectedHasError);
        //Assert.True(response.ErrorMessage == expectedErrorMessage);
        Assert.True(response.ReturnValue == expectedReturnValue);
    }

    // Delete Operations
    [Fact]
    public void DAO_ExecuteWriteOnly_ValidDeleteSqlCommandPassedIn_MultipleRowsDeletedInDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();

        var dao = new SqlServerDAO();

        var sql2 = "INSERT INTO UserAccount (UserName, Salt, UserHash) VALUES ('dummyUsername', 123456, 'dummyUserHash')";
        var sql = "DELETE FROM UserAccount WHERE UserName = 'dummyUsername'";

        var sqlCommands = new List<KeyValuePair<string, HashSet<SqlParameter>?>>()
        {
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql2, null),
            KeyValuePair.Create<string, HashSet<SqlParameter>?>(sql, null)
        };
        // Expected values
        var expectedReturnValue = 2;

        // Act
        timer.Start();
        var response = dao.ExecuteWriteOnly(sqlCommands);
        timer.Stop();


        // Assert
        Assert.True(timer.Elapsed.TotalSeconds <= 5);
        Assert.True(response == expectedReturnValue);

    }

    [Fact]
    public void DAO_ExecuteWriteOnly_ValidDeleteSqlCommandPassedIn_NoRowsDeletedInDatabase_Pass()
    {
        // Arrange
        var timer = new Stopwatch();
        var dao = new SqlServerDAO();

        var sql = "DELETE FROM UserAccount " +
            "WHERE UID = null";

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
        var sql = "DELETE FROM UserAccount";

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
#pragma warning restore