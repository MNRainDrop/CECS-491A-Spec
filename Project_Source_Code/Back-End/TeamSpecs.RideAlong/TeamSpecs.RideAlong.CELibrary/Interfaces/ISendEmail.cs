using TeamSpecs.RideAlong.Model;


namespace TeamSpecs.RideAlong.CELibrary.Interfaces;

public interface ISendEmail
{
    IResponse SendEmail(IAccountUserModel accountUser, string vin);
}

