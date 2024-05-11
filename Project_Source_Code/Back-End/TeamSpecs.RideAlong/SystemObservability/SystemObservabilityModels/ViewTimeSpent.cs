namespace TeamSpecs.RideAlong.SystemObservability;

public class ViewTimeSpent
{
    public int TimeInSeconds { get; set; }
    public string Feature { get; set; }
    public ViewTimeSpent(int timeInSeconds, string feature)
    {
        TimeInSeconds = timeInSeconds;
        Feature = feature;
    }
}
