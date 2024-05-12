namespace TeamSpecs.RideAlong.Model;
public interface IUserDataRequestModel
{
    long UserId { get; set; }
    string UserName { get; set; }
    DateTime DoB { get; set; }
    string AltUserName { get; set; }
    DateTime CreationDate { get; set; }
    string Address { get; set; }
    string Name { get; set; }
    string PhoneNumber { get; set; }

}