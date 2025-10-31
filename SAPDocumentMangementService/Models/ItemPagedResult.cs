namespace SAPDocumenMangementService.Models
{
    public class ItemPagedResult<T>
    {
        public List<T> Items { get; set; }
        public long TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
