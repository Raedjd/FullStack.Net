using LanguageExt.Common;
using MediatR;
using MongoDB.Driver;
using SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Commands;
using SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Contracts;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.MongoDB;

namespace SAPDocumenMangementService.Features.Documentation.UpdateDocumentation.Handlers
{
    /// <summary>
    /// Handler for updating documentation.
    /// </summary>
    public class UpdateDocumentationHandler : IRequestHandler<UpdateDocumentationCommand, Result<UpdateDocumentationResponse>>
    {
        private readonly ILogger<UpdateDocumentationHandler> _logger;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDocumentationHandler "/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        /// <param name="logger">The logger instance.</param>
        public UpdateDocumentationHandler(ILogger<UpdateDocumentationHandler> logger, IMongoDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        public async Task<Result<UpdateDocumentationResponse>> Handle(UpdateDocumentationCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var collection = MongoCollectionBuilder.GetMongoCollection<DocumentModel>(_database);
                var filter = Builders<DocumentModel>.Filter.Eq(x => x.Id, request.DocumentModel.Id);

                var document = await collection.Find(filter).FirstOrDefaultAsync();

                if (document == null)
                {
                    throw new InvalidOperationException($"Document with ID {request.DocumentModel.Id} is not found.");
                }


                request.DocumentModel.Id = document.Id;

                await collection.ReplaceOneAsync(filter, request.DocumentModel);

                _logger.LogInformation("Document updated successfully.");

                return new UpdateDocumentationResponse
                {
                    DocumentModel = request.DocumentModel,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updated the document.");
                return new Result<UpdateDocumentationResponse>(ex);
            }
        }
    }
}
