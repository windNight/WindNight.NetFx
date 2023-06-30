namespace WindNight.Core.SQL.Abstractions
{
 
    public interface IQueryPageBase
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}