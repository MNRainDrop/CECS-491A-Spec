namespace TeamSpecs.RideAlong.UserAdministration;

public class ProfileUserModel : IProfileUserModel
{
    public DateTime DateOfBirth { get; set; }

    public string AlternateUserName { get; set; }

    public ProfileUserModel(DateTime dateOfBirth, string alternateUserName)
    {
        DateOfBirth = dateOfBirth;
        AlternateUserName = alternateUserName;
    }
}
