using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace MyRecipeBook.Infrastructure.Migrations
{
    public static class DatabaseMigration
    {
        public static void Migrate(string connectionString, IServiceProvider serviceProvider)
        {
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
                dbConnection.Execute($"CREATE DATABASE `{databaseName}`;");
            } else
            {
                return;
            }

        }

        private static void MigrationsOnDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.ListMigrations();
            runner.MigrateUp();
        }

    }
}
