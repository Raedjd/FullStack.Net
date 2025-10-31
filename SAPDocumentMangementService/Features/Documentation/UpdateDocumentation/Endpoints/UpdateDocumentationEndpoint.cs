using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Commands;
using SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Contracts;
using MVC = Microsoft.AspNetCore.Mvc;

namespace SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Endpoints
{
    /// <summary>
    /// Endpoint for updating documentation.
    /// </summary>
    public class UpdateDocumentationEndpoint : Endpoint<UpdateDocumentationRequest, Results<Ok<UpdateDocumentationResponse>, IResult>>
    {
        private readonly IValidator<UpdateDocumentationRequest> _validator;
        private readonly ISender _sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConfigMailingEndpoint"/> class.
        /// </summary>
        /// <param name="validator">The validator used to validate the <see cref="UpdateConfigMailingRequest"/>.</param>
        /// <param name="sender">The sender used to handle the processing of the request.</param>
        public UpdateDocumentationEndpoint(IValidator<UpdateDocumentationRequest> validator, ISender sender)
        {
            _validator = validator;
            _sender = sender;
        }

        public override void Configure()
        {
            Put("/documents");
            Options(o => o.WithName("UpdateDocumentations"));
            AllowAnonymous();
        }
        public override async Task<Results<Ok<UpdateDocumentationResponse>, IResult>> ExecuteAsync(UpdateDocumentationRequest req, CancellationToken ct)
        {

            var validationResult = await _validator.ValidateAsync(req);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AddError(error);
                }
            }

            ThrowIfAnyErrors();

            var command = req.Adapt<UpdateDocumentationCommand>();

            var result = await _sender.Send(command);

            var response = result.Match<IResult>(
                   /// To be verified Later
                   _ => TypedResults.Ok(_.Adapt<UpdateDocumentationResponse>()),
              failed =>
              {
                  int status = failed switch
                  {
                      ArgumentException => StatusCodes.Status400BadRequest,
                      _ => StatusCodes.Status500InternalServerError
                  };

                  var problemDetails = new MVC.ProblemDetails
                  {
                      Status = status,
                      Title = "An error occurred while update documentation",
                      Type = failed.GetType().Name,
                      Detail = failed.Message
                  };

                  return TypedResults.Problem(problemDetails);

              }
              );

            return response switch
            {
                Ok<UpdateDocumentationResponse> success => success,
                ProblemHttpResult problemDetails => problemDetails,
                _ => throw new Exception("Unknown")
            };
        }
    }
}