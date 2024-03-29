using System.Security.Cryptography;
using System.Text;

namespace TeamSpecs.RideAlong.Services;

public class HashService : IHashService
{
    public string hashPass(int salt, int pepper, string pass)
    {
        byte[] saltAndPepper = BitConverter.GetBytes(salt).Concat(BitConverter.GetBytes(pepper)).ToArray();
        string passHash = string.Empty;
        using (Rfc2898DeriveBytes Pbkdf2 = new Rfc2898DeriveBytes((string)pass, saltAndPepper, 10000, HashAlgorithmName.SHA256))
        {
            byte[] hashedUser = Pbkdf2.GetBytes(32); // 32 bytes = 256 bits
            passHash = BitConverter.ToString(hashedUser).Replace("-", string.Empty);
        }
        return passHash;
    }

    public string hashUser(string userName, int pepper)
    {
        byte[] pepperBytes = BitConverter.GetBytes(pepper);
        byte[] userBytes = Encoding.UTF8.GetBytes(userName);
        byte[] pepperedUserbytes = userBytes.Concat(pepperBytes).ToArray();
        string userHash = string.Empty;

        using (SHA256 hasher = SHA256.Create())
        {
            byte[] hash = hasher.ComputeHash(pepperedUserbytes);
            userHash = BitConverter.ToString(hash).Replace("-",string.Empty);
        }
        return userHash;
    }
}
