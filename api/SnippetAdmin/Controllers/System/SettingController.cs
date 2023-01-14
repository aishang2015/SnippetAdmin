using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.Constants;
using SnippetAdmin.Core.Attributes;
using SnippetAdmin.Core.FileStore;
using SnippetAdmin.Data;
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

		[HttpPost]
		[CommonResultResponseType<List<GetSettingGroupsOutputModel>>]
		[Description("取得配置组信息")]
		public CommonResult<List<GetSettingGroupsOutputModel>> GetSettingGroups()
		{
			var result = _dbContext.SysSettingGroups
				.OrderBy(g => g.Index)
				.Select(g => new GetSettingGroupsOutputModel
				{
					Code = g.Code,
					Name = g.Name,
					Icon = g.Icon,
				})
				.ToList();
			return CommonResult.Success(result);
		}

		[HttpPost]
		[CommonResultResponseType<List<GetSettingsOutputModel>>]
		[Description("取得配置详细信息")]
		public CommonResult<List<GetSettingsOutputModel>> GetSettings(GetSettingsInputModel inputModel)
		{
			var subGroups = _dbContext.SysSettingSubGroups
				.Where(g => g.GroupCode == inputModel.GroupCode).ToList();
			var settings = _dbContext.SysSettings
				.Where(g => g.GroupCode == inputModel.GroupCode && g.IsShow).ToList();

			var result = _mapper.Map<List<GetSettingsOutputModel>>(subGroups);
			result.ForEach(r =>
			{
				var dbSettings = settings.Where(s => s.SubGroupCode == r.Code).ToList();
				r.Settings = _mapper.Map<List<Setting>>(dbSettings);
			});
			return CommonResult.Success(result);
		}

		[HttpPost]
		[CommonResultResponseType]
		[Description("更新配置值")]
		public async Task<CommonResult> SaveSetting(SaveSettingInputModel inputModel)
		{
			var setting = _dbContext.SysSettings.FirstOrDefault(s => s.Code == inputModel.Code);
			if (setting == null)
			{
				return CommonResult.Fail(MessageConstant.SETTING_ERROR_0001);
			}

			setting.Value = inputModel.Value;
			_dbContext.SysSettings.Update(setting);
			await _dbContext.SaveChangesAsync();
			return CommonResult.Success(MessageConstant.SETTING_INFO_0001, setting.Name);
		}

		///// <summary>
		///// 取得系统配置
		///// </summary>
		//[HttpPost]
		//[CommonResultResponseType<List<SysSetting>>]
		//[Description("取得系统配置")]
		//public CommonResult<List<SysSetting>> GetSettings(GetSettingsInputModel inputModel)
		//{
		//	var result = _dbContext.SysSettings.Where(s => inputModel.KeyList.Contains(s.Key)).ToList();
		//	return CommonResult.Success(result);
		//}

		///// <summary>
		///// 更新系统配置
		///// </summary>
		//[HttpPost]
		//[CommonResultResponseType]
		//[Description("更新系统配置")]
		//public async Task<CommonResult> UpdateSetting(UpdateSettingInputModel inputModel)
		//{
		//	var setting = _dbContext.SysSettings.FirstOrDefault(s => s.Key == inputModel.Key);
		//	if (setting == null)
		//	{
		//		_dbContext.SysSettings.Add(new SysSetting
		//		{
		//			Key = inputModel.Key,
		//			Value = inputModel.Value
		//		});
		//	}
		//	else
		//	{
		//		setting.Value = inputModel.Value;
		//		_dbContext.SysSettings.Update(setting);
		//	}
		//	await _dbContext.SaveChangesAsync();
		//	return CommonResult.Success(MessageConstant.SYSTEM_INFO_004);
		//}

		//    setting表结构变化处理注释掉
		//      /// <summary>
		//      /// 获取登录页配置
		//      /// </summary>
		//      [HttpPost]
		//      [CommonResultResponseType<GetLoginPageSettingOutputModel>]
		//      [AllowAnonymous]
		//[Description("获取登录页配置")]
		//public CommonResult GetLoginPageSetting()
		//      {
		//          var settings = GetSettings(
		//               SettingType.LoginPageTitle,
		//               SettingType.LoginPageBackground,
		//               SettingType.SystemIcon);

		//          var result = new GetLoginPageSettingOutputModel
		//          {
		//              Title = GetSettingValue(settings, SettingType.LoginPageTitle),
		//              Background = GetSettingValue(settings, SettingType.LoginPageBackground),
		//              Icon = GetSettingValue(settings, SettingType.SystemIcon),
		//          };

		//          return CommonResult.Success(result);
		//      }

		//      /// <summary>
		//      /// 保存登录页配置
		//      /// </summary>
		//      /// <returns></returns>
		//      [HttpPost]
		//      [CommonResultResponseType]
		//[Description("保存登录页配置")]
		//public async Task<CommonResult> SaveLoginPageSetting([FromForm] SaveLoginPageSettingInputModel formData)
		//      {
		//          var settings = GetSettings(
		//              SettingType.LoginPageTitle,
		//              SettingType.LoginPageBackground,
		//              SettingType.SystemIcon);

		//          if (formData.Icon != null)
		//          {
		//              var newFileName = GuidHelper.NewSequentialGuid() + "." + formData.Icon.FileName.Split('.').Last();
		//              await _fileStoreService.SaveFromStreamAsync(formData.Icon.OpenReadStream(), newFileName);
		//              await SaveSetting(settings, SettingType.SystemIcon, newFileName);
		//          }

		//          if (formData.Background != null)
		//          {
		//              var newFileName = GuidHelper.NewSequentialGuid() + "." + formData.Background.FileName.Split('.').Last();
		//              await _fileStoreService.SaveFromStreamAsync(formData.Background.OpenReadStream(), newFileName);
		//              await SaveSetting(settings, SettingType.LoginPageBackground, newFileName);
		//          }

		//          if (!string.IsNullOrEmpty(formData.Title))
		//          {
		//              await SaveSetting(settings, SettingType.LoginPageTitle, formData.Title);
		//          }

		//          await _dbContext.SaveChangesAsync();

		//          return CommonResult.Success(MessageConstant.SETTING_INFO_0001);
		//      }

		//      private List<SysSetting> GetSettings(params SettingType[] types)
		//      {
		//          return _dbContext.SysSettings
		//              .Where(s => types.Contains(s.SettingType)).ToList();
		//      }

		//      private static string GetSettingValue(List<SysSetting> settings, SettingType type)
		//      {
		//          return settings.FirstOrDefault(s => s.SettingType == type)?.Value;
		//      }

		//      private async Task SaveSetting(List<SysSetting> settings, SettingType type, string value)
		//      {
		//          var iconSetting = settings.FirstOrDefault(s => s.SettingType == type);
		//          if (iconSetting != null)
		//          {
		//              iconSetting.Value = value;
		//              _dbContext.SysSettings.Update(iconSetting);
		//          }
		//          else
		//          {
		//              await _dbContext.SysSettings.AddAsync(new SysSetting
		//              {
		//                  SettingType = type,
		//                  Value = value
		//              });
		//          }
		//      }
	}
}
