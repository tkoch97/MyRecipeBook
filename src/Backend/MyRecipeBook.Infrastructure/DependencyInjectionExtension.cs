using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Infrastructure.DataAccess;
using MyRecipeBook.Infrastructure.DataAccess.Repositories;
using MyRecipeBook.Infrastructure.Extensions;
using MyRecipeBook.Infrastructure.Secutiry.Cryptography;
using MyRecipeBook.Infrastructure.Secutiry.Tokens.Access.Generator;
using MyRecipeBook.Infrastructure.Secutiry.Tokens.Access.Validator;
using MyRecipeBook.Infrastructure.Services.LoggedUser;
using System.Reflection;

namespace MyRecipeBook.Infrastructure
{
    public static class DependencyInjectionExtension
    {

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {   
            AddPasswordEncrypter(services, configuration);
            AddRepositories(services);
            AddTokens(services, configuration);
            AddLoggedUser(services);
            if (configuration.IsInMemoryTestEnvironment())
            {
                return;
            }

            var databaseType = configuration.DatabaseType();
            if (databaseType == DatabaseType.MySql)
            {
                AddDbContext_MySQL(services, configuration);
                AddFluentMigrator_MySQL(services, configuration);
            } else
            {
                AddDbContext_SQLServer(services, configuration);
                AddFluentMigrator_SqlServer(services, configuration);
            }
        }

        private static void AddDbContext_MySQL(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 43));

            services.AddDbContext<MyRecipeBookDbContext>(options =>
            {
                options.UseMySql(connectionString, serverVersion);
            });
        }

        private static void AddDbContext_SQLServer(IServiceCollection services, IConfiguration configuration) 
        { 
            var connectionString = configuration.ConnectionString();

            services.AddDbContext<MyRecipeBookDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }

        private static void AddFluentMigrator_MySQL(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options
                .AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
            });
        }

        private static void AddFluentMigrator_SqlServer(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
            });
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            services.AddScoped<IRecipeWriteOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeReadOnlyRepository, RecipeRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddLoggedUser(IServiceCollection services) => services.AddScoped<ILoggedUser, LoggedUser>();

        private static void AddTokens(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

            services.AddScoped<IAccessTokenGenerator>
                (options => new JwtTokenGenerator(
                    signingKey!, expirationTimeMinutes));
            services.AddScoped<IAccessTokenValidator>(
                options => new JwtTokenValidator(signingKey!));
        }

        private static void AddPasswordEncrypter(IServiceCollection services, IConfiguration configuration)
        {
            var additionalKey = configuration.GetValue<string>("Settings:Passwords:AdditionalKey");
            services.AddScoped<IPasswordEncrypter>(options => new Sha512Encrypter(additionalKey!));
        }

    }
}
