using LanguageExt.Common;
using MediatR;
using MongoDB.Driver;
using SAPDocumenMangementService.Features.Statistic.GetStatistics.Queries;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.MongoDB;

namespace SAPDocumenMangementService.Features.Statistic.GetStatistics.Handlers
{
    public class GetStatisticsHandler : IRequestHandler<GetStatisticsQuery, Result<StatisticModel>>
    {
        private readonly ILogger<GetStatisticsHandler> _logger;
        private readonly IMongoDatabase _database;

        public GetStatisticsHandler(ILogger<GetStatisticsHandler> logger, IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        public async Task<Result<StatisticModel>> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var collection = MongoCollectionBuilder.GetMongoCollection<DocumentModel>(_database);

                request.QueryParams.TryGetValue("startDateFilter", out var startDateFilter);
                request.QueryParams.TryGetValue("endDateFilter", out var endDateFilter);

                string? startDateFilterString = startDateFilter is null ? null : startDateFilter.ToString();
                string? endDateFilterString = endDateFilter is null ? null : endDateFilter.ToString();

                var filterBuilder = Builders<DocumentModel>.Filter;
                FilterDefinition<DocumentModel> combinedFilter = filterBuilder.Empty;
              

                if (!string.IsNullOrWhiteSpace(startDateFilterString) && DateTime.TryParse(startDateFilterString, out DateTime startDate))
                {
                    startDate = startDate.Date;

                    if (!string.IsNullOrWhiteSpace(endDateFilterString) &&
                        DateTime.TryParse(endDateFilterString, out DateTime endDate))
                    {
                        endDate = endDate.Date.AddDays(1).AddTicks(-1);

                        var dateFilter = filterBuilder.And(
                            filterBuilder.Gte(x => x.Timestamp, startDate),
                            filterBuilder.Lte(x => x.Timestamp, endDate)
                        );

                        combinedFilter = filterBuilder.And(combinedFilter, dateFilter);
                    }
                    else
                    {
                        var nextDay = startDate.AddDays(1);
                        var dateFilter = filterBuilder.And(
                            filterBuilder.Gte(x => x.Timestamp, startDate),
                            filterBuilder.Lt(x => x.Timestamp, nextDay)
                        );

                        combinedFilter = filterBuilder.And(combinedFilter, dateFilter);
                    }
                }
                else
                {
                    var today = DateTime.UtcNow; 
                    var sevenDaysAgo = today.AddDays(-7);

                    var dateFilter = filterBuilder.And(
                        filterBuilder.Gte(x => x.Timestamp, sevenDaysAgo),
                        filterBuilder.Lte(x => x.Timestamp, today)
                    );

                    combinedFilter = filterBuilder.And(combinedFilter, dateFilter);

                }


                var documents = await collection.Find(combinedFilter).ToListAsync(cancellationToken);

                var statistics = new StatisticModel
                {
                    TotalDocuments = documents.Count,
                    OpenDocuments = documents.Where(_ => _.Status == 0).Count(),
                    SuccessDocuments = documents.Where(_ => _.Status == 1).Count(),
                    FailureDocuments = documents.Where(_ => _.Status == 2).Count(),

                };

                _logger.LogInformation("statistics retrieved successfully.");
                return statistics;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting statistics.");
                return new Result<StatisticModel>(ex);
            }
        }
    }
}

