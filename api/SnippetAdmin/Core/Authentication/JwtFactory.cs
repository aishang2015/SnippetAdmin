using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SnippetAdmin.Core.Authentication
{
    public interface IJwtFactory
    {
        public string GenerateJwtToken(string userName);

        public string GenerateJwtToken(List<(string, string)> tuples = null);

        public ClaimsPrincipal ValidToken(string token);
    }

    public class JwtFactory : IJwtFactory
    {
        private readonly JwtOption _jwtOption;

        private readonly TokenValidationParameters _tokenValidationParams;

        public JwtFactory(IOptions<JwtOption> jwtOption,
            TokenValidationParameters tokenValidationParams)
        {
            _jwtOption = jwtOption.Value;
            _tokenValidationParams = tokenValidationParams;
        }

        public string GenerateJwtToken(string userName)
        {
            return GenerateJwtToken(new List<(string, string)> { (ClaimTypes.Name, userName) });
        }

        public string GenerateJwtToken(List<(string, string)> tuples = null)
        {
            var userClaims = new List<Claim>();
            if (tuples != null)
            {
                foreach (var tuple in tuples)
                {
                    userClaims.Add(new Claim(tuple.Item1, tuple.Item2));
                }
            }
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOption.Issuer,
                audience: _jwtOption.Audience,
                expires: _jwtOption.Expires,
                claims: userClaims,
                signingCredentials: _jwtOption.SigningCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }

        public ClaimsPrincipal ValidToken(string token)
        {
            try
            {
                return new JwtSecurityTokenHandler().ValidateToken(token, _tokenValidationParams,
                    out var securityToken);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}