namespace SnippetAdmin.Models.RBAC.User
{
    public class RemoveOrgMemberInputModel
    {
        public int OrgId { get; set; }

        public int UserId { get; set; }
    }
}