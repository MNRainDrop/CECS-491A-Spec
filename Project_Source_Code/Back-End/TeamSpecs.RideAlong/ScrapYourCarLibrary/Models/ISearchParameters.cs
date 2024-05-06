namespace ScrapYourCarLibrary.Models
{
    public interface ISearchParameters
    {
        int page { get; set; }
        int pageSize { get; set; }
        string? partName { get; set; }
        string? partNumber { get; set; }
        string? make { get; set; }
        string? model { get; set; }
        int? year { get; set; }
    }
}
