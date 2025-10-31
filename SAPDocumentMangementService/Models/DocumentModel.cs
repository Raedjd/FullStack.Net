using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SAPDocumenMangementService.MongoDB;
namespace SAPDocumenMangementService.Models
{
    [CollectionName("edi_orders")]
    public class DocumentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; }

        [BsonElement("filename")]
        public string? Filename { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("messages")]
        public List<MessageItem?> Messages { get; set; }

        [BsonElement("payload")]
        public Payload? Payload { get; set; }
    }

    public class MessageItem
    {
        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("docNum")]
        public int? DocNum { get; set; }

        [BsonElement("docEntry")]
        public int? DocEntry { get; set; }

        [BsonElement("message")]
        public string? Message { get; set; }
        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Timestamp { get; set; }
    }

    public class Payload
    {
        [BsonElement("CardCode")]
        public string? CardCode { get; set; }

        [BsonElement("NumAtCard")]
        public string? NumAtCard { get; set; }

        [BsonElement("PayToCode")]
        public string? PayToCode { get; set; }

        [BsonElement("DocDueDate")]
        public string? DocDueDate { get; set; }

        [BsonElement("ShipToCode")]
        public string? ShipToCode { get; set; }

        [BsonElement("U_Type_Doc")]
        public string? U_Type_Doc { get; set; }

        [BsonElement("DocumentLines")]
        public List<DocumentLine?> DocumentLines { get; set; }

        [BsonElement("AddressExtension")]
        public AddressExtension? AddressExtension { get; set; }

        [BsonElement("U_IBS_EDI_GLN_BY")]
        public string? U_IBS_EDI_GLN_BY { get; set; }

        [BsonElement("U_IBS_EDI_GLN_DP")]
        public string? U_IBS_EDI_GLN_DP { get; set; }

        [BsonElement("U_IBS_EDI_GLN_PAY")]
        public string? U_IBS_EDI_GLN_PAY { get; set; }

        [BsonElement("U_IBS_EDI_FILENAME")]
        public string? U_IBS_EDI_FILENAME { get; set; }

        [BsonElement("U_IBS_DOC_NUM_EDI_ORDERS")]
        public string? U_IBS_DOC_NUM_EDI_ORDERS { get; set; }
    }

    public class DocumentLine
    {
        [BsonElement("FreeText")]
        public string? FreeText { get; set; }

        [BsonElement("ItemCode")]
        public string? ItemCode { get; set; }

        [BsonElement("Quantity")]
        public int? Quantity { get; set; }

        [BsonElement("ShipDate")]
        public string? ShipDate { get; set; }

        [BsonElement("U_IBS_EDI_LINE_POS")]
        public string? U_IBS_EDI_LINE_POS { get; set; }
    }

    public class AddressExtension
    {
        [BsonElement("ShipToCity")]
        public string? ShipToCity { get; set; }

        [BsonElement("ShipToBlock")]
        public string? ShipToBlock { get; set; }

        [BsonElement("ShipToState")]
        public string? ShipToState { get; set; }

        [BsonElement("ShipToCounty")]
        public string? ShipToCounty { get; set; }

        [BsonElement("ShipToStreet")]
        public string? ShipToStreet { get; set; }

        [BsonElement("ShipToCountry")]
        public string? ShipToCountry { get; set; }

        [BsonElement("ShipToZipCode")]
        public string? ShipToZipCode { get; set; }

        [BsonElement("ShipToBuilding")]
        public string? ShipToBuilding { get; set; }

        [BsonElement("ShipToStreetNo")]
        public string? ShipToStreetNo { get; set; }

        [BsonElement("ShipToAddressType")]
        public string? ShipToAddressType { get; set; }
    }
}
