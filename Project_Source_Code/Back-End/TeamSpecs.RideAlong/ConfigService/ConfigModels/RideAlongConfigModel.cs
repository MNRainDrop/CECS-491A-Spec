using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class RideAlongConfigModel
{
    [JsonPropertyName("ConnectionStrings")]
    public required ConnectionStrings CONNECTION_STRINGS { get; set; }

    [JsonPropertyName("EmailServiceLogin")]
    public required EmailServiceLogin EMAIL_SERVICE_LOGIN { get; set; }

    [JsonPropertyName("VehicleProfileManager")]
    public required VehicleProfileManager VEHICLE_PROFILE_MANAGER { get; set; }

    [JsonPropertyName("RentalFleetManager")]
    public required RentalFleetManager RENTAL_FLEET_MANAGER { get; set; }

    [JsonPropertyName("InventoryManager")]
    public required InventoryManager INVENTORY_MANAGER { get; set; }
}