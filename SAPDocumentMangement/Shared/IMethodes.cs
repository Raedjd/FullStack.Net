using MudBlazor;

namespace SAPDocumentMangement.Shared
{
    public interface IMethodes
    {
        Task SnackSuccess(string message);
        Task SnackError(string message);
        Task<bool> DeleteDialog(string itemName);
        Task<bool> WarningDialog(string itemName, string action);     
        Task<string> BuildFilterExpressions<T>(GridState<T> state);
    }
}
