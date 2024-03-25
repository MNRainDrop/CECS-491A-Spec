namespace TeamSpecs.RideAlong.Model;

public class ViewableDetailsModel : IViewableDetailsModel
{
    public string VIN { get; set; }
    public int Make_isViewable { get; set; } = 0;
    public int Model_isViewable { get; set; } = 0;
    public int Year_isViewable { get; set; } = 0;
    public int Color_isViewable { get; set; } = 0;
    public int Name_isViewable { get; set; } = 0;
    public int VIN_isViewable { get; set; } = 0;

    // Unsure if we will still include photos for vehicles
    public int Photo_isViewable { get; set; } = 0;

    public ViewableDetailsModel(string vin, int makeIsViewable = 0, int modelIsViewable = 0, int yearIsViewable = 0, int colorIsViewable = 0, int nameIsViewable = 0, int vinIsViewable = 0, int photoIsViewable = 0)
    {
        VIN = vin;
        Make_isViewable = makeIsViewable;
        Model_isViewable = modelIsViewable;
        Year_isViewable = yearIsViewable;
        Color_isViewable = colorIsViewable;
        Name_isViewable = nameIsViewable;
        VIN_isViewable = vinIsViewable;
        Photo_isViewable = photoIsViewable;
    }
}
