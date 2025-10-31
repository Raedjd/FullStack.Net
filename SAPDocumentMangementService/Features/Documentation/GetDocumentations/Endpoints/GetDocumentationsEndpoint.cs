using FastEndpoints;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SAPDocumenMangementService.Features.Documentation.GetDocumentations.Queries;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.Shared;
using MVC = Microsoft.AspNetCore.Mvc;

namespace SAPDocumenMangementService.Features.Documentation.GetDocumentations.Endpoints
{
    /// <summary>
    /// Endpoint for getting documentations.
    /// </summary>

    public class GetDocumentationsEndpoint : EndpointWithoutRequest<Results<Ok<ItemPagedResult<DocumentModel>>, IResult>>
    {
        private readonly ISender _sender;

        public GetDocumentationsEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Get("/documents");
            Version(1);
        }

        public override async Task<Results<Ok<ItemPagedResult<DocumentModel>>, IResult>>
    ExecuteAsync(CancellationToken ct)
        {
            var routeValues = HttpContext.Request.RouteValues;

            var queryParams = HttpContext.Request.Query.ToDictionary(k => k.Key, v => v.Value.FirstOrDefault());

            var headerParams = HttpContext.Request.Headers.ToDictionary(k => k.Key, v => v.Value.FirstOrDefault());

            var req = new GenericQuery
            {
                QueryParams = queryParams,
                HeaderParams = headerParams,
                RouteParams = routeValues.ToDictionary()
            };

            //var validationResult = await _contractRequestValidator.ValidateAsync(req);

            //if (!validationResult.IsValid)
            //{

            //}

            var query = req.Adapt<GetDocumentationsQuery>();

            var result = await _sender.Send(query);

            var response = result.Match<IResult>(
                   
                   _ => TypedResults.Ok(_.Adapt<ItemPagedResult<DocumentModel>>()),
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
                      Title = "An error occurred while Getting the documentations",
                      Type = failed.GetType().Name,
                      Detail = failed.Message
                  };

                  return TypedResults.Problem(problemDetails);

              }
              );

            return response switch
            {
                Ok<ItemPagedResult<DocumentModel>> success => success,
                ProblemHttpResult problemDetails => problemDetails,
                _ => throw new Exception("Unknow")
            };

        }
    }
}