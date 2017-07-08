using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Gale.Security.Oauth.Jwt
{
    public class Verifier
    {
        public static ClaimsPrincipal Validate(string token, InMemorySymmetricSecurityKey signingKey, string issuer, string audience)
        {

            var pub = System.Configuration.ConfigurationManager.AppSettings["Gale:JWT:PublicKey:Path"];
            if (pub != null)
            {
                //Complex Sign (RSA256)
                string publicKey = "";
                if (pub.IndexOf(":\\") >= 0)
                {
                    publicKey = System.IO.File.ReadAllText(pub);
                }
                else
                {
                    publicKey = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(System.IO.Path.Combine("~", pub)));
                }


                RSAParameters rsaParams;

                using (var tr = new StringReader(publicKey))
                {
                    var pemReader = new PemReader(tr);
                    var publicKeyParams = pemReader.ReadObject() as RsaKeyParameters;
                    if (publicKeyParams == null)
                    {
                        throw new System.Exception("Could not read RSA public key");
                    }
                    rsaParams = DotNetUtilities.ToRSAParameters(publicKeyParams);
                }
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(rsaParams);


                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParams = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidAudience = audience,
                        ValidIssuer = issuer,
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        ValidateLifetime = true
                    };

               
                    SecurityToken securityToken = null;
                    return tokenHandler.ValidateToken(token,validationParams, out securityToken);

                }

            }
            else
            {
                var tokenHandler = new JwtSecurityTokenHandler();


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
}
