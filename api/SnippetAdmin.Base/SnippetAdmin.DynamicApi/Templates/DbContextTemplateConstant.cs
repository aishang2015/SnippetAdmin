namespace SnippetAdmin.DynamicApi.Templates
{
	public class DbContextTemplateConstant
	{

		public static string ControllerTemplate { get; private set; } =
"""
using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using SnippetAdmin.CommonModel;
using SnippetAdmin.CommonModel.Extensions;
using SnippetAdmin.DynamicApi.Models;
using SnippetAdmin.DynamicApi.Templates;
using SnippetAdmin.Core.Extensions;
using SnippetAdmin.Models.{Entity};

using {Namespace};

using {DbContextNamespace};

namespace SnippetAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "{GroupName}")]
    public class {Entity}Controller: ControllerBase
    {

        private readonly {DbContextType} {DbContextName};

        public {Entity}Controller({DbContextType} {DbContextName})
        {
            this.{DbContextName} = {DbContextName};
        }
        
        [HttpPost("FindOne")]
        public CommonResult FindOne([FromBody] IdInputModel<int> inputModel)
        {
            var result = {DbContextName}.Set<{Entity}>().Find(inputModel.Id);
            return CommonResult.Success(result);
        }

        [HttpPost("GetMany")]
        public CommonResult GetMany([FromBody] DynamicSearchInputModel inputModel)
        {
            var dataQuery = {DbContextName}.Set<{Entity}>().AsQueryable();
            dataQuery = inputModel.GetFilterExpression(dataQuery);
            dataQuery = dataQuery.Sort(inputModel.Sorts);
            var result = new PagedOutputModel<{Entity}>
            {
                Total = dataQuery.Count(),
                Data = dataQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToList()
            };            
            return CommonResult.Success(result);
        }

        [HttpPost("GetMany2")]
        public CommonResult GetMany2([FromBody] Get{Entity}InputModel inputModel)
        {
            var dataQuery = {DbContextName}.Set<{Entity}>().AsQueryable();
            dataQuery = dataQuery{QueryCondition};
            dataQuery = dataQuery.Sort(inputModel.Sorts);
            var result = new PagedOutputModel<{Entity}>
            {
                Total = dataQuery.Count(),
                Data = dataQuery.Skip(inputModel.SkipCount).Take(inputModel.TakeCount).ToList()
            };            
            return CommonResult.Success(result);
        }

        [HttpPost("AddOne")]
        public async Task<CommonResult> AddOne([FromBody] {Entity} entity)
        {
            {DbContextName}.Set<{Entity}>().Add(entity);
            await {DbContextName}.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_003);
        }

        [HttpPost("AddMany")]
        public async Task<CommonResult> AddOne([FromBody] IEnumerable<{Entity}> data)
        {
            {DbContextName}.Set<{Entity}>().AddRange(data);
            await {DbContextName}.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_003);
        }

        [HttpPost("UpdateOne")]
        public async Task<CommonResult> UpdateOne([FromBody] {Entity} entity)
        {
            {DbContextName}.Set<{Entity}>().Update(entity);
            await {DbContextName}.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_002);
        }

        [HttpPost("DeleteOne")]
        public async Task<CommonResult> DeleteOne([FromBody] IdInputModel<int> inputModel)
        {
            var result = {DbContextName}.Set<{Entity}>().Find(inputModel.Id);
            {DbContextName}.Remove(result);
            await {DbContextName}.SaveChangesAsync();
            return CommonResult.Success(DynamicMessageConstants.DYNAMIC_INFO_001);
        }

        {dicAction}
    }
}
""";

		public static string DicActionTemplate { get; private set; } = """

        [HttpPost("GetDic")]
        public CommonResult GetDic()
        {
            var query = {DbContextName}.Set<{Entity}>().Select(d => new DicOutputModel<string>{
                Key = d.{KeyProperty}.ToString(),
                Value = d.{ValueProperty}.ToString()
            }).Distinct();
            return CommonResult.Success(query.ToList());
        }
""";

		public static string SearchModelTemplate { get; private set; } = """

        using System;
        using SnippetAdmin.CommonModel;

        namespace SnippetAdmin.Models.{Entity}
        {
            public record Get{Entity}InputModel: PagedInputModel
            {
                {Properties}
            }
        }
""";
	}
}
