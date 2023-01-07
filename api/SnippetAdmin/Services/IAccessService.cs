namespace SnippetAdmin.Services
{
	public interface IAccessService
	{
		/// <summary>
		/// get current request's user's organization ids not contained lower levels org id
		/// </summary> 
		public List<int> GetCurrentUserOrgId();

		/// <summary>
		/// get current request's user's organization ids contains all lower levels org ids  
		/// </summary> 
		public List<int> GetCurrentUserGroupOrgId();
	}
}
