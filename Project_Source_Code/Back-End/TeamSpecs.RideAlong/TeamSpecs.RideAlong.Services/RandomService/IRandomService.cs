namespace TeamSpecs.RideAlong.Services;

public interface IRandomService
{
    public  uint GenerateUnsignedInt();
    public  int GenerateSignedInt();
    string GenerateRandomString(int size); 
}
