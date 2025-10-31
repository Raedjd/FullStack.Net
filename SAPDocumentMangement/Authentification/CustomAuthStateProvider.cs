using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components;
using SAPDocumentMangement.Models.AccountRegistration;
using System.Net.Http;

namespace SAPDocumentMangement.Authentification
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, ICustomAuthStateProvider
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CustomAuthStateProvider> _logger;
        private readonly IConfiguration _configuration;
        private readonly NavigationManager _navigationManager;

        public CustomAuthStateProvider(ProtectedLocalStorage localStorage, IHttpClientFactory httpClientFactory, ILogger<CustomAuthStateProvider> logger, IConfiguration configuration, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _navigationManager = navigationManager;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var sessionModel = (await _localStorage.GetAsync<LoginResponseModel>("sessionState")).Value;


            if (sessionModel == null || string.IsNullOrWhiteSpace(sessionModel.Access_token))
            {
                await MarkUserAsLoggedOut();
               _navigationManager.NavigateTo("/login");
            }
            _navigationManager.NavigateTo("/");
            var identity = sessionModel == null ? new ClaimsIdentity() : GetClaimsIdentity(sessionModel.Access_token);

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task<LoginResponseModel> LogIn(LoginModel logInData)
        {
            try
            {
                _logger.LogInformation("Sending login request...");

                var json = JsonSerializer.Serialize(logInData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var api_url = _configuration["API_URL"];
                var httpClient = _httpClientFactory.CreateClient("sapdocumentmangementservice");

                var request = new HttpRequestMessage(HttpMethod.Post, api_url + "Login-client")
                {
                    Content = content
                };

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                    _logger.LogInformation("Login success.");
                    return responseBody!;
                }
                else
                {
                    _logger.LogWarning("Login failed with status code: {StatusCode}", response.StatusCode);
                    return new LoginResponseModel
                    {
                        IsSuccess = false,
                        Message = $"Login failed: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during login");
                return new LoginResponseModel(ex);
            }
        }


        public async Task MarkUserAsAuthenticated(LoginResponseModel model)
        {
            await _localStorage.SetAsync("sessionState", model);
            var identity = GetClaimsIdentity(model.Access_token);
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = new List<Claim>();
            claims.AddRange(jwtToken.Claims);

            var roleClaims = jwtToken.Claims
                .Where(c => c.Type == "Roles")
                .Select(c => c.Value);

            foreach (var role in roleClaims)
            {
                if (!string.IsNullOrEmpty(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            return new ClaimsIdentity(claims, "jwt");
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.DeleteAsync("sessionState");
            await _localStorage.DeleteAsync("tenant");
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
