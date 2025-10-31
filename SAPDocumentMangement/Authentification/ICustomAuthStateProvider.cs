using SAPDocumentMangement.Models.AccountRegistration;

namespace SAPDocumentMangement.Authentification
{
    public interface ICustomAuthStateProvider
    {
        Task<LoginResponseModel> LogIn(LoginModel signInData);
    }
}
