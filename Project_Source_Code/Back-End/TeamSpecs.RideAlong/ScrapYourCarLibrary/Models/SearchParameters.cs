namespace ScrapYourCarLibrary.Models
{
    public class SearchParameters : ISearchParameters
    {
        public SearchParameters(int page, int pageSize, string? name = null, string? number = null, string? make = null, string? model = null, int? year = null)
        {
            this.page = page;
            this.pageSize = pageSize;
            this.partName = name;
            this.partNumber = number;
            this.make = make;
            this.model = model;
            this.year = year;
        }
        public int page { get; set; }
        public int pageSize { get; set; }
        public string? partName { get; set; }
        public string? partNumber { get; set; }
        public string? make { get; set; }
        public string? model { get; set; }
        public int? year { get; set; }
    }
}
