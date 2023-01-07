using Microsoft.AspNetCore.Mvc;

namespace SnippetAdmin.Controllers.Dynamic
{
	//[Route("api/[controller]/[action]")]
	//[ApiController]
	//[ApiExplorerSettings(GroupName = "v1")]
	public class DynamicController : ControllerBase//, IDynamicApi
	{
		///// <summary>
		///// 获取程序所有动态接口信息
		///// </summary>
		//[HttpPost]
		//[CommonResultResponseType(typeof(CommonResult<List<GetDynamicInfoOutputModel>>))]
		//public Task<CommonResult<List<GetDynamicInfoOutputModel>>> GetDynamicInfo()
		//{
		//    var classes = ReflectionHelper.GetAssemblyTypes()
		//        .Where(t => t.GetCustomAttribute(typeof(DynamicApiAttribute)) != null)
		//        .Select(c =>
		//        new
		//        {
		//            EntityName = c.Name,
		//            Name = (c.GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).ApiName,
		//            Group = (c.GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).ApiGroup
		//        }).ToList();

		//    var result = classes.GroupBy(d => d.Group).Select(g => new GetDynamicInfoOutputModel
		//    {
		//        Group = g.Key,
		//        DynamicInfoGroups = g.Select(gdata => new DynamicInfoGroup
		//        {
		//            EntityName = gdata.EntityName,
		//            Name = gdata.Name
		//        }).ToList()
		//    }).ToList();
		//    return Task.FromResult(CommonResult.Success(result));
		//}

		///// <summary>
		///// 获取动态实体列信息
		///// </summary>
		//[HttpPost]
		//[CommonResultResponseType(typeof(CommonResult<List<GetColumnsOutputModel>>))]
		//public Task<CommonResult<List<GetColumnsOutputModel>>> GetColumns([FromBody] GetColumnsInputModel inputModel)
		//{
		//    var entityType = ReflectionHelper.GetAssemblyTypes()
		//        .Where(t => t.GetCustomAttribute(typeof(DynamicApiAttribute)) != null &&
		//            t.Name == inputModel.EntityName)
		//        .FirstOrDefault();
		//    var result = entityType.GetProperties().Select(p => new GetColumnsOutputModel
		//    {
		//        PropertyName = p.Name,
		//        PropertyDescribe = (p.GetCustomAttribute(typeof(CommentAttribute)) as CommentAttribute)?.Comment ?? p.Name,
		//        PropertyType = p.PropertyType switch
		//        {
		//            Type proType when proType == typeof(short) => "number",
		//            Type proType when proType == typeof(int) => "number",
		//            Type proType when proType == typeof(long) => "number",
		//            Type proType when proType == typeof(double) => "number",
		//            Type proType when proType == typeof(float) => "number",
		//            Type proType when proType == typeof(decimal) => "number",

		//            Type proType when proType == typeof(short?) => "number",
		//            Type proType when proType == typeof(int?) => "number",
		//            Type proType when proType == typeof(long?) => "number",
		//            Type proType when proType == typeof(double?) => "number",
		//            Type proType when proType == typeof(float?) => "number",
		//            Type proType when proType == typeof(decimal?) => "number",

		//            Type proType when proType == typeof(string) => "string",

		//            Type proType when proType == typeof(DateTime) => "date",
		//            Type proType when proType == typeof(DateTime?) => "date",

		//            Type proType when proType == typeof(bool) => "bool",
		//            Type proType when proType == typeof(bool?) => "bool",

		//            Type proType when proType.IsEnum => "enum",
		//            Type proType when Nullable.GetUnderlyingType(proType) != null &&
		//                Enum.GetUnderlyingType(proType).IsEnum => "enum",

		//            _ => string.Empty
		//        }
		//    }).ToList();
		//    return Task.FromResult(CommonResult.Success(result));
		//}
	}
}
