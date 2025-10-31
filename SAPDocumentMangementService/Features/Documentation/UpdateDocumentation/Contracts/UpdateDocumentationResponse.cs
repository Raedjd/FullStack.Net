using SAPDocumenMangementService.Models;

namespace SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Contracts
{
    /// <summary>
    /// Response for updating documentation.
    /// </summary>
    public class UpdateDocumentationResponse
    {
        public DocumentModel DocumentModel { get; set; }
    }
}
