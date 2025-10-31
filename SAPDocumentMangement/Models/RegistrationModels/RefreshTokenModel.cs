using System.ComponentModel.DataAnnotations;

namespace SAPDocumentMangement.Models.AccountRegistration
{
    public class RefreshTokenModel
    {
        [Required]
        public string Refresh_Toekn { get; set; }
    }
}
