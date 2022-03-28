namespace SnippetAdmin.Core.Dynamic.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DynamicApiAttribute : Attribute
    {
        public string ApiName { get; private set; }

        public string ApiGroup { get; private set; }

        public DynamicApiAttribute(string apiName, string apiGroup = "默认分组")
        {
            ApiName = apiName;
            ApiGroup = apiGroup;
        }
    }
}
