namespace TeamSpecs.RideAlong.SystemObservability;

public class ViewClickCount
{
    public int Count { get; set; }
    public string FeatureName { get; set; }

    public ViewClickCount(int count, string featureName)
    {
        Count = count;
        FeatureName = featureName;
    }
}
