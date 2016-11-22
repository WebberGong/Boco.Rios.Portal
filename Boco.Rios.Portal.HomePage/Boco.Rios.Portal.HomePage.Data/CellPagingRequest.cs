namespace Boco.Rios.Portal.HomePage.Data
{
    public class CellPagingRequest
    {
        public string Name { get; set; }
        public int Lac { get; set; }
        public int Ci { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}