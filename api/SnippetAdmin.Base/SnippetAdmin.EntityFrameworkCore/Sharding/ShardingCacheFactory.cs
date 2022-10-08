using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SnippetAdmin.EntityFrameworkCore.Sharding
{
    public class ShardingCacheFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
            => context is ShardingDbContext db
                ? (context.GetType(), db.ShardingKey, designTime)
                : (object)context.GetType();

        public object Create(DbContext context)
            => Create(context, false);
    }
}
