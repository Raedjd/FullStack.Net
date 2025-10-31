using FastEndpoints;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SAPDocumenMangementService.Features.Statistic.GetStatistics.Queries;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.Shared;
using MVC = Microsoft.AspNetCore.Mvc;

namespace SAPDocumenMangementService.Features.Statistic.GetStatistics.Endpoints
{
    public class GetStatisticsEndpoint : EndpointWithoutRequest<Results<Ok<StatisticModel>, IResult>>
    {
        private readonly ISender _sender;

        public GetStatisticsEndpoint(ISender sender)
        {
            _sender = sender;
        }

        public override void Configure()
        {
            Get("/statistics");
            Version(1);
        }

        public override async Task<Results<Ok<StatisticModel>, IResult>>
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

            var query = req.Adapt<GetStatisticsQuery>();

            var result = await _sender.Send(query);

            var response = result.Match<IResult>(

                   _ => TypedResults.Ok(_.Adapt<StatisticModel>()),
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
                      Title = "An error occurred while Getting the statistics",
                      Type = failed.GetType().Name,
                      Detail = failed.Message
                  };

                  return TypedResults.Problem(problemDetails);

              }
              );

            return response switch
            {
                Ok<StatisticModel> success => success,
                ProblemHttpResult problemDetails => problemDetails,
                _ => throw new Exception("Unknow")
            };

        }
    }
}