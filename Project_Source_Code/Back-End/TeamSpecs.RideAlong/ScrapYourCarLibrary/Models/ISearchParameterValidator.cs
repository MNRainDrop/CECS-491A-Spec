namespace TeamSpecs.RideAlong.ScrapYourCarLibrary.Models
{
    public interface ISearchParameterValidator
    {
        public string whereSql { get; set; }
        public int startOnRow { get; set; }
        public int pageSize { get; set; }
        public List<string> rejectedParams { get; set; }
    }
}
