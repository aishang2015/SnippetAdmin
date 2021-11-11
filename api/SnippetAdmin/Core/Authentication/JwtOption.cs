using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SnippetAdmin.Core.Authentication
{
    public class JwtOption
    {
        #region 配置属性

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public double ExpireSpan { get; set; }

        public string SecretKey { get; set; }

        public double RefreshExpireSpan { get; set; }

        #endregion 配置属性

        public DateTime Expires { get => DateTime.Now.AddMinutes(ExpireSpan); }

        public SecurityKey SecurityKey { get => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey)); }

        public SigningCredentials SigningCredentials { get => new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256); }
    }
}