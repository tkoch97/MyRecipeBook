using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MySqlConnector;

namespace MyRecipeBook.Infrastructure.Migrations
{
    public static class DatabaseMigration
    {
        public static void Migrate(DatabaseType databaseType, string connectionString, IServiceProvider serviceProvider)
        {
            if(databaseType == DatabaseType.SqlServer)
                EnsureDatabaseCreated_SqlServer(connectionString);
            else if(databaseType == DatabaseType.MySql)
                EnsureDatabaseCreated_MySql(connectionString);

            MigrationsOnDatabase(serviceProvider);
        }

        private static void EnsureDatabaseCreated_MySql(string connectionString)
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
            var databaseName = connectionStringBuilder.Database;
            connectionStringBuilder.Remove("Database");

            var parameters = new DynamicParameters();
            parameters.Add("name", databaseName);

            using var dbConnection = new MySqlConnection(connectionStringBuilder.ConnectionString);
            var records = dbConnection.Query
                (
                "SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name", parameters
                );

            if (!records.Any()) 
            {
                dbConnection.Execute($"CREATE DATABASE {databaseName}");
            }
        }

        private static void EnsureDatabaseCreated_SqlServer(string connectionString)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.Remove("Database");

            using var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);

            var parameters = new DynamicParameters();
            parameters.Add("name", databaseName);

            var records = dbConnection.Query
                (
                "SELECT * FROM sys.databases WHERE name = @name", parameters
                );
            if(!records.Any())
                dbConnection.Execute($"CREATE DATABASE {databaseName}");
        }

        private static void MigrationsOnDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.ListMigrations();
            runner.MigrateUp();
        }

    }
}
