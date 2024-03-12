namespace TeamSpecs.RideAlong.Model;

public class ProfileUserModel : IProfileUserModel
{
    public DateTime DateOfBirth { get; set; }

    public DateTime DateCreated { get; set; }

    public ProfileUserModel(DateTime dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
        DateCreated = DateTime.Now;
    }
}
