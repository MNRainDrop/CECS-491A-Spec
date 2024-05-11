using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TeamSpecs.RideAlong.ConfigService;

public class SystemObservability
{
    [JsonPropertyName("kpiReturnResults")]
    public required int KPIRETURNRESULTS { get; set; }

    [JsonPropertyName("validDateRanges")]
    public required int[] VALIDDATERANGES { get; set; }

    [SetsRequiredMembers]
    public SystemObservability(int kpiReturnResults, int[] vALIDDATERANGES)
    {
        KPIRETURNRESULTS = kpiReturnResults;
        VALIDDATERANGES = vALIDDATERANGES;
    }
}
