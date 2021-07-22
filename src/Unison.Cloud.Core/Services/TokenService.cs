using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;
using Unison.Cloud.Core.Interfaces.Configuration;
using Unison.Cloud.Core.Interfaces.Services;

namespace Unison.Cloud.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly IAuthConfiguration _authConfiguration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IAuthConfiguration authConfiguration, ILogger<TokenService> logger)
        {
            _authConfiguration = authConfiguration;
            _logger = logger;
        }

        public string Create(Account account)
        {
            SecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_authConfiguration.Secret);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, account.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
