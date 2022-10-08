namespace Hawthorn.EntityFramework.Sharding
{
    public interface IShardingInfoService
    {
        public void AddShardingInfo(params (Type, string)[] shardingKvs);

        public List<(Type, string)> GetShardingList();

        public string GetShardingInfoKey();
    }
}
