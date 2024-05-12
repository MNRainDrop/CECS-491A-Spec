namespace TeamSpecs.RideAlong.Model.ServiceLogModel
{
    public interface IServiceLogModel
    {
        int? ServiceLogID { get; set; }

        string Category { get; set; }
        string Part { get; set; }
        DateTime Date { get; set; }
        string Description { get; set; }
        int? Mileage { get; set; }
        string? VIN { get; set; }
    }
}
