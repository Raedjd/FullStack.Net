namespace SAPDocumentMangement.Models.AccountRegistration
{
    public class LoginResponseModel
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
        public int Refresh_expires_in { get; set; }
        public string Refresh_token { get; set; }
        public string Token_type { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public LoginResponseModel() { }

        public LoginResponseModel(Exception ex)
        {
            IsSuccess = false;
            Message = ex.Message;
        }
    }
}
