using AutoMapper;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models.Account;
using SnippetAdmin.Models.RBAC.Element;
using SnippetAdmin.Models.RBAC.Organization;
using SnippetAdmin.Models.RBAC.Role;

namespace SnippetAdmin.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SnippetAdminUser, UserInfoOutputModel>().ReverseMap();

            CreateMap<Element, GetElementOutputModel>();
            CreateMap<CreateElementInputModel, Element>();
            CreateMap<UpdateElementInputModel, Element>();

            CreateMap<Organization, GetOrganizationOutputModel>();
            CreateMap<CreateOrganizationInputModel, Organization>();
            CreateMap<UpdateOrganizationInputModel, Organization>();

            CreateMap<SnippetAdminRole, GetRoleOutputModel>();
            CreateMap<AddOrUpdateRoleInputModel, SnippetAdminRole>();

        }
    }
}