using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService.ConfigModels;

public sealed class EmailServiceLogin
{
    [JsonPropertyName("password")]
    public required string PASSWORD { get; set; }
    [JsonPropertyName("username")]
    public required string EMAIL { get; set; }

    [SetsRequiredMembers]
    public EmailServiceLogin(string password, string email)
    {
        PASSWORD = password;
        EMAIL = email;
    }
}
