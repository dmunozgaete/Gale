using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gale.Security.Oauth.Jwt
{
    public class Manager
    {
        private static string _oauthSecret_base64 = "OUE0MjM0RjI4MzRBRDQ3MDRBOEE3MTg2OUI4RDEyQTk=";    //HEXA 64 //TODO BETTER OFUSCATION
        private static readonly string _issuer = "OAuthServer";
        private static readonly string _audienceId = "OAuthClient";

        /// <summary>
        /// Returns the Security Encryption Credentials
        /// </summary>
        private static System.IdentityModel.Tokens.InMemorySymmetricSecurityKey signingKey
        {
            get
            {
                return new InMemorySymmetricSecurityKey(Convert.FromBase64String(_oauthSecret_base64));    //SIGNING KEY //HMAC
            }
        }

        public static Wrapper CreateToken(List<Claim> claims)
        {
            return CreateToken(claims, DateTime.Now.AddYears(2));   //TWO YEARS EXPIRES
        }

        public static Wrapper CreateToken(List<Claim> claims, DateTime expiresIn)
        {
            var credentials = new SigningCredentials(signingKey, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", "http://www.w3.org/2001/04/xmlenc#sha256");

            return Signer.Sign(claims, expiresIn, credentials, _issuer, _audienceId);
        }

        public static ClaimsPrincipal ValidateToken(string token)
        {
            return Verifier.Validate(token, signingKey, _issuer, _audienceId);
        }

    }
}
