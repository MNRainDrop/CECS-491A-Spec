using System.Diagnostics;
using TeamSpecs.RideAlong.ConfigService;
using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public class SystemObservabilityManager : ISystemObservabilityManager
{
    private readonly ISystemObservabilityService _service;
    private readonly IConfigServiceJson _configService;
    private readonly ILogService _logService;

    private readonly int _numOfResults;
    private readonly int[] _validDateRanges;
    public SystemObservabilityManager(
        ISystemObservabilityService systemObservabilityService,
        IConfigServiceJson configService,
        ILogService logService
    )
    {
        _service = systemObservabilityService;
        _configService = configService;
        _logService = logService;

        _numOfResults = _configService.GetConfig().SYSTEM_OBSERVABILITY.KPIRETURNRESULTS;
        _validDateRanges = _configService.GetConfig().SYSTEM_OBSERVABILITY.VALIDDATERANGES;
}
    public IResponse GetALlKPIs(int dateRange)
    {
        #region Validate Parameters
        if (dateRange < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dateRange));
        }
        if (!Array.Exists(_validDateRanges, element => element == dateRange))
        {
            throw new ArgumentOutOfRangeException(nameof(dateRange));
        }
        #endregion

        #region Call Services
        var response = new Response()
        {
            HasError = false,
            ReturnValue = new List<object>()
        };
        var timer = new Stopwatch();
        timer.Start();
        try
        {
            var loginAttempts = _service.GetLoginAttempts(dateRange);
            response.ReturnValue.Add(new KeyValuePair<string, object?>(nameof(loginAttempts), loginAttempts.ReturnValue));

            var accountCreationAttempts = _service.GetAccountCreationAttempts(dateRange);
            response.ReturnValue.Add(new KeyValuePair<string, object?>(nameof(accountCreationAttempts), accountCreationAttempts.ReturnValue));

            var topLongestViews = _service.GetTopLongestVisitedViews(_numOfResults, dateRange);
            response.ReturnValue.Add(new KeyValuePair<string, object?>(nameof(topLongestViews), topLongestViews.ReturnValue));

            var topVisitedViews = _service.GetTopMostVisitedViews(_numOfResults, dateRange);
            response.ReturnValue.Add(new KeyValuePair<string, object?>(nameof(topVisitedViews), topVisitedViews.ReturnValue));

            var topRegisteredVehicles = _service.GetTopRegisteredVehicles(_numOfResults, dateRange);
            response.ReturnValue.Add(new KeyValuePair<string, object?>(nameof(topRegisteredVehicles), topRegisteredVehicles.ReturnValue));

            var vehicleCreationAttempts = _service.GetVehicleCreationAttempts(dateRange);
            response.ReturnValue.Add(new KeyValuePair<string, object?>(nameof(vehicleCreationAttempts), vehicleCreationAttempts.ReturnValue));
        }
        catch (Exception ex)
        {
            response.ErrorMessage += ex.Message;
            response.HasError = true;
        }
        timer.Stop();

        if (timer.Elapsed.TotalSeconds > 15)
        {
            _logService.CreateLogAsync("Warning", "Server", response.ErrorMessage + "Retrieving KPI logs took longer than 15 seconds.", null);
        }
        #endregion

        #region Log to Database
        if (response.HasError)
        {
            response.ErrorMessage = $"SystemObservability: Failed to retrieve KPIs." + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = $"SystemObservability: Successfully retrieved KPIs.";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Business", response.ErrorMessage, null);
        #endregion

        return response;
    }

    public IResponse GetAllLogs(int dateRange)
    {
        #region Validate Parameters
        if (dateRange < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dateRange));
        }
        #endregion

        #region Call Services
        IResponse response = new Response();
        var timer = new Stopwatch();
        timer.Start();
        try
        {
            var serviceResponse = _service.GetAllLogs(dateRange);
            if (serviceResponse.ReturnValue is not null)
            {
                response = serviceResponse;
            }
        }
        catch (Exception ex)
        {
            response.ErrorMessage += ex.Message;
            response.HasError = true;
        }
        timer.Stop();

        if (timer.Elapsed.TotalSeconds > 15)
        {
            _logService.CreateLogAsync("Warning", "Server", response.ErrorMessage + "Retrieving KPI logs took longer than 15 seconds.", null);
        }
        #endregion

        #region Log to Database
        if (response.HasError)
        {
            response.ErrorMessage = $"SystemObservability: Failed to retrieve KPIs." + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = $"SystemObservability: Successfully retrieved KPIs.";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Business", response.ErrorMessage, null);
        #endregion

        return response;
    }
}
