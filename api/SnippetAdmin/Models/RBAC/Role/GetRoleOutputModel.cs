namespace SnippetAdmin.Models.RBAC.Role
{
    public class GetRoleOutputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Remark { get; set; }

        public int[] Rights { get; set; }

        public bool IsActive { get; set; }
    }
}
