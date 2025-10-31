using LanguageExt.Common;
using MediatR;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.Shared;

namespace SAPDocumenMangementService.Features.Documentation.GetDocumentations.Queries
{
    /// <summary>
    /// Query for getting documentations.
    /// </summary>
    public class GetDocumentationsQuery : GenericQuery, IRequest<Result<ItemPagedResult<DocumentModel>>>
    {
    }
}
