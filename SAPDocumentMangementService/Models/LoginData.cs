namespace SAPDocumenMangementService.Models
{
    public class LoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginDataClient
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
    }
}
