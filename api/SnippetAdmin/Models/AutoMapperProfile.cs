using AutoMapper;
using SnippetAdmin.Data.Entity.RBAC;
using SnippetAdmin.Models.Account;
using SnippetAdmin.Models.RBAC.Element;

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
        }
    }
}