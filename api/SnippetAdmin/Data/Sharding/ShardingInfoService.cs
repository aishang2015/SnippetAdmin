using Hawthorn.EntityFramework.Sharding;

namespace SnippetAdmin.Data.Sharding
{
	public class ShardingInfoService : IShardingInfoService
	{
		private List<(Type, string)> _typeTableNames = new();

		public void AddShardingInfo(params (Type, string)[] shardingKvs)
		{
			_typeTableNames.AddRange(shardingKvs);
			_typeTableNames = _typeTableNames.Distinct().ToList();
		}

		public List<(Type, string)> GetShardingList() => _typeTableNames.Distinct().ToList();

		public string GetShardingInfoKey()
		{
			return string.Join(';', _typeTableNames.Select(tt => $"{tt.Item1.FullName},{tt.Item2}"));
		}
	}
}
