using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public interface IMailKitService
{
    IResponse SendEmail(string emailaddrs, string title, string body);
}
