using SAPDocumenMangementService.Models;

namespace SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Contracts
{
    /// <summary>
    /// Request for updating documentation.
    /// </summary>
    public class UpdateDocumentationRequest
    {
        public DocumentModel DocumentModel { get; set; }
    }
}
