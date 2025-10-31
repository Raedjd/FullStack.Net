using SAPDocumentMangement.Models.Documentation;
using SAPDocumentMangement.Shared;

namespace SAPDocumentMangement.IServices
{
    public interface IDocumentationService
    {
        Task<PagedResult<DocumentItem>> GetAllDocumentationWithPaginationAsync(string queryString);
        Task<DocumentItem> UpdateDocumentationAsync(DocumentItem documentItem);
        Task<AddPayloadToSLResponse> SendDocumentationAsync(AddPayloadToSLRequest documentItem);
        Task<StatisticsItem> GetStatisticsAsync(string queryString);
    }
}
