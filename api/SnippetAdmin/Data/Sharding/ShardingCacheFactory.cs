using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SnippetAdmin.Data.Sharding
{
	public class ShardingCacheFactory : IModelCacheKeyFactory
	{
		public object Create(DbContext context, bool designTime)
			=> context is SnippetAdminDbContext db
				? 1//(context.GetType(), db.ShardingKey, designTime)
				: (object)context.GetType();

		public object Create(DbContext context)
			=> Create(context, false);
	}
}
