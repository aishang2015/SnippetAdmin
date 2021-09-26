using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public string[] Roles { get; set; }

        public OrgPositionOutputModel[] OrgPositions { get; set; }
    }

    public class OrgPositionOutputModel
    {
        public string Org { get; set; }

        public string Position { get; set; }
    }
}
