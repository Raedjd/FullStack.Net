using LanguageExt.Common;
using MediatR;
using SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Contracts;
using SAPDocumenMangementService.Models;

namespace SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Commands
{
    /// <summary>
    /// Command for updating documentation.
    /// </summary>
    public class UpdateDocumentationCommand : IRequest<Result<UpdateDocumentationResponse>>
    {
        public DocumentModel DocumentModel { get; set; }
    }
}
