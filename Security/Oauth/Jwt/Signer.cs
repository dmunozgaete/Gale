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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Gale.Security.Oauth.Jwt
{
    public class Signer
    {
        public static Wrapper Sign(List<Claim> claims, DateTime expiresIn, SigningCredentials credentials, string issuer, string audience)
        {
            var notBefore = DateTime.Now;       //Not Before Date Expires
            String jwt = null;

            var pem = System.Configuration.ConfigurationManager.AppSettings["Gale:JWT:PrivateKey:Path"];
            if (pem != null)
            {
                //Complex Sign (RSA256)
                string privateKey = "";
                if (pem.IndexOf(":\\") >= 0)
                {
                    privateKey = System.IO.File.ReadAllText(pem);
                }
                else
                {
                    privateKey = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(System.IO.Path.Combine("~", pem)));
                }

                RSAParameters rsaParams;
                using (var tr = new StringReader(privateKey))
                {
                    var pemReader = new PemReader(tr);
                    var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                    if (keyPair == null)
                    {
                        throw new System.Exception("Could not read RSA private key");
                    }
                    var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                    rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
                }
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(rsaParams);

                    var epoch = new DateTime(1970, 1, 1);
                    claims.Add(new Claim("aud", audience));
                    claims.Add(new Claim("iss", issuer));
                    claims.Add(new Claim("exp", ((Int32)(expiresIn.Subtract(epoch).TotalSeconds)).ToString()));
                    claims.Add(new Claim("nbf", ((Int32)(notBefore.Subtract(epoch).TotalSeconds)).ToString()));
                    claims.Add(new Claim("iat", ((Int32)(notBefore.Subtract(epoch).TotalSeconds)).ToString()));

                    Dictionary<string, object> payload = claims.ToDictionary(k => k.Type, v => (object)v.Value);
                    jwt = Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);
                }


            }
            else
            {
                var token = new JwtSecurityToken(issuer, audience, claims, notBefore, expiresIn, credentials);
                var handler = new JwtSecurityTokenHandler();
                jwt = handler.WriteToken(token);


            }

            return new Wrapper(expiresIn, "Bearer", jwt);

        }
    }
}
