namespace TeamSpecs.RideAlong.UserAdministration;

public class ProfileUserModel : IProfileUserModel
{
    public DateTime DateOfBirth { get; set; }

    public string SecondaryEmail { get; set; }

    public ProfileUserModel(DateTime dateOfBirth, string secondaryEmail)
    {
        DateOfBirth = dateOfBirth;
        SecondaryEmail= secondaryEmail;
    }
}
