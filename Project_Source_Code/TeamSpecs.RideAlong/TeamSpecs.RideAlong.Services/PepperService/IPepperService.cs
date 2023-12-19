namespace TeamSpecs.RideAlong.Services;

public interface IPepperService
{
    uint GeneratePepper(string key);
    public KeyValuePair<string, uint> PopulateKeyValue(string key, uint value);
    uint RetrievePepper(string key);
}
