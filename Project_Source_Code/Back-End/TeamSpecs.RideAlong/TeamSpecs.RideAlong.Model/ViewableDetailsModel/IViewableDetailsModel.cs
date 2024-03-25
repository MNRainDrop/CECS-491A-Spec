namespace TeamSpecs.RideAlong.Model;
public interface IViewableDetailsModel
{
    string VIN {  get; set; }
    int Make_isViewable { get; set; }
    int Model_isViewable {  get; set; }
    int Year_isViewable { get; set; }
    int Color_isViewable { get; set; }
    int Name_isViewable { get; set; }
    int VIN_isViewable { get; set; }
    
    // Unsure if we will still include photos for vehicles
    int Photo_isViewable { get; set; }
}
