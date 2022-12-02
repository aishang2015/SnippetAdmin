using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Data;
using SnippetAdmin.Endpoint.Apis.System;
using System.ComponentModel;
using System.Globalization;

namespace SnippetAdmin.Controllers.Develop
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	[Authorize]
	[ApiExplorerSettings(GroupName = "v1")]
	public class DataController : ControllerBase, IDataApi
	{
		private readonly SnippetAdminDbContext _dbContext;

		private readonly Dictionary<string, IQueryable> _entityDictionary;

		private readonly Dictionary<string, Func<StreamWriter, Task>> _methodDictionary;

		public DataController(SnippetAdminDbContext dbContext)
		{
			_dbContext = dbContext;
			_entityDictionary = new Dictionary<string, IQueryable>
			{
				{ "RbacElement" ,_dbContext.RbacElements},
				{ "RbacElementTree" , _dbContext.RbacElementTrees },
				{ "RbacOrganization" , _dbContext.RbacOrganizations},
				{ "RbacOrganizationTree" , _dbContext.RbacOrganizationTrees},
				{ "RbacOrganizationType" , _dbContext.RbacOrganizationTypes},
				{ "RbacPosition" , _dbContext.RbacPositions},
				{ "RbacRole" , _dbContext.Roles},
				{ "RbacRoleClaim" , _dbContext.RoleClaims},
				{ "RbacUser" , _dbContext.Users},
				{ "RbacUserClaim" , _dbContext.UserClaims},
				{ "RbacUserLogin" , _dbContext.UserLogins},
				{ "RbacUserRole" , _dbContext.UserRoles},
				{ "RbacUserToken" , _dbContext.UserTokens},
				{ "Job" , _dbContext.Jobs},
				{ "JobRecord" ,_dbContext.JobRecords},
				{ "SysDicType",_dbContext.SysDicTypes},
				{ "SysDicValue" , _dbContext.SysDicValues},
                //{ "SysRefreshToken" , _dbContext.SysRefreshTokens},
                //{ "SysExceptionLog" , _dbContext.SysExceptionLogs},
                //{ "SysAccessLog" ,_dbContext.SysApiAccessLogs},
                //{ "SysLoginLog" , _dbContext.SysLoginLogs},
            };

			_methodDictionary = new Dictionary<string, Func<StreamWriter, Task>>
			{
				{ "element",WriteElementAsync },
				{ "dictionary",WriteDictionaryAsync },
			};
		}

		/// <summary>
		/// 取得能够生成csv的数据类型
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[CommonResultResponseType<List<string>>]
		[Description("取得能够生成csv的数据类型")]
		public Task<CommonResult<List<string>>> GetCsvDataType()
		{
			var result = _entityDictionary.Keys.ToList();
			return Task.FromResult(CommonResult.Success(result));
		}

		/// <summary>
		/// 导出csv数据
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("导出csv数据")]
		public Task<FileContentResult> ExportCsvData(IdInputModel<string> model)
		{
			IQueryable queryable =
				_entityDictionary.ContainsKey(model.Id) ?
				_entityDictionary[model.Id] : throw new NotSupportedException();

			using var ms = new MemoryStream();
			using var sw = new StreamWriter(ms)
			{
				AutoFlush = true
			};
			using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
			foreach (var obj in queryable)
			{
				csv.WriteRecord(obj);
				csv.NextRecord();
			}

			ms.Seek(0, SeekOrigin.Begin);
			return Task.FromResult(File(ms.GetBuffer(), "text/plain", "data.csv"));
		}

		/// <summary>
		/// 取得能够生成代码的数据类型
		/// </summary>
		[HttpGet]
		[CommonResultResponseType<List<string>>]
		[Description("取得能够生成代码的数据类型")]
		public Task<CommonResult<List<string>>> GetCodeDataType()
		{
			return Task.FromResult(CommonResult.Success(_methodDictionary.Keys.ToList()));
		}

		/// <summary>
		/// 将数据导出为ef代码
		/// </summary>
		[HttpPost]
		[CommonResultResponseType]
		[Description("将数据导出为ef代码")]
		public async Task<FileContentResult> ExportCodeData(IdInputModel<string> model)
		{
			using var ms = new MemoryStream();
			using var sw = new StreamWriter(ms)
			{
				AutoFlush = true
			};

			if (!_methodDictionary.Keys.Contains(model.Id))
			{
				throw new NotSupportedException();
			}
			await _methodDictionary[model.Id](sw);

			ms.Seek(0, SeekOrigin.Begin);
			return File(ms.GetBuffer(), "text/plain", "element.txt");
		}

		private async Task WriteElementAsync(StreamWriter sw)
		{
			var elements = await _dbContext.RbacElements.ToListAsync();
			var elementTrees = await _dbContext.RbacElementTrees.ToListAsync();

			sw.WriteLine("// 元素数据");
			foreach (var e in elements)
			{
				sw.WriteLine($"_dbContext.RbacElements.Add(new RbacElement {{ Id = {e.Id}, Name = \"{e.Name}\", Identity = \"{e.Identity}\", Type = ElementType.{e.Type}, AccessApi = \"{e.AccessApi}\", Sorting = {e.Sorting} }});");
			}

			sw.WriteLine();
			sw.WriteLine("// 元素树数据");
			foreach (var e in elementTrees)
			{
				sw.WriteLine($"_dbContext.RbacElementTrees.Add(new RbacElementTree {{ Id = {e.Id}, Ancestor = {e.Ancestor}, Descendant = {e.Descendant} ,Length = {e.Length} }});");
			}
			sw.WriteLine("await _dbContext.SaveChangesAsync();");
		}

		private async Task WriteDictionaryAsync(StreamWriter sw)
		{
			var dicTypes = await _dbContext.SysDicTypes.ToListAsync();
			var dicValues = await _dbContext.SysDicValues.ToListAsync();

			sw.WriteLine("// 字典类型");
			foreach (var type in dicTypes)
			{
				sw.WriteLine($"_dbContext.SysDicTypes.Add(new SysDicType {{Id = {type.Id},Name = \"{type.Name}\", Code = \"{type.Code}\" }});");
			}

			sw.WriteLine();
			sw.WriteLine("// 字典项");
			foreach (var value in dicValues)
			{
				sw.WriteLine($"_dbContext.SysDicValues.Add(new SysDicValue {{Id = {value.Id},Name = \"{value.Name}\", Code = \"{value.Code}\", TypeId = {value.TypeId}, Sorting = {value.Sorting} }});");
			}
			sw.WriteLine("await _dbContext.SaveChangesAsync();");
		}
	}
}
