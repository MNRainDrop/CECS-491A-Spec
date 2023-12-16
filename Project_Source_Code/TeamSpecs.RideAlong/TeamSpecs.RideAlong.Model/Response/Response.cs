namespace TeamSpecs.RideAlong.Model;

public class Response : IResponse
{
    public Response()
    {
        HasError = true;
        ErrorMessage = null;
        ReturnValue = null;
        RetryAttempts = 0;
        IsSafeToRetry = false;
    }

    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }
    public ICollection<object>? ReturnValue { get; set; }
    public int RetryAttempts { get; set; }
    public bool IsSafeToRetry { get; set; }
}