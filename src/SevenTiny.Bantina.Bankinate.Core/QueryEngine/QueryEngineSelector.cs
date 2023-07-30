using SevenTiny.Bantina.Bankinate.Core.Exceptions;
using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate
{
    internal static class QueryEngineSelector
    {
        internal static ILinqQueryable<TEntity> Select<TEntity>(DataBaseType dataBaseType, DbContext dbContext) where TEntity : class
        {
            switch (dataBaseType.GetCategory())
            {
                case DataBaseCategory.Relational:
                    return new SqlQueryable<TEntity>(dbContext as SqlDbContext);
                case DataBaseCategory.NonRelational:
                    return new NoSqlQueryable<TEntity>(dbContext as NoSqlDbContext);
                default:
                    break;
            }
            throw new UnknownDataBaseTypeException("Please select the correct database.");
        }
    }
}
