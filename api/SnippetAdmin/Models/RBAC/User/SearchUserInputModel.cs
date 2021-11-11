﻿using SnippetAdmin.Models.Common;

namespace SnippetAdmin.Models.RBAC.User
{
    public class SearchUserInputModel : PagedInputModel
    {
        public string UserName { get; set; }

        public string RealName { get; set; }

        public string Phone { get; set; }

        public int? Role { get; set; }

        public int? Org { get; set; }
    }
}