namespace TeamSpecs.RideAlong.Tests;
using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
public class DataAccessShould
{
    [Fact]
    public void CreateWriteToDataStore()
    {

        var timer = new Stopwatch();
        var dao = new DataAccessObject();
        string[] tables = { "logTime", "logMessage" };
        string[] values = {DateTime.UtcNow.ToString(), "THIS IS A TEST MESSAGE"};

        var initialMessageCount = dao.readFromDataStore("SELECT COUNT(logMessage) FROM [RideAlong].[dbo].[LoggingTestTable] WHERE logMessage = 'THIS IS A TEST MESSAGE';");

        timer.Start();
        var results = dao.writeToDataStore("LoggingTestTable", tables, values);
        timer.Stop();

        var finalMessageCount = dao.readFromDataStore("SELECT COUNT(logMessage) FROM [RideAlong].[dbo].[LoggingTestTable] WHERE logMessage = 'THIS IS A TEST MESSAGE';");

        Assert.True(results.hasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        Assert.True(initialMessageCount == finalMessageCount - 1);
    }
}