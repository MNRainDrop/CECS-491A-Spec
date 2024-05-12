using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class VehicleMarketplaceManager
{
    [JsonPropertyName("numOfResults")]
    public required int NUMOFRESULTS { get; set; }
  
    [SetsRequiredMembers]
    public VehicleMarketplaceManager(int numOfResults)
    {
        NUMOFRESULTS = numOfResults;
    }
}
