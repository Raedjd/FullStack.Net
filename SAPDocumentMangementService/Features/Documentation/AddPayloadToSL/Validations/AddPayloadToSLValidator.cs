using FluentValidation;
using SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Contracts;

namespace SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Validations
{
    public class AddPayloadToSLValidator : AbstractValidator<AddPayloadToSLRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddPayloadToSLValidator"/> class.
        /// </summary>
        public AddPayloadToSLValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
