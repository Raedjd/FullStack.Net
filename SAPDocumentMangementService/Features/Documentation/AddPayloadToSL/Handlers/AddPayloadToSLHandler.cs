using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using B1SLayer;
using LanguageExt.Common;
using MediatR;
using MongoDB.Driver;
using SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Commands;
using SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Contracts;
using SAPDocumenMangementService.Models;
using SAPDocumenMangementService.MongoDB;


namespace SAPDocumenMangementService.Features.Documentation.AddPayloadToSL.Handlers
{
    public class AddPayloadToSLHandler : IRequestHandler<AddPayloadToSLCommand, Result<AddPayloadToSLResponse>>
    {
        private readonly ILogger<AddPayloadToSLHandler> _logger;
        private readonly IMongoDatabase _database;
        protected readonly SLConnection _serviceLayer;

        public AddPayloadToSLHandler(ILogger<AddPayloadToSLHandler> logger, IMongoDatabase database, SLConnection serviceLayer)
        {
            _logger = logger;
            _database = database;
            _serviceLayer = serviceLayer;
        }

        public async Task<Result<AddPayloadToSLResponse>> Handle(AddPayloadToSLCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var collection = MongoCollectionBuilder.GetMongoCollection<DocumentModel>(_database);
                var filter = Builders<DocumentModel>.Filter.Eq(x => x.Id, request.Id);

                var document = await collection.Find(filter).FirstOrDefaultAsync();

                if (document == null)
                {
                    throw new InvalidOperationException($"Document with ID {request.Id} is not found.");
                }


                var success = await SendOrderToServiceLayerAsync(document.Payload,request.Id);

                var result = new AddPayloadToSLResponse()
                {
                    Id = request.Id,
                    IsAccepted = success,
                };
                return result;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding payload");
                return new Result<AddPayloadToSLResponse>(ex);
            }

        }
        public async Task<bool> SendOrderToServiceLayerAsync(Payload payload,string Id)
        {
            try
            {
                
                var payloadSended = BuildPayload(payload);

                var result = await PostData(payloadSended, Id);

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual IDictionary<string, object> BuildPayload(Payload payload)
        {
            var createPayload = new ExpandoObject() as IDictionary<string, object>;

            JsonObject jsonObject = JsonSerializer.SerializeToNode(payload)?.AsObject();

            foreach (var property in jsonObject)
            {
                createPayload.Add(property.Key, property.Value);
            }

            return createPayload;
        }


        public virtual async Task<bool>  PostData(IDictionary<string, object> payload,string Id)
        {
            try
            {
                var result = await _serviceLayer.Request($"Orders").PostAsync<object>(payload);
                var msg = "Document Loaded Successfully";
                var json = (JsonElement)result;
                 var docEntry = json.GetProperty("DocEntry").GetInt32();
                var docNum = json.GetProperty("DocNum").GetInt32();

                await AddMessageToDocumentAsync(Id, 1,1,msg, docEntry, docNum);
                return true;
            }
            catch (Exception ex)
            {
                await AddMessageToDocumentAsync(Id, 2, 2, ex.Message,null,null);
                return false;
            }
        }

        public async Task AddMessageToDocumentAsync(string id, int statusMsg, int statusDoc, string message, int? docEntry, int? docNum)
        {
            var collection = MongoCollectionBuilder.GetMongoCollection<DocumentModel>(_database);
            var filter = Builders<DocumentModel>.Filter.Eq(x => x.Id, id);

            var document = await collection.Find(filter).FirstOrDefaultAsync();
            if (document == null)
            {
                throw new InvalidOperationException($"Document with ID {id} is not found.");
            }

            var newMessage = new MessageItem
            {
                Status = statusMsg,
                DocNum = docNum,
                DocEntry = docEntry,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            if (document.Messages == null)
                document.Messages = new List<MessageItem>();

            document.Messages.Add(newMessage);

            var update = Builders<DocumentModel>.Update.Set(x => x.Messages, document.Messages).Set(x => x.Status, statusDoc);
            await collection.UpdateOneAsync(filter, update);
        }

    }
}