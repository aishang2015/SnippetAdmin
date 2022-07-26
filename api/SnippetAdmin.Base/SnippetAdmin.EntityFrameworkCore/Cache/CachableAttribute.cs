namespace SnippetAdmin.EntityFrameworkCore.Cache
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CachableAttribute : Attribute
    {
        public bool CacheAble { get; private set; }

        public CachableAttribute(bool cacheAble = true)
        {
            CacheAble = cacheAble;
        }
    }
}
