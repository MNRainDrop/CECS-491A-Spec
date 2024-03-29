namespace TeamSpecs.RideAlong.Model;

public class ProfileUserModel : IProfileUserModel
{
    public DateTime DateOfBirth { get; set; }

    public DateTime DateCreated { get; set; }

    public string AlternateUserName { get; set; }
    public ProfileUserModel(DateTime dateOfBirth, string alternateUserName)
    {
        DateOfBirth = dateOfBirth;
        DateCreated = DateTime.Now;
        AlternateUserName = alternateUserName;
    }
}
