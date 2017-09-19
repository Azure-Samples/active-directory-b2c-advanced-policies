using WingTipCommon;

namespace WingTipToysWebApplication
{
    public static class MigrationStatusExtensions
    {
        public static string ToDisplayString(this MigrationStatus migrationStatus)
        {
            switch (migrationStatus)
            {
                case MigrationStatus.New:
                    {
                        return "New";
                    }

                case MigrationStatus.NotMigrated:
                    {
                        return "Not migrated";
                    }

                case MigrationStatus.MigratedWithoutPassword:
                    {
                        return "Migrated without password";
                    }

                case MigrationStatus.MigratedWithPassword:
                    {
                        return "Migrated with password";
                    }

                default:
                    {
                        return "Unknown";
                    }
            }
        }
    }
}
