namespace TeamSpecs.RideAlong.UserAdministration;

public class ProfileUserModel : IProfileUserModel
{
    public DateTime DateOfBirth { get; set; }

    public ProfileUserModel(DateTime dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }
}
