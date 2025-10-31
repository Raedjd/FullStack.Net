using LanguageExt.Common;
using MediatR;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.Shared;

namespace SAPDocumenMangementService.Features.Statistic.GetStatistics.Queries
{
    public class GetStatisticsQuery : GenericQuery, IRequest<Result<StatisticModel>>
    {
    }
}
