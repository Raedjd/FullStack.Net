using LanguageExt.Common;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using SAPDocumenMangementService.Features.Documentation.GetDocumentations.Queries;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.MongoDB;

namespace SAPDocumenMangementService.Features.Documentation.GetDocumentations.Handlers
{
    /// <summary>
    /// Handler for getting documentations.
    /// </summary>
    public class GetDocumentationsHandler : IRequestHandler<GetDocumentationsQuery, Result<ItemPagedResult<DocumentModel>>>
    {
        private readonly ILogger<GetDocumentationsHandler> _logger;
        private readonly IMongoDatabase _database;

        public GetDocumentationsHandler(ILogger<GetDocumentationsHandler> logger, IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        public async Task<Result<ItemPagedResult<DocumentModel>>> Handle(GetDocumentationsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var collection = MongoCollectionBuilder.GetMongoCollection<DocumentModel>(_database);

                request.QueryParams.TryGetValue("page", out var page);
                request.QueryParams.TryGetValue("pageSize", out var pageSize);
                request.QueryParams.TryGetValue("search", out var search);
                request.QueryParams.TryGetValue("sort", out var sort);
                request.QueryParams.TryGetValue("statusFilter", out var statusFilter);
                request.QueryParams.TryGetValue("startDateFilter", out var startDateFilter);
                request.QueryParams.TryGetValue("endDateFilter", out var endDateFilter);


                int? pageSkip = page is null ? null : Convert.ToInt32(page.ToString());
                int? pageSizeTop = pageSize is null ? null : Convert.ToInt32(pageSize.ToString());
                bool sorting = sort is null ? false : Convert.ToBoolean(sort);
                string? searchString = search is null ? null : search.ToString();
                string? statusFilterString = statusFilter is null ? null : statusFilter.ToString();
                string? startDateFilterString = startDateFilter is null ? null : startDateFilter.ToString();
                string? endDateFilterString = endDateFilter is null ? null : endDateFilter.ToString();

                var filterBuilder = Builders<DocumentModel>.Filter;
                FilterDefinition<DocumentModel> combinedFilter = filterBuilder.Empty;

                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    var textFilters = new List<FilterDefinition<DocumentModel>>
                                            {
                                              filterBuilder.Regex(x => x.Filename, new BsonRegularExpression(searchString, "i")),
                                              filterBuilder.Regex(x => x.Payload.CardCode, new BsonRegularExpression(searchString, "i")),
                                            };

                    combinedFilter = filterBuilder.Or(textFilters);
                }

                if (!string.IsNullOrWhiteSpace(statusFilterString))
                {
                    FilterDefinition<DocumentModel> statusFilterDef = statusFilterString switch
                    {
                        "Open" => filterBuilder.Eq(x => x.Status, 0),
                        "Success" => filterBuilder.Eq(x => x.Status, 1),
                        "Failure" => filterBuilder.Eq(x => x.Status, 2),
                        _ => filterBuilder.Empty
                    };

                    combinedFilter = filterBuilder.And(combinedFilter, statusFilterDef);
                }

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

                var pageItem = pageSkip ?? 0;
                var pageSizeItem = pageSizeTop ?? 10;

                var sortDefinition = !sorting ? Builders<DocumentModel>.Sort.Descending(x => x.Timestamp) : Builders<DocumentModel>.Sort.Ascending(x => x.Timestamp);


                var totalCount = await collection.CountDocumentsAsync(combinedFilter, cancellationToken: cancellationToken);


                var documents = await collection.Find(combinedFilter).Sort(sortDefinition).Skip(pageItem * pageSizeItem).Limit(pageSizeItem).ToListAsync(cancellationToken);

                var pagedResult = new ItemPagedResult<DocumentModel>
                {
                    TotalCount = totalCount,
                    Items = documents,

                };

                _logger.LogInformation("Documents retrieved successfully.");
                return pagedResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting documents.");
                return new Result<ItemPagedResult<DocumentModel>>(ex);
            }
        }
    }
}
