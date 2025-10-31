using LanguageExt.Common;
using MediatR;
using SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Contracts;

namespace SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Commands
{
    public class AddPayloadToSLCommand : IRequest<Result<AddPayloadToSLResponse>>
    {
        public string Id { get; set; }
    }
}
