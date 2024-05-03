using System.Text.Json;
using TeamSpecs.RideAlong.ConfigService.ConfigModels;

namespace TeamSpecs.RideAlong.ConfigService;

public class ConfigServiceJson : IConfigServiceJson
{
    public RideAlongConfigModel GetConfig()
    {
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(currentDir, @"..\..\..\..\RideAlong.config.json");
        var fileStream = File.ReadAllText(filePath);

        RideAlongConfigModel configModel = JsonSerializer.Deserialize<RideAlongConfigModel>(fileStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
        return configModel;
    }
}
