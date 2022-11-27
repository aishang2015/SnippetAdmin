using Microsoft.AspNetCore.Mvc;

namespace SnippetAdmin.Core.Attributes
{
	public class CommonResultResponseTypeAttribute : ProducesResponseTypeAttribute 
	{
		public CommonResultResponseTypeAttribute() : base(200)
		{
			Type = typeof(CommonResult);
		}
	}

	public class CommonResultResponseTypeAttribute<T> : ProducesResponseTypeAttribute where T : class
	{
		public CommonResultResponseTypeAttribute() : base( 200)
		{
			Type = typeof(CommonResult<T>);
		}
	}
}