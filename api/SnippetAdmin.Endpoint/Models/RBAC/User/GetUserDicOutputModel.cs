using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnippetAdmin.Endpoint.Models.RBAC.User
{
    public class GetUserDicOutputModel
    {
        public int UserId { get; set; }

        public string RealName { get; set; }
    }
}
