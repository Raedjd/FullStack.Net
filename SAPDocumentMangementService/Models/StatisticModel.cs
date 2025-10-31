namespace SAPDocumenMangementService.Models
{
    public class StatisticModel
    {
        public int TotalDocuments { get; set; }
        public int OpenDocuments { get; set; }
        public int SuccessDocuments { get; set; }
        public int FailureDocuments { get; set; }
    }
}
