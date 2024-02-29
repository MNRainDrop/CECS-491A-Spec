using System.Text.Json;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.Services;

public class PepperService : IPepperService
{
    private IPepperTarget _pepperTarget;
    public PepperService(IPepperTarget Target) 
    {
        _pepperTarget = Target; 
    }

    public uint GeneratePepper(string key)
    {
        var pepperValue = RandomService.GenerateUnsignedInt();
        KeyValuePair<string, uint> Pepper = PopulateKeyValue(key, pepperValue);
        SendingPepper(Pepper);
        return pepperValue;
    }

    public KeyValuePair<string, uint> PopulateKeyValue(string key, uint value)
    {
        KeyValuePair<string, uint> pepper = new KeyValuePair<string, uint>(key, value);
        return pepper;
    }

    public object SendingPepper(KeyValuePair<string, uint> PepperObject)
    {
        _pepperTarget.WriteToFile(PepperObject);
        var pepperValue = PepperObject.Value;

        return pepperValue;
    }
    public uint RetrievePepper(string key)
    {
        var response = _pepperTarget.RetrieveFromFile(key);
        if (response.ReturnValue is not null) {
            foreach (uint o in response.ReturnValue)
            {
                return o;
            }
        }
        

        return 0;
    }
}
