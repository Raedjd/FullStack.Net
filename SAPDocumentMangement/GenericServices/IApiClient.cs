
namespace SAPDocumentMangement.GenericServices
{
    public interface IApiClient
    {
        Task<T> GetFromJsonAsync<T>(string path);

        Task<T1> PostAsync<T1, T2>(string path, T2 postModel);

        Task<T1> PutAsync<T1, T2>(string path, T2 putModel);
        Task<T1> PatchAsync<T1, T2>(string path, T2 patchModel);
        Task PatchWithoutResponseAsync<T>(string path, T patchModel);
        Task<T> DeleteAsync<T>(string path, string id);
       // Task<T1> SoftDeleteAsync<T1, T2>(string path, T2 deleteModel);
        Task<T1> DeleteWithErrorResponseAsync<T1>(string path, string id);

        Task<T1> DeleteModelAsync<T1,T2>(string path, T2 deleteModel);
        Task<(bool Success, string Error)> PatchWithoutObjectResponseAsync<T>(string path, T patchModel);
    }
}
