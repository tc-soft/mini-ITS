namespace mini_ITS.Core.Database
{
    public abstract class SqlPagedResultBase
    {
        public int Page { get; set; }
        public int ResultsPerPage { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }       
    }
}