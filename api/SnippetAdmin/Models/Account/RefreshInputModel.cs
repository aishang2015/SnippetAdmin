﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetAdmin.Models.Account
{
    public class RefreshInputModel
    {
        public string UserName { get; set; }

        public string JwtToken { get; set; }

        public string RefreshToken { get; set; }
    }
}