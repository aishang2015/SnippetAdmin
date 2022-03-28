using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SnippetAdmin.Core.Dynamic.Attributes;
using SnippetAdmin.Core.Utils;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Models;
using SnippetAdmin.Models.Dynamic;
using System.Reflection;

namespace SnippetAdmin.Controllers.Dynamic
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DynamicController : ControllerBase
    {
        /// <summary>
        /// 获取程序所有动态接口信息
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CommonResult<List<GetDynamicInfoOutputModel>>), 200)]
        //[Authorize]
        //[SnippetAdminAuthorize]
        public CommonResult GetDynamicInfo()
        {
            var classes = ReflectionUtil.GetAssemblyTypes()
                .Where(t => t.GetCustomAttribute(typeof(DynamicApiAttribute)) != null)
                .Select(c =>
                new
                {
                    EntityName = c.Name,
                    Name = (c.GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).ApiName,
                    Group = (c.GetCustomAttribute(typeof(DynamicApiAttribute)) as DynamicApiAttribute).ApiGroup
                }).ToList();

            var result = classes.GroupBy(d => d.Group).Select(g => new GetDynamicInfoOutputModel
            {
                Group = g.Key,
                DynamicInfoGroups = g.Select(gdata => new DynamicInfoGroup
                {
                    EntityName = gdata.EntityName,
                    Name = gdata.Name
                }).ToList()
            });
            return this.SuccessCommonResult(result);
        }
    }
}
