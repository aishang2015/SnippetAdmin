using Microsoft.AspNetCore.Http;

namespace SnippetAdmin.Endpoint.Models.System.Setting
{
    public class SaveLoginPageSettingInputModel
    {
        public string Title { get; set; }

        public IFormFile Background { get; set; }

        public IFormFile Icon { get; set; }
    }
}
