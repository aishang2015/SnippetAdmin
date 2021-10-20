using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SnippetAdmin.Data.Auth;
using SnippetAdmin.Models;
using System.Collections.Generic;
using System.Linq;

namespace SnippetAdmin.Controllers.RBAC
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiInfoController : ControllerBase
    {
        /// <summary>
        /// 获取程序所有API信息
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CommonResult<List<string>>), 200)]
        [Authorize]
        [SnippetAdminAuthorize]
        public CommonResult GetApiPaths([FromServices] IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
        {
            var result = apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
                .SelectMany(i => i.Items).Select(i => i.RelativePath);
            return this.SuccessCommonResult(result);
        }
    }
}