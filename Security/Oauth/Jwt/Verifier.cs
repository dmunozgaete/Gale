using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gale.Security.Oauth.Jwt
{
    public class Verifier
    {
        public static ClaimsPrincipal Validate(string token, InMemorySymmetricSecurityKey signingKey, string issuer, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Parse JWT from the Base64UrlEncoded wire form 
            //(<Base64UrlEncoded header>.<Base64UrlEncoded body>.<signature>)
            //JwtSecurityToken parsedJwt = tokenHandler.ReadToken(token) as JwtSecurityToken;

            TokenValidationParameters validationParams =
                new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = audience,
                    ValidIssuer = issuer,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true
                };

            SecurityToken securityToken = null;

            return tokenHandler.ValidateToken(token, validationParams, out securityToken);
        }
    }
}
