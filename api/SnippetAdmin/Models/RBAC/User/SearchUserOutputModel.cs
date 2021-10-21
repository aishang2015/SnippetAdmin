namespace SnippetAdmin.Models.RBAC.User
{
    public class SearchUserOutputModel
    {
        public int Id { get; set; }

        public string Avatar { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public int Gender { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public RoleInfo[] Roles { get; set; }

        public OrgPositionOutputModel[] OrgPositions { get; set; }
    }

    public class RoleInfo
    {
        public string RoleName { get; set; }

        public bool IsActive { get; set; }
    }

    public class OrgPositionOutputModel
    {
        public string Org { get; set; }

        public string Position { get; set; }
    }
}