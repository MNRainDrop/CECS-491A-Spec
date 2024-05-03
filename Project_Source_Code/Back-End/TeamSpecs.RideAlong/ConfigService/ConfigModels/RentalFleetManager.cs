using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class RentalFleetManager
{
    [JsonPropertyName("numOfResults")]
    public required int NUMOFRESULTS { get; set; }
    [JsonPropertyName("maxOwnedCars")]
    public required int MAXOWNEDCARS { get; set; }

    [SetsRequiredMembers]
    public RentalFleetManager(int numOfResults, int maxOwnedCars)
    {
        NUMOFRESULTS = numOfResults;
        MAXOWNEDCARS = maxOwnedCars;
    }
}
