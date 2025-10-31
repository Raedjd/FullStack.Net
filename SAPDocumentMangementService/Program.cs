using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Prometheus;
using SAPDocumenMangementService.Extensions;
using SAPDocumenMangementService.Models;
using Serilog;
using System.Diagnostics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .Build();

#region Logging
//ConfigureLogging(configuration, environment);

builder.Services.AddLogging(loggingBuilder =>
        loggingBuilder.AddSerilog(dispose: true));
#endregion

builder.Services.AddCommonServices(configuration);

builder.Services.AddFastEndpoints();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Title = context.Exception == null ? null : context.Exception.Message;

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Audience = builder.Configuration["Authentication:Audience"];
        o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.UseHttpMetrics();

app.UseFastEndpoints(c =>
{
    c.Serializer.Options.PropertyNamingPolicy = null;
    c.Versioning.Prefix = "v";
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.PrependToRoute = true;
    c.Versioning.DefaultVersion = 1;
    c.Endpoints.Configurator = ep =>
    {
        ep.Policy(p => p.RequireAuthenticatedUser());
    };
});

app.MapMetrics();

app.UseStatusCodePages();

app.MapPost("/Login-client", async ([Microsoft.AspNetCore.Mvc.FromBody] LoginData loginData) =>
{
    var client = new HttpClient();
    var keycloakTokenEndpoint = builder.Configuration["keycloak:TokenUrl"];
    var keycloakGrantType = builder.Configuration["keycloak:Grant_type"];
    var keycloakClientId = Environment.GetEnvironmentVariable("ClIENT_ID");
    var keycloakClientSecret = Environment.GetEnvironmentVariable("ClIENT_SECRET");

    var requestContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("client_id", keycloakClientId),
        new KeyValuePair<string, string>("client_secret",keycloakClientSecret),
        new KeyValuePair<string, string>("grant_type", keycloakGrantType),
        new KeyValuePair<string, string>("username", loginData.Username),
        new KeyValuePair<string, string>("password", loginData.Password)
    });

    var request = new HttpRequestMessage(HttpMethod.Post, keycloakTokenEndpoint)
    {
        Content = requestContent
    };

    var response = await client.SendAsync(request);

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();

        return Results.Ok(JsonDocument.Parse(content));
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous();

app.MapPost("/refresh_token", async (HttpContext httpContext) =>
{
    var client = new HttpClient();
    var keycloakTokenEndpoint = builder.Configuration["keycloak:TokenUrl"];
    var keycloakGrantTypeRefreshtoken = builder.Configuration["keycloak:Grant_type_refresh_token"];
    var keycloakClientId = Environment.GetEnvironmentVariable("ClIENT_ID");
    var keycloakClientSecret = Environment.GetEnvironmentVariable("ClIENT_SECRET");
    var refreshToken = httpContext.Request.Query["refreshToken"].ToString();

    var requestContent = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("client_id", keycloakClientId),
        new KeyValuePair<string, string>("client_secret",keycloakClientSecret),
        new KeyValuePair<string, string>("grant_type", keycloakGrantTypeRefreshtoken),
        new KeyValuePair<string, string>("refresh_token", refreshToken),

    });

    var request = new HttpRequestMessage(HttpMethod.Post, keycloakTokenEndpoint)
    {
        Content = requestContent
    };

    var response = await client.SendAsync(request);

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();

        return Results.Ok(JsonDocument.Parse(content));
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous();

app.Run();

//void ConfigureLogging(IConfiguration configuration, string environment)
//{
//    var appName = configuration["APPLICATION_NAME"];
//    var lokiUrl = Environment.GetEnvironmentVariable("LOKI_URL");

//    var outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] ({Application}/{Environment}) {Message:lj}{NewLine}{Exception}";

//    Log.Logger = new LoggerConfiguration()
//        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//        .Enrich.FromLogContext()
//        .Enrich.WithProperty("Environment", environment)
//        .Enrich.WithProperty("Application", appName)
//        .Enrich.WithMachineName()
//        .Filter.ByExcluding(le =>
//            le.Properties.TryGetValue("RequestPath", out var path) &&
//            path.ToString().Contains("/metrics"))
//        .Filter.ByExcluding(le =>
//            le.MessageTemplate.Text.Contains("Prometheus metrics"))
//        .WriteTo.Console(outputTemplate: outputTemplate)
//        .WriteTo.GrafanaLoki(
//            lokiUrl,
//            labels: new[]
//            {
//                new LokiLabel { Key = "app", Value = appName },
//                new LokiLabel { Key = "env", Value = environment },
//                new LokiLabel { Key = "machine", Value = Environment.MachineName }
//            },
//            textFormatter: new MessageTemplateTextFormatter(outputTemplate, null)
//        )
//        .CreateLogger();
//}


