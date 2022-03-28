using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Core.Attributes;
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
        [CommonResultResponseType(typeof(CommonResult<List<GetDynamicInfoOutputModel>>))]
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

        /// <summary>
        /// 获取动态实体列信息
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(CommonResult<List<GetColumnsOutputModel>>))]
        public CommonResult GetColumns([FromBody] GetColumnsInputModel inputModel)
        {
            var entityType = ReflectionUtil.GetAssemblyTypes()
                .Where(t => t.GetCustomAttribute(typeof(DynamicApiAttribute)) != null &&
                    t.Name == inputModel.EntityName)
                .FirstOrDefault();
            var result = entityType.GetProperties().Select(p => new GetColumnsOutputModel
            {
                PropertyName = p.Name,
                PropertyDescribe = (p.GetCustomAttribute(typeof(CommentAttribute)) as CommentAttribute)?.Comment ?? p.Name
            });
            return this.SuccessCommonResult(result);
        }
    }
}
