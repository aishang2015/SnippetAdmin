namespace SnippetAdmin.DynamicApi.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DynamicApiAttribute : Attribute
	{
		public string ApiName { get; private set; }

		public string ApiGroup { get; private set; }

		public Type DbContextType { get; private set; }

		public DynamicApiAttribute(string apiName, Type dbContextType, string apiGroup = "DynamicApi")
		{
			ApiName = apiName;
			DbContextType = dbContextType;
			ApiGroup = apiGroup;
		}
	}
}
