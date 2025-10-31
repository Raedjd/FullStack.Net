using Microsoft.AspNetCore.Http;
using SAPDocumentMangement.Components;
using SAPDocumentMangement.GenericServices;
using SAPDocumentMangement.IServices;
using SAPDocumentMangement.Models.Documentation;
using SAPDocumentMangement.Shared;

namespace SAPDocumentMangement.Services
{
    public class DocumentationService(ILogger<DocumentationService> logger, IApiClient ApiClient, IConfiguration configuration) : IDocumentationService
    {
        private readonly ILogger<DocumentationService> _logger = logger;
        private readonly IApiClient _apiClient = ApiClient;

        public async Task<PagedResult<DocumentItem>> GetAllDocumentationWithPaginationAsync(string queryString)
        {
            try
            {
                var documentation = await _apiClient.GetFromJsonAsync<PagedResult<DocumentItem>>("api/v1/documents?"+queryString);

                if (documentation != null)
                {
                    return documentation;
                }

                _logger.LogWarning("Failed to retrieve documentation.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving documentation");
                return null;
            }
        }

        public async Task<StatisticsItem> GetStatisticsAsync(string queryString)
        {
            try
            {
                var statistics = await _apiClient.GetFromJsonAsync<StatisticsItem>("api/v1/statistics?" + queryString);

                if (statistics != null)
                {
                    return statistics;
                }

                _logger.LogWarning("Failed to retrieve statistics.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving statistics");
                return null;
            }
        }

        public async Task<AddPayloadToSLResponse> SendDocumentationAsync(AddPayloadToSLRequest documentItem)
        {
            try
            {
                var documentResponse = await _apiClient.PostAsync<AddPayloadToSLResponse, AddPayloadToSLRequest>("api/v1/payloads", documentItem);

                if (documentResponse != null)
                {
                    return documentResponse;
                }

                _logger.LogWarning("Failed to add documentItem.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while add ocumentItem");
                return null;
            }
        }

        public async Task<DocumentItem> UpdateDocumentationAsync(DocumentItem documentItem)
        {
            try
            {
                var requestPayload = new
                {
                    documentModel = documentItem
                };
                var documentResponse = await _apiClient.PutAsync<DocumentItem, object>("api/v1/documents", requestPayload);

                if (documentResponse != null)
                {
                    return documentResponse;
                }

                _logger.LogWarning("Failed to update documentItem.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while update documentItem");
                return null;
            }
        }
    }
}
