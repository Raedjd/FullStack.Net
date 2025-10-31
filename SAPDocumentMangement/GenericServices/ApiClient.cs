using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using SAPDocumentMangement.Authentification;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using SAPDocumentMangement.Models.AccountRegistration;
using System.Net.Http.Json;
using System.Text;
using System.Net;

namespace SAPDocumentMangement.GenericServices
{
    public class ApiClient(IHttpClientFactory httpClientFactory, ProtectedLocalStorage localStorage, NavigationManager navigationManager, AuthenticationStateProvider authStateProvider, IConfiguration configuration) : IApiClient
    {
        private readonly string origin_url = configuration["API_URL"];
        private readonly HttpClient httpClient = httpClientFactory.CreateClient("sapdocumentmangementservice");
        public async Task SetAuthorizeHeader()
        {
            try
            {
                var sessionState = (await localStorage.GetAsync<LoginResponseModel>("sessionState")).Value;


                if (sessionState != null && !string.IsNullOrEmpty(sessionState.Access_token))
                {
                    if (IsTokenExpired(sessionState.Access_token))
                    {
                        var refreshTokenUrl = $"{origin_url}refresh_token?refreshToken={sessionState.Refresh_token}";
                        var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenUrl);
                        var response = await httpClient.SendAsync(request);


                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                            await ((CustomAuthStateProvider)authStateProvider).MarkUserAsAuthenticated(responseBody);
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseBody.Access_token);

                        }
                        else
                        {
                            await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                            navigationManager.NavigateTo("/login");
                        }
                    }
                    else
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.Access_token);
                    }

                }
            }
            catch (Exception ex)
            {
                navigationManager.NavigateTo("/login");
            }
        }


        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var exp = jwt.Payload.Exp;
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;

            return expirationDate < DateTime.UtcNow;
        }


        public async Task<T> GetFromJsonAsync<T>(string path)
        {
            try
            {
              await SetAuthorizeHeader();
                return await httpClient.GetFromJsonAsync<T>(origin_url + path);
            }
            catch (Exception e)
            {
                return default;
            }
        }
        public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();

            var res = await httpClient.PostAsJsonAsync(origin_url + path, postModel);

            if (res != null && res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
            }
            return default;
        }
        public async Task<T1> PutAsync<T1, T2>(string path, T2 putModel)
        {
            await SetAuthorizeHeader();
            var res = await httpClient.PutAsJsonAsync(origin_url + path, putModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
            }
            return default;
        }

        public async Task<T1> PatchAsync<T1, T2>(string path, T2 patchModel)
        {
            await SetAuthorizeHeader();
            var res = await httpClient.PatchAsJsonAsync(origin_url + path, patchModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
            }
            return default;
        }
        public async Task<T> DeleteAsync<T>(string path, string id)
        {
            try
            {
                await SetAuthorizeHeader();

                var response = await httpClient.DeleteAsync($"{origin_url}{path}/{id}");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<T>();
            }catch(Exception ex) 
            {
                return default;
            }
 
        }
        public async Task<T1> DeleteWithErrorResponseAsync<T1>(string path, string id)
        {
            await SetAuthorizeHeader();

            var response = await httpClient.DeleteAsync($"{origin_url}{path}/{id}");

            if (response != null && response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await response.Content.ReadAsStringAsync());
            }
            return default;

        }

        public async Task<T1> DeleteModelAsync<T1, T2>(string path, T2 deleteModel)
        {
            await SetAuthorizeHeader();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(origin_url + path),
                Content = new StringContent(JsonConvert.SerializeObject(deleteModel), Encoding.UTF8, "application/json")
            };

            var res = await httpClient.SendAsync(request);
            if (res != null && res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
            }
            return default;
        }

        public async Task PatchWithoutResponseAsync<T>(string path, T patchModel)
        {
            await SetAuthorizeHeader();
            var response = await httpClient.PatchAsJsonAsync(origin_url + path, patchModel);

            if (!response.IsSuccessStatusCode)
            {
                // Lire le contenu de la réponse pour diagnostiquer l'erreur
                var errorContent = await response.Content.ReadAsStringAsync();

                // Lever une exception avec les détails (tu peux aussi logger au lieu de throw)
                throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
            }
        }

        public async Task<(bool Success, string Error)> PatchWithoutObjectResponseAsync<T>(string path, T patchModel)
        {
            await SetAuthorizeHeader();
            var response = await httpClient.PatchAsJsonAsync(origin_url + path, patchModel);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, errorContent);
        }
    }
}