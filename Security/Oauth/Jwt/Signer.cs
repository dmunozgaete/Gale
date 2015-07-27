using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gale.Security.Oauth.Jwt
{
    public class Signer
    {
        public static Wrapper Sign(List<Claim> claims, DateTime expiresIn, SigningCredentials credentials, string issuer, string audience)
        {
            /*
            System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            byte[] data = new byte[64];
            rng.GetBytes(data);
            BitConverter.ToString(data).Replace("-", "");
            */

            var notBefore = DateTime.Now;       //Not Before Date Expires
            var token = new JwtSecurityToken(issuer, audience, claims, notBefore, expiresIn, credentials);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return new Wrapper(expiresIn, "Bearer", jwt);
        }
    }
}
