
namespace SAPDocumentMangement.IServices
{
    public interface IGenericService
    {
        Task<List<T>?> GetListAsync<T>(string endpoint, string tenantHeaderValue);
        Task<bool> CreateAsync<T>(string endpoint, string tenantHeaderValue, T item);
        Task<bool> UpdateAsync<T>(string endpoint, string tenantHeaderValue, string id, T item);
        Task<bool> DeleteSingleAsync(string endpoint, string tenantHeaderValue, string id);
        Task<bool> DeleteAsync(string endpoint, string tenantHeaderValue, List<string> ids);

    }
}
