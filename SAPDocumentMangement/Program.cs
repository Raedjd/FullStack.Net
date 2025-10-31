using SAPDocumentMangement.Authentification;
using SAPDocumentMangement.Components;
using SAPDocumentMangement.GenericServices;
using SAPDocumentMangement.IServices;
using SAPDocumentMangement.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;
using MudBlazor.Services;
using SAPDocumentMangement.Models.Settings;
using SAPDocumentMangement.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddMudServices(options => { options.PopoverOptions.CheckForPopoverProvider = false; });
builder.Services.AddMudServices();

builder.Services.AddAuthentication();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ICustomAuthStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IApiClient,ApiClient>();
builder.Services.AddScoped<IDocumentationService, DocumentationService>();
builder.Services.AddScoped<IMethodes, Methodes>();


builder.Services.Configure<CircuitOptions>(options => options.DetailedErrors = true);

var API_URL = builder.Configuration["API_URL"];

builder.Services.AddHttpClient("sapdocumentmangementservice", client =>
{
    client.BaseAddress = new Uri(API_URL.ToString());
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

builder.Services.AddDataProtection()
.PersistKeysToFileSystem(new DirectoryInfo("/var/data-protection-keys-sap"))
.SetApplicationName("SAPDocmentationMangement");

builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 1024 * 1024 * 10; // 10 MB
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
