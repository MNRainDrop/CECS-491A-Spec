namespace TeamSpecs.RideAlong.Model;

public class VehicleMMYCount
{
    public int Count { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }

    public VehicleMMYCount(int count, string make, string model, int year)
    {
        Count = count;
        Make = make;
        Model = model;
        Year = year;
    }

}
