using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.FileStore;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.Endpoint.Models.System.Setting;
using System.ComponentModel;

namespace SnippetAdmin.Controllers.System
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	//[Authorize(Policy = "AccessApi")]
	[AllowAnonymous]
	[ApiExplorerSettings(GroupName = "v1")]
	public class SettingController : ControllerBase
	{
		private readonly SnippetAdminDbContext _dbContext;

		private readonly IFileStoreService _fileStoreService;

		private readonly IMapper _mapper;

		public SettingController(SnippetAdminDbContext dbContext,
			IFileStoreService fileStoreService,
			IMapper mapper)
		{
			_dbContext = dbContext;
			_fileStoreService = fileStoreService;
			_mapper = mapper;
		}

        /// <summary>
        /// 取得系统配置
        /// </summary>
        [HttpPost]
        [CommonResultResponseType<GetSettingsOutputModel>]
        [Description("取得系统配置")]
        public CommonResult<GetSettingsOutputModel> GetSettings(GetSettingsInputModel inputModel)
        {
            var result = new GetSettingsOutputModel();

            result.Settings = _dbContext.SysSettings.Where(s => inputModel.KeyList.Contains(s.Key))
                .Select(s => new Setting
                {
                    Key = s.Key,
                    Value = s.Value,
                }).ToList();
            return CommonResult.Success(result);
        }

        /// <summary>
        /// 更新系统配置
        /// </summary>
        [HttpPost]
        [CommonResultResponseType]
        [Description("更新系统配置")]
        public async Task<CommonResult> UpdateSetting(UpdateSettingInputModel inputModel)
        {
            foreach (var setting in inputModel.Settings)
            {
                var findSetting = _dbContext.SysSettings.FirstOrDefault(s => s.Key == setting.Key);
                if (findSetting == null)
                {
                    _dbContext.SysSettings.Add(new SysSetting
                    {
                        Key = setting.Key,
                        Value = setting.Value
                    });
                }
                else
                {
                    findSetting.Value = setting.Value;
                    _dbContext.SysSettings.Update(findSetting);
                }
            }
            await _dbContext.SaveChangesAsync();
            return CommonResult.Success(MessageConstant.SYSTEM_INFO_004);
        }
    }
}
