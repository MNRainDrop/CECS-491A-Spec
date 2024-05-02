using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class RideAlongConfigModel
{
    [JsonPropertyName("ConnectionStrings")]
    public required ConnectionStrings CONNECTION_STRINGS { get; set; }
    [JsonPropertyName("EmailServiceLogin")]
    public required EmailServiceLogin EMAIL_SERVICE_LOGIN { get; set; }
    [JsonPropertyName("NumOfResults")]
    public required NumOfResults RETURN_RESULTS { get; set; }
}