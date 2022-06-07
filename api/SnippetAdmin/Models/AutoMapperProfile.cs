using AutoMapper;
using SnippetAdmin.Data.Entity.Rbac;
using SnippetAdmin.Data.Entity.Scheduler;
using SnippetAdmin.Models.Account;
using SnippetAdmin.Models.RBAC.Element;
using SnippetAdmin.Models.RBAC.Organization;
using SnippetAdmin.Models.RBAC.Role;
using SnippetAdmin.Models.RBAC.User;
using SnippetAdmin.Models.Scheduler.Job;

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
        }
    }
}