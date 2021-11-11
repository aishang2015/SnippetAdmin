using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace SnippetAdmin.Data.Cache
{
    public class MemoryCacheInitializer
    {
        public static readonly Action<IMemoryCache, SnippetAdminDbContext> InitialCache =
            (memoryCache, dbcontext) =>
            {
                var dbSetPropertyTypes = dbcontext.GetType().GetProperties()
                    .Where(property =>
                        property.PropertyType.IsGenericType && (
                        typeof(DbSet<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()) ||
                        property.PropertyType.GetInterface(typeof(DbSet<>).FullName) != null));

                var toListMethod = typeof(MemoryCacheInitializer).GetMethod("GetDataList");
                foreach (var dbSetProperty in dbSetPropertyTypes)
                {
                    var method = toListMethod.MakeGenericMethod(dbSetProperty.PropertyType.GetGenericArguments()[0]);
                    var data = method.Invoke(new MemoryCacheInitializer(), new object[] { dbcontext });
                    memoryCache.Set(dbSetProperty.PropertyType.GetGenericArguments()[0].FullName, data);
                }
            };

        public List<T> GetDataList<T>(SnippetAdminDbContext dbContext) where T : class
        {
            return dbContext.Set<T>().ToList();
        }
    }
}
