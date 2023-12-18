using System.Security.Cryptography;

namespace TeamSpecs.RideAlong.Services;

public class RandomService : IRandomService
{
    public static uint GenerateUnsignedInt()
    {
        return (uint) RandomNumberGenerator.GetInt32(Int32.MaxValue);
    }

    public static int GenerateSignedInt()
    {
        return RandomNumberGenerator.GetInt32(Int32.MaxValue);
    }

    public string GenerateRandomString(int size, bool lowercase)
    {
        //This method does not work since it only support .NET 8
        // var resutlString = RandomNumberGenerator.GetString(size, lowercase);
        // return resutlString;
        return "";
    }
}
