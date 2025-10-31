namespace SAPDocumenMangementService.Shared
{
    public class GenericQuery
    {
        public Dictionary<string, string?> QueryParams { get; set; }
        public Dictionary<string, string?> HeaderParams { get; set; }
        public Dictionary<string, object?> RouteParams { get; set; }
    }
}
