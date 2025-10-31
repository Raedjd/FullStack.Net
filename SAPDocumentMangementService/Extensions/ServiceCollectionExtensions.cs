using B1SLayer;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;


namespace SAPDocumenMangementService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
            services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
            #region MongoDB

            MongoClientSettings settings = new MongoClientSettings();
            settings.MinConnectionPoolSize = 1;
            settings.MaxConnectionPoolSize = 25;
            var mongodbUser = Environment.GetEnvironmentVariable("MONGODB_SAP_MANGEMENT_LINK_USER");
            var mongodbPwd = Environment.GetEnvironmentVariable("MONGODB_SAP_MANGEMENT_LINK_PWD");
            var mongodbHost = Environment.GetEnvironmentVariable("MONGODB_HOST");
            var mongodbPort = Environment.GetEnvironmentVariable("MONGODB_PORT");
            var mongodbAuthDb = Environment.GetEnvironmentVariable("MONGODB_AUTH_DB");
            var mongodbSapMangement = Environment.GetEnvironmentVariable("MONGODB_SAP_MANGEMENT_LINK_DB");

            settings.Credential = MongoCredential.CreateCredential(mongodbAuthDb, mongodbUser, mongodbPwd);
            settings.Server = new MongoServerAddress(mongodbHost, Convert.ToInt32(mongodbPort));

            MongoClient client = new MongoClient(settings);

            IMongoDatabase database = client.GetDatabase(mongodbSapMangement);

            services.TryAddSingleton<IMongoDatabase>(database);
            #endregion

            services.AddScoped(s =>
            {

                var slRoot = Environment.GetEnvironmentVariable("SL_ROOT");
                var slDBName = Environment.GetEnvironmentVariable("SL_DB_NAME");
                var slUserName = Environment.GetEnvironmentVariable("SL_USERNAME");
                var slPassword = Environment.GetEnvironmentVariable("SL_PASSWORD");

                if (string.IsNullOrWhiteSpace(slRoot) || string.IsNullOrWhiteSpace(slDBName) || string.IsNullOrWhiteSpace(slUserName) || string.IsNullOrWhiteSpace(slPassword))
                {
                    throw new InvalidOperationException("The environment variables SL_ROOT, SL_DB_NAME, SL_USERNAME, and SL_PASSWORD must be defined.");

                }

                return new SLConnection(new Uri(slRoot), slDBName, slUserName, slPassword);
            });

            return services;
        }
    }
}
