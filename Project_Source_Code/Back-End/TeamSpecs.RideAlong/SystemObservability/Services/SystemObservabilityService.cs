using TeamSpecs.RideAlong.LoggingLibrary;
using TeamSpecs.RideAlong.Model;

namespace TeamSpecs.RideAlong.SystemObservability;

public class SystemObservabilityService : ISystemObservabilityService
{
    private readonly ISystemObservabilityTarget _systemTarget;
    private readonly ILogService _logService;

    public SystemObservabilityService(ISystemObservabilityTarget systemTarget, ILogService logService)
    {
        _systemTarget = systemTarget;
        _logService = logService;
    }
    
    /**
     * Works as intended, but is not tested
     */
    public IResponse GetAccountCreationAttempts(int timeFrame)
    {
        #region Validate Parameters
        if (timeFrame < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }
        #endregion

        var response = _systemTarget.GetAccountCreationAttemptsSql(timeFrame);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve KPI. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of KPI. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, null);
        #endregion

        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetAllLogs(int timeFrame)
    {
        #region Validate Parameters
        if (timeFrame < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }
        #endregion

        var response = _systemTarget.GetLogsSql(timeFrame);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve Logs. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of Logs. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, null);
        #endregion

        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetLoginAttempts(int timeFrame)
    {
        #region Validate Parameters
        if (timeFrame < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }
        #endregion

        var response = _systemTarget.GetLoginAttemptsSql(timeFrame);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve KPI. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of KPI. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, null);
        #endregion

        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetTopLongestVisitedViews(int numOfResults, int timeFrame)
    {
        #region Validate Parameters
        if (timeFrame < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }
        if (numOfResults < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numOfResults));
        }
        #endregion

        var response = _systemTarget.GetLongestVisitedViewsSql(numOfResults, timeFrame);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve KPI. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of KPI. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, null);
        #endregion

        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetTopMostVisitedViews(int numOfResults, int timeFrame)
    {
        #region Validate Parameters
        if (timeFrame < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }
        if (numOfResults < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numOfResults));
        }
        #endregion

        var response = _systemTarget.GetMostVisitedViewsSql(numOfResults, timeFrame);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve KPI. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of KPI. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, null);
        #endregion

        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetTopRegisteredVehicles(int numOfResults, int timeFrame)
    {
        #region Validate Parameters
        if (timeFrame < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }
        if (numOfResults < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numOfResults));
        }
        #endregion

        var response = _systemTarget.GetMostRegisteredVehiclesSql(numOfResults, timeFrame);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve KPI. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of KPI. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, null);
        #endregion

        return response;
    }

    /**
     * Works as intended, but is not tested
     */
    public IResponse GetVehicleCreationAttempts(int timeFrame)
    {
        #region Validate Parameters
        if (timeFrame < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeFrame));
        }
        #endregion

        var response = _systemTarget.GetVehicleCreationAttemptsSql(timeFrame);

        #region Log the action to the database
        if (response.HasError)
        {
            response.ErrorMessage = "Could not retrieve KPI. " + response.ErrorMessage;
        }
        else
        {
            response.ErrorMessage = "Successful retrieval of KPI. ";
        }
        _logService.CreateLogAsync(response.HasError ? "Error" : "Info", "Server", response.ErrorMessage, null);
        #endregion

        return response;
    }
}
