﻿namespace SnippetAdmin.Models.RBAC.User
{
    public class GetUserOutputModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string RealName { get; set; }

        public int Gender { get; set; }

        public string PhoneNumber { get; set; }

        public int[] Roles { get; set; }
    }
}
