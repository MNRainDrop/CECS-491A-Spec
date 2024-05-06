namespace ScrapYourCarLibrary.Models
{
    public interface ISearchParameters
    {
        string? name { get; set; }
        string? make { get; set; }
        string? model { get; set; }
        int? year { get; set; }
    }
}
