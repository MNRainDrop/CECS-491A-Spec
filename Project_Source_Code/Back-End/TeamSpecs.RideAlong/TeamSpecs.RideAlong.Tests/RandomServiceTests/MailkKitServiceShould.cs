using System.Diagnostics;
using TeamSpecs.RideAlong.DataAccess;
using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.Services;

namespace TeamSpecs.RideAlong.TestingLibrary;

public class MailkKitServiceShould
{

    [Fact]
    public void MailKitService_SendEmail_ParametersPassedIn_ReturnValue_Pass()
    {
        //Arrange 
        var timer = new Stopwatch();
        IResponse response;
        MailKitService mailkit = new MailKitService();
        string addr = "baker123dy@gmail.com";
        string title = "Mail test";
        string body = "Hello World!";


        //Act 
        timer.Start();
        response = mailkit.SendEmail(addr,title,body);      
        timer.Stop();

        //Assert 
        Assert.True(timer.Elapsed.TotalSeconds <= 5);
        Assert.True(response.HasError == false);
    }

    [Fact]
    public void MailKitService_SendEmail_NoEmailPassedIn_ReturnValue_Fail()
    {
        //Arrange 
        var timer = new Stopwatch();
        IResponse response;
        MailKitService mailkit = new MailKitService();
        string addr = "";
        string title = "Mail test";
        string body = "Hello World!";


        //Act 
        timer.Start();
        response = mailkit.SendEmail(addr, title, body);
        timer.Stop();

        //Assert 
        Assert.True(timer.Elapsed.TotalSeconds <= 5);
        Assert.True(response.HasError == true);
    }
}

