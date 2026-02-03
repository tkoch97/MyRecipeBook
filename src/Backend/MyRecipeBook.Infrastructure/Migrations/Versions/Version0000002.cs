using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_USER_ADD_USER_IDENTIFIER, "Add UserIdentifier column to Users table")]
    public class Version0000002 : VersionBase
    {
        
        public override void Up()
        {
            Alter.Table("Users")
                .AddColumn("UserIdentifier").AsGuid().Nullable().WithDefault(SystemMethods.NewGuid);
            
            Execute.Sql("UPDATE Users SET UserIdentifier = UserIdentifier");

            Alter.Table("Users")
                .AlterColumn("UserIdentifier").AsGuid().NotNullable();
        }
    }
}
