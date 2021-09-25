using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetAdmin.Models.RBAC.Role
{
    public class ActiveRoleInputModel
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }
    }
}
