namespace ScrapYourCarLibrary.Models
{
    public class SearchParameters : ISearchParameters
    {
        public SearchParameters(string? name = null, string? make = null, string? model = null, int? year = null)
        {
            this.name = name;
            this.make = make;
            this.model = model;
            this.year = year;
        }
        string? name { get; set; }
        string? make { get; set; }
        string? model { get; set; }
        int? year { get; set; }
    }
}
