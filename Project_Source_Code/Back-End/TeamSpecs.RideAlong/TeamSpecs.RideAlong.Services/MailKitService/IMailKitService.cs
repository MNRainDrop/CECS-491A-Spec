using TeamSpecs.RideAlong.Model;
using MimeKit;
namespace TeamSpecs.RideAlong.Services;

public interface IMailKitService
{
    IResponse SendEmail(string emailaddrs, string title, string body);

    IResponse SendEmailwithAttachments(string emailaddrs, string title,string body, string path);

}
