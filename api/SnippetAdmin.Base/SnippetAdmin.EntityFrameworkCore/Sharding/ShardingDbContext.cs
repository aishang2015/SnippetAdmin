using Hawthorn.EntityFramework.Sharding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace SnippetAdmin.EntityFrameworkCore.Sharding
{
	public class ShardingDbContext : DbContext
	{
		public string ShardingKey { get; set; }

		private List<(Type, string)> _typeNames;

		private DbContextOptions _options;

		public ShardingDbContext(DbContextOptions options,
			IShardingInfoService service) : base(options)
		{
			_options = options;
			ShardingKey = service.GetShardingInfoKey();
			_typeNames = service.GetShardingList();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			if (_typeNames.Count > 0)
			{
				foreach (var typeName in _typeNames)
				{
					modelBuilder.SharedTypeEntity(typeName.Item2, typeName.Item1).ToTable(typeName.Item2);
				}
			}
		}

		public async Task CreateTable(string tableName, Type type)
		{
			using var context = new TableDbContext(GetOption(), tableName, type);
			var creator = context.GetService<IRelationalDatabaseCreator>();
			try
			{
				await creator.CreateTablesAsync();
			}
			catch (Exception)
			{
			}
		}

		private DbContextOptions<TableDbContext> GetOption()
		{
			var builder = new DbContextOptionsBuilder<TableDbContext>()
				.EnableServiceProviderCaching(false);

#pragma warning disable EF1001 // Internal EF Core API usage.
			var inMemoryOptions = _options.FindExtension<InMemoryOptionsExtension>();
			if (inMemoryOptions != null)
			{
				return builder.UseInMemoryDatabase(inMemoryOptions.StoreName)
					.Options;
			}

			var sqliteOptions = _options.FindExtension<SqliteOptionsExtension>();
			if (sqliteOptions != null)
			{
				return builder.UseSqlite(sqliteOptions.ConnectionString)
					.Options;
			}

			var sqlServerOptions = _options.FindExtension<SqlServerOptionsExtension>();
			if (sqlServerOptions != null)
			{
				return builder.UseSqlServer(sqlServerOptions.ConnectionString)
					.Options;
			}

			var mysqlOptions = _options.FindExtension<MySqlOptionsExtension>();
			if (mysqlOptions != null)
			{
				return builder.UseMySql(mysqlOptions.ConnectionString,
					ServerVersion.AutoDetect(mysqlOptions.ConnectionString))
					.Options;
			}

			var npgsqlOptions = _options.FindExtension<NpgsqlOptionsExtension>();
			if (inMemoryOptions != null)
			{
				return builder.UseNpgsql(npgsqlOptions.ConnectionString)
					.Options;
			}

			//var oracleOptions = _options.GetExtension<OracleOptionsExtension>();
			//if (oracleOptions != null)
			//{
			//	// todo
			//	return builder.UseOracle(oracleOptions.ConnectionString)
			//		.Options;
			//}

			return null;
#pragma warning restore EF1001 // Internal EF Core API usage.
		}
	}
}
