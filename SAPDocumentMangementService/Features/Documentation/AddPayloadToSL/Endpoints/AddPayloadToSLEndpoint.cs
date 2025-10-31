using FastEndpoints;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Commands;
using SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Contracts;
using MVC = Microsoft.AspNetCore.Mvc;

namespace SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Endpoints
{
    public class AddPayloadToSLEndpoint : Endpoint<AddPayloadToSLRequest, Results<Created<AddPayloadToSLResponse>, IResult>>
    {
        private readonly IValidator<AddPayloadToSLRequest> _validator;
        private readonly ISender _sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPayloadToSLEndpoint"/> class.
        /// </summary>
        /// <param name="validator">The validator used to validate the <see cref="AddPayloadToSLRequest"/>.</param>
        /// <param name="sender">The sender used to handle the processing of the request.</param>
        /// <remarks>
        /// The constructor requires both a validator to ensure the request is valid and a sender to handle the processing logic.
        /// </remarks>
        public AddPayloadToSLEndpoint(IValidator<AddPayloadToSLRequest> validator, ISender sender)
        {
            _validator = validator;
            _sender = sender;
        }

        public override void Configure()
        {
            Post("/payloads");
            Options(o => o.WithName("AddPayloadToSL"));
            Version(1);
        }
        public override async Task<Results<Created<AddPayloadToSLResponse>, IResult>> ExecuteAsync(AddPayloadToSLRequest req, CancellationToken ct)
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

            var command = req.Adapt<AddPayloadToSLCommand>();

            var result = await _sender.Send(command);

            var response = result.Match<IResult>(
                   /// To be verified Later
                   _ => TypedResults.Created("", _),
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
                      Title = "An error occurred while add payload",
                      Type = failed.GetType().Name,
                      Detail = failed.Message
                  };

                  return TypedResults.Problem(problemDetails);

              }
              );

            return response switch
            {
                Created<AddPayloadToSLResponse> success => success,
                ProblemHttpResult problemDetails => problemDetails,
                _ => throw new Exception("Unknown")
            };
        }
    }
}
