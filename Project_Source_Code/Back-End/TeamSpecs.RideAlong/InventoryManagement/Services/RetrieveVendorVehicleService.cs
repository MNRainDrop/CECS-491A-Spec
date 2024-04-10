using TeamSpecs.RideAlong.Model;
using TeamSpecs.RideAlong.LoggingLibrary;

namespace TeamSpecs.RideAlong.InventoryManagement;

public class RetrieveVendorVehicleService : IRetrieveVendorVehicleService
{
    private readonly IRetrieveVendorVehicleTarget _vehicleTarget;
    private readonly ILogService _logService;

    public RetrieveVendorVehicleService(IRetrieveVendorVehicleTarget vehicleTarget, ILogService logService)
    {
        _vehicleTarget = vehicleTarget;
        _logService = logService;
    }

    public IResponse retrieveVendorVehicles(IAccountUserModel userAccount, int page, int itemsPerPage, ICollection<object>? searchFilters = null)
    {
        #region Validate Parameters
        if (userAccount is null)
        {
            throw new ArgumentNullException(nameof(userAccount));
        }
        if (string.IsNullOrWhiteSpace(userAccount.UserHash))
        {
            throw new ArgumentNullException(nameof(userAccount.UserHash));
        }
        if (searchFilters is not null)
        {
            foreach (var item in searchFilters)
            {
                if (item is null)
                {
                    throw new ArgumentException(nameof(item));
                }
                var itemType = item.GetType();
                if (itemType.IsGenericType)
                {
                    var baseType = itemType.GetGenericTypeDefinition();
                    if (baseType == typeof(KeyValuePair<,>))
                    {
                        Type[] argTypes = itemType.GetGenericArguments();
                        if (argTypes[0] != typeof(string))
                        {
                            throw new ArgumentException(nameof(item));
                        }
                    }
                }
                
            }
        }
        #endregion

        var search = new List<object>()
        {
            new KeyValuePair<string, long>("Owner_UID", userAccount.UserId)
        };

        if (searchFilters is not null)
        {
            foreach (var item in searchFilters)
            {
                search.Add(item);
            }
        }
        
        var response = _vehicleTarget.readVendorVehicleProfilesSql(search, itemsPerPage, page);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve vehicles. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of vehicle profile. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, userAccount.UserHash);
        #endregion
        return response;
    }
}
