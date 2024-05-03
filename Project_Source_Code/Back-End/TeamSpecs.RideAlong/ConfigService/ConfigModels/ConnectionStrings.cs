using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class ConnectionStrings
{
    [JsonPropertyName("readOnly")]
    public required string READONLY { get; set; }
    [JsonPropertyName("writeOnly")]
    public required string WRITEONLY { get; set; }
    [JsonPropertyName("admin")]
    public required string ADMIN { get; set; }
    
    [SetsRequiredMembers]
    public ConnectionStrings(string readOnly, string writeOnly, string admin)
    {
        ADMIN = admin;
        READONLY = readOnly;
        WRITEONLY = writeOnly;
    }
}
