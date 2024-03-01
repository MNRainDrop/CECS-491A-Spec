using System.Collections.Generic;
using System.Linq;
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
            var keyValues = response.ReturnValue as List<object>;

            if (keyValues is not null)
            {
                foreach (var x in keyValues)
                {
                    /*var temp = x as KeyValuePair<string, uint>?;
                    if (temp!=null && temp.Value.Key == key)
                    {
                        return temp.Value.Value;
                    }*/
                    var temp = x as List<KeyValuePair<string, uint>>;
                    if (temp != null)
                    {
                        foreach (var y in temp)
                        {
                            var temp2 = y as KeyValuePair<string, uint>?;
                            if (temp2.Value.Key == key)
                            {
                                return temp2.Value.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Value is not in pepper");
            }
        }
        throw new Exception("Could not retrieve peppers");
    }
}
