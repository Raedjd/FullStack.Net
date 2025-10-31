using FluentValidation;
using SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Contracts;

namespace SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Validations
{
    /// <summary>
    /// Validation for updating documentation.
    /// </summary>
    public class UpdateDocumentationValidator : AbstractValidator<UpdateDocumentationRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDocumentationValidator"/> class.
        /// </summary>
        public UpdateDocumentationValidator()
        {
            RuleFor(x => x.DocumentModel)
                   .NotNull().WithMessage("DocumentModel is required.");
        }
    }
}
