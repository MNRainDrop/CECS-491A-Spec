
namespace TeamSpecs.RideAlong.Model.AccountRequestModel
{
    public interface IAccountCreationRequestModel
    {
        string AccountType { get; set; }
        string AltEmail { get; set; }
        DateTime DateOfBirth { get; set; }
        string Email { get; set; }
        string Otp { get; set; }
    }
}