using AutoMapper;
using SnippetAdmin.Data.Entity;
using SnippetAdmin.Models.Account;

namespace SnippetAdmin.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<SnippetAdminUser, UserInfoOutputModel>().ReverseMap();
        }
    }
}