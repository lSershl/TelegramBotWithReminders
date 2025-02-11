using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bot.Persistence
{
    public static class Extensions
    {
        public static void ToSnakeCaseTables(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName()?.Underscore());

                var tableObjectIdentifier =
                    StoreObjectIdentifier.Table(
                        entity.GetTableName()?.Underscore()!,
                        entity.GetSchema());

                // Replace column names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName(tableObjectIdentifier)?.Underscore());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName()?.Underscore());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName()?.Underscore());
                }
            }
        }
    }
}
