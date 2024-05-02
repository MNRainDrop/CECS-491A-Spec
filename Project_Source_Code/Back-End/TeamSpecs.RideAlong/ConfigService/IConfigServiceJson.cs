using TeamSpecs.RideAlong.ConfigService.ConfigModels;

namespace TeamSpecs.RideAlong.ConfigService;

public interface IConfigServiceJson
{
    RideAlongConfigModel GetConfig();
}
