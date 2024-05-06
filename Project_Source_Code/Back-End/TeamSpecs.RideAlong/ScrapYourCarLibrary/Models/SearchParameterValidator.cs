namespace TeamSpecs.RideAlong.ScrapYourCarLibrary.Models
{
    public class SearchParameterValidator : ISearchParameterValidator
    {
        public string whereSql { get; set; }
        public int startOnRow { get; set; }
        public int pageSize { get; set; }
        public List<string> rejectedParams { get; set; }
        public SearchParameterValidator(string whereSql, int startOnRow, int pageSize, List<string> rejectedParams)
        {
            this.whereSql = whereSql;
            this.startOnRow = startOnRow;
            this.pageSize = pageSize;
            this.rejectedParams = rejectedParams;
        }
    }
}
