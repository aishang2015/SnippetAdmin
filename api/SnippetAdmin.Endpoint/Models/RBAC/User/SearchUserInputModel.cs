using SnippetAdmin.Endpoint.Models.Common;

namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
    public record SearchUserInputModel : PagedInputModel
    {
        public string UserName { get; set; }

        public string RealName { get; set; }

        public string Phone { get; set; }

        public int? Role { get; set; }

        public int? Org { get; set; }

        public int? Position { get; set; }
    }
}