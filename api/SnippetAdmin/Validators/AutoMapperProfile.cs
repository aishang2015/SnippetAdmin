using AutoMapper;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Data.Entity.Scheduler;
using SnippetAdmin.Data.Entity.System;
using SnippetAdmin.Endpoint.Models.Account;
using SnippetAdmin.Endpoint.Models.RBAC.Element;
using SnippetAdmin.Endpoint.Models.RBAC.Organization;
using SnippetAdmin.Endpoint.Models.RBAC.Role;
using SnippetAdmin.Endpoint.Models.RBAC.User;
using SnippetAdmin.Endpoint.Models.Scheduler.Job;
using SnippetAdmin.Endpoint.Models.System.Dic;

namespace SnippetAdmin.Models
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<RbacUser, UserInfoOutputModel>().ReverseMap();

			CreateMap<RbacElement, GetElementOutputModel>();
			CreateMap<CreateElementInputModel, RbacElement>();
			CreateMap<UpdateElementInputModel, RbacElement>();

			CreateMap<RbacOrganization, GetOrganizationOutputModel>();
			CreateMap<CreateOrganizationInputModel, RbacOrganization>();
			CreateMap<UpdateOrganizationInputModel, RbacOrganization>();

			CreateMap<RbacRole, GetRoleOutputModel>();
			CreateMap<AddOrUpdateRoleInputModel, RbacRole>();

			CreateMap<AddOrUpdateUserInputModel, RbacUser>();
			CreateMap<RbacUser, GetUserOutputModel>();

			CreateMap<Job, GetJobsOutputModel>();

			CreateMap<RbacOrganizationType, GetOrganizationTypesOutputModel>();

			CreateMap<SysDicType, GetDicTypeListOutputModel>();
			CreateMap<AddDicTypeInputModel, SysDicType>();
			CreateMap<UpdateDicTypeInputModel, SysDicType>();
			CreateMap<SysDicValue, GetDicValueListOutputModel>();
			CreateMap<AddDicValueInputModel, SysDicValue>();
			CreateMap<UpdateDicValueInputModel, SysDicValue>();
		}
	}
}