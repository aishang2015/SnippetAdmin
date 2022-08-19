using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.FileStore;
using SnippetAdmin.Core.Helpers;
using SnippetAdmin.Data;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.Data.Enums;
using SnippetAdmin.Endpoint.Models.System.Setting;

namespace SnippetAdmin.Controllers.System
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "AccessApi")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class SettingController : ControllerBase
    {
        private readonly SnippetAdminDbContext _dbContext;

        private readonly IFileStoreService _fileStoreService;

        public SettingController(SnippetAdminDbContext dbContext,
            IFileStoreService fileStoreService)
        {
            _dbContext = dbContext;
            _fileStoreService = fileStoreService;
        }

        /// <summary>
        /// 获取登录页配置
        /// </summary>
        [HttpPost]
        [CommonResultResponseType(typeof(GetLoginPageSettingOutputModel))]
        [AllowAnonymous]
        public CommonResult GetLoginPageSetting()
        {
            var settings = GetSettings(
                 SettingType.LoginPageTitle,
                 SettingType.LoginPageBackground,
                 SettingType.SystemIcon);

            var result = new GetLoginPageSettingOutputModel
            {
                Title = GetSettingValue(settings, SettingType.LoginPageTitle),
                Background = GetSettingValue(settings, SettingType.LoginPageBackground),
                Icon = GetSettingValue(settings, SettingType.SystemIcon),
            };

            return CommonResult.Success(result);
        }

        /// <summary>
        /// 保存登录页配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CommonResultResponseType]
        public async Task<CommonResult> SaveLoginPageSetting([FromForm] SaveLoginPageSettingInputModel formData)
        {
            var settings = GetSettings(
                SettingType.LoginPageTitle,
                SettingType.LoginPageBackground,
                SettingType.SystemIcon);

            if (formData.Icon != null)
            {
                var newFileName = GuidHelper.NewSequentialGuid() + "." + formData.Icon.FileName.Split('.').Last();
                await _fileStoreService.SaveFromStreamAsync(formData.Icon.OpenReadStream(), newFileName);
                await SaveSetting(settings, SettingType.SystemIcon, newFileName);
            }

            if (formData.Background != null)
            {
                var newFileName = GuidHelper.NewSequentialGuid() + "." + formData.Background.FileName.Split('.').Last();
                await _fileStoreService.SaveFromStreamAsync(formData.Background.OpenReadStream(), newFileName);
                await SaveSetting(settings, SettingType.LoginPageBackground, newFileName);
            }

            if (!string.IsNullOrEmpty(formData.Title))
            {
                await SaveSetting(settings, SettingType.LoginPageTitle, formData.Title);
            }

            await _dbContext.SaveChangesAsync();

            return CommonResult.Success(MessageConstant.SETTING_INFO_0001);
        }

        private List<SysSetting> GetSettings(params SettingType[] types)
        {
            return _dbContext.SysSettings
                .Where(s => types.Contains(s.SettingType)).ToList();
        }

        private static string GetSettingValue(List<SysSetting> settings, SettingType type)
        {
            return settings.FirstOrDefault(s => s.SettingType == type)?.Value;
        }

        private async Task SaveSetting(List<SysSetting> settings, SettingType type, string value)
        {
            var iconSetting = settings.FirstOrDefault(s => s.SettingType == type);
            if (iconSetting != null)
            {
                iconSetting.Value = value;
                _dbContext.SysSettings.Update(iconSetting);
            }
            else
            {
                await _dbContext.SysSettings.AddAsync(new SysSetting
                {
                    SettingType = type,
                    Value = value
                });
            }
        }
    }
}
