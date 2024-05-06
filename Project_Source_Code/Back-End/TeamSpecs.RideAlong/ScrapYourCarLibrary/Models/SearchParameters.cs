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
        public string? name { get; set; }
        public string? make { get; set; }
        public string? model { get; set; }
        public int? year { get; set; }
    }
}
