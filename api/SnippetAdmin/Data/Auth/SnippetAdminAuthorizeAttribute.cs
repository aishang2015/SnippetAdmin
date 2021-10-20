using Microsoft.AspNetCore.Mvc;

namespace SnippetAdmin.Data.Auth
{
    public class SnippetAdminAuthorizeAttribute : TypeFilterAttribute
    {
        public SnippetAdminAuthorizeAttribute() : base(typeof(SnippetAdminAuthorizeFilter))
        {
        }
    }
}