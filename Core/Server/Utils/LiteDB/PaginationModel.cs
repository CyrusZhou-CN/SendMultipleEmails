namespace Uamazing.SME.Server.Utils.LiteDB
{
    public class PaginationModel
    {
        public string SortBy { get; set; }
        public bool Descending { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }
}
