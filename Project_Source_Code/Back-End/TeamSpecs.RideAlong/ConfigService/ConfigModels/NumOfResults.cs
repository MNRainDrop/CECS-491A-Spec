using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class NumOfResults
{
    [JsonPropertyName("vehicleProfile")]
    public required int VEHICLEPROFILE { get; set; }
    [JsonPropertyName("inventoryManagement")]
    public required int INVENTORYMANAGEMENT { get; set; }
    [JsonPropertyName("rentalFleet")]
    public required int RENTALFLEET { get; set; }

    [SetsRequiredMembers]
    public NumOfResults(int vehicleProfile, int inventoryManagement, int rentalFleet)
    {
        VEHICLEPROFILE = vehicleProfile;
        INVENTORYMANAGEMENT = inventoryManagement;
        RENTALFLEET = rentalFleet;
    }
}
