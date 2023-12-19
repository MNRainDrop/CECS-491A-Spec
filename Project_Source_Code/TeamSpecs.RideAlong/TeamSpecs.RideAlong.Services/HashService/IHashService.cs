namespace TeamSpecs.RideAlong.Services;

public interface IHashService
{
    string hashUser(string userName, int pepper);
    string hashPass(int salt, int pepper, string pass);
}
