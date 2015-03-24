using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Karma.Security.Oauth.Jwt
{
    public class AuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private string _errorCode = "";
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (Authorize(actionContext))
            {
                return;
            }
            HandleUnauthorizedRequest(actionContext);
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //DOCUMENTATION: RFC 6750 http://self-issued.info/docs/draft-ietf-oauth-v2-bearer.html

            //WWW-Authenticate Header 
            System.Web.HttpContext.Current.Response.Headers.Add("WWW-Authenticate", "Bearer");

            //--------------------------------------------------------------------------------
            //401  Unauthorized
            var statusCode = System.Net.HttpStatusCode.Unauthorized;
            var error = "";
            var error_description = Karma.Exception.Errors.ResourceManager.GetString(_errorCode);

            switch (_errorCode)
            {
                case "BEARER_TOKEN_NOT_FOUND":
                    //400 BAD REQUEST
                    statusCode = System.Net.HttpStatusCode.BadRequest;
                    error = "invalid_request";
                    break;
                case "TOKEN_EXPIRED":
                    //400 BAD REQUEST
                    error = "invalid_token";
                    break;
                case "ACCESS_UNAUTHORIZED":
                    //400 BAD REQUEST
                    error = "invalid_token";
                    break;

            }
            //--------------------------------------------------------------------------------

            throw new HttpResponseException(new System.Net.Http.HttpResponseMessage()
            {
                ReasonPhrase = error,
                StatusCode = statusCode,
                Content = new System.Net.Http.ObjectContent<AuthorizeError>(new AuthorizeError()
                {
                    error = error,
                    error_description = error_description,
                    code = _errorCode
                },
                new System.Net.Http.Formatting.JsonMediaTypeFormatter())
            });

        }

        private bool Authorize(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                System.Net.Http.Headers.AuthenticationHeaderValue header = actionContext.Request.Headers.Authorization;
                if (header == null)
                {
                    //--------------------------------------------------------------------------------
                    _errorCode = "BEARER_TOKEN_NOT_FOUND";
                    return false;
                    //--------------------------------------------------------------------------------
                }
                else
                {
                    //--------------------------------------------------------------------------------
                    var JwtVerifier = Karma.Security.Oauth.Jwt.Manager.ValidateToken(header.Parameter);
                    return true;
                    //--------------------------------------------------------------------------------
                }
            }
            catch (System.IdentityModel.Tokens.SecurityTokenExpiredException ex)
            {
                _errorCode = "TOKEN_EXPIRED";
                return false;
            }
            catch (System.Exception ex)
            {
                _errorCode = "ACCESS_UNAUTHORIZED";
                return false;
            }
        }


        internal class AuthorizeError
        {
            public string error { get; set; }

            public string code { get; set; }

            public string error_description { get; set; }
        }
    }
}

