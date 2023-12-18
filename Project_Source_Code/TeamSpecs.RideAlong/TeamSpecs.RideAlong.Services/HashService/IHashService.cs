namespace TeamSpecs.RideAlong.Services;

public interface IHashService
{
    string hashUser(string userName, uint pepper);
    string hashPass(uint salt, uint pepper, string pass);
}
