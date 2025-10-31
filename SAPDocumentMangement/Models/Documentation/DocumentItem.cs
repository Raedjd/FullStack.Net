namespace SAPDocumentMangement.Models.Documentation
{
 public class DocumentItem
  {
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }

    public string? Filename { get; set; }

    public int Status { get; set; }

    public List<MessageItem?> Messages { get; set; }

    public Payload? Payload { get; set; }
}

public class MessageItem
{
    public int Status { get; set; }

    public int? DocNum { get; set; }

    public int? DocEntry { get; set; }

    public string? Message { get; set; }
    public DateTime? Timestamp { get; set; }
    }

public class Payload
{
    public string? CardCode { get; set; }

    public string? NumAtCard { get; set; }

    public string? PayToCode { get; set; }

    public string? DocDueDate { get; set; }

    public string? ShipToCode { get; set; }

    public string? U_Type_Doc { get; set; }

    public List<DocumentLine?> DocumentLines { get; set; }

    public AddressExtension? AddressExtension { get; set; }

    public string? U_IBS_EDI_GLN_BY { get; set; }

    public string? U_IBS_EDI_GLN_DP { get; set; }

    public string? U_IBS_EDI_GLN_PAY { get; set; }

    public string? U_IBS_EDI_FILENAME { get; set; }

    public string? U_IBS_DOC_NUM_EDI_ORDERS { get; set; }
}

public class DocumentLine
{
    public string? FreeText { get; set; }

    public string? ItemCode { get; set; }

    public int? Quantity { get; set; }

    public string? ShipDate { get; set; }

    public string? U_IBS_EDI_LINE_POS { get; set; }
}

    public class AddressExtension
    {
        public string? ShipToCity { get; set; }
        public string? ShipToBlock { get; set; }
        public string? ShipToState { get; set; }
        public string? ShipToCounty { get; set; }
        public string? ShipToStreet { get; set; }
        public string? ShipToCountry { get; set; }
        public string? ShipToZipCode { get; set; }
        public string? ShipToBuilding { get; set; }
        public string? ShipToStreetNo { get; set; }
        public string? ShipToAddressType { get; set; }
    }

    public class StatisticsItem
    {
        public int TotalDocuments { get; set; }
        public int OpenDocuments { get; set; }
        public int SuccessDocuments { get; set; }
        public int FailureDocuments { get; set; }
    }

    public class AddPayloadToSLResponse
    {
        public string Id { get; set; }
        public bool IsAccepted { get; set; }
    }
    public class AddPayloadToSLRequest
    {
        public string Id { get; set; }
    }
}
