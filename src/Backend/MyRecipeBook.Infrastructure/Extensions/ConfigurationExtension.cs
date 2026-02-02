using Microsoft.Extensions.Configuration;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Infrastructure.Extensions
{
    public static class ConfigurationExtension
    {
        public static bool IsInMemoryTestEnvironment(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }

        public static DatabaseType DatabaseType(this IConfiguration configuration)
        {
            var databaseType = configuration.GetConnectionString("DatabaseType");
            return (DatabaseType)Enum.Parse(typeof(DatabaseType), databaseType!);
        }

        public static string ConnectionString(this IConfiguration configuration)
        {
            var dataBaseType = configuration.DatabaseType();
            if(dataBaseType == Domain.Enums.DatabaseType.MySql)
                return configuration.GetConnectionString("ConnectionMySQLServer")!;
            else
                return configuration.GetConnectionString("ConnectionSqlServer")!;
        }
    }
}
