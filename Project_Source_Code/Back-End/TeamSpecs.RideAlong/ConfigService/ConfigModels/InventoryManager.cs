using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class InventoryManager
{
    [JsonPropertyName("numOfResults")]
    public required int NUMOFRESULTS { get; set; }
    [JsonPropertyName("maxOwnedCars")]
    public required int MAXOWNEDCARS { get; set; }

    [SetsRequiredMembers]
    public InventoryManager(int numOfResults, int maxOwnedCars)
    {
        NUMOFRESULTS = numOfResults;
        MAXOWNEDCARS = maxOwnedCars;
    }
}
