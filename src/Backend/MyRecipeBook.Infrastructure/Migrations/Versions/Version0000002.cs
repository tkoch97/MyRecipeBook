using FluentMigrator;

namespace MyRecipeBook.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_USER_ADD_USER_IDENTIFIER, "Add UserIdentifier column to Users table")]
    public class Version0000002 : VersionBase
    {
        public override void Up()
        {
            Alter.Table("Users")
                .AddColumn("UserIdentifier").AsGuid().NotNullable();

            Execute.Sql("UPDATE Users SET UserIdentifier = UUID()");
        }
    }
}
