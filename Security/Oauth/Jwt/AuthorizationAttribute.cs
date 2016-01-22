using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;

namespace Gale.Security.Oauth.Jwt
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
            int statusCode = (int)System.Net.HttpStatusCode.OK;
            var error = "";
            var error_description = Gale.Exception.Errors.ResourceManager.GetString(_errorCode);

            switch (_errorCode)
            {
                case "BEARER_TOKEN_NOT_FOUND":
                    //400 BAD REQUEST
                    statusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    error = "invalid_request";
                    break;
                case "TOKEN_EXPIRED":
                    //419 SESSION TIMEOUT
                    statusCode = 419;
                    error = "token_expired";
                    break;
                case "ACCESS_UNAUTHORIZED":
                    //401 UNAUTHORIZED
                    statusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    error = "access_denied";
                    break;
                case "INVALID_TOKEN":
                    //400 BAD REQUEST
                    statusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    error = "invalid_token";
                    break;

            }
            //--------------------------------------------------------------------------------

            throw new HttpResponseException(new System.Net.Http.HttpResponseMessage()
            {
                ReasonPhrase = error,
                StatusCode = (System.Net.HttpStatusCode)statusCode,
                Content = new System.Net.Http.ObjectContent<AuthorizeError>(new AuthorizeError()
                {
                    error = error,
                    error_description = error_description,
                    code = _errorCode
                },
                new System.Net.Http.Formatting.JsonMediaTypeFormatter())
            });

        }

        private string GetAccessTokenOnQuery(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var queryStrings = actionContext.Request.GetQueryNameValuePairs();
            if (queryStrings == null)
            {
                return null;
            }

            var match = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "access_token", true) == 0);
            if (string.IsNullOrEmpty(match.Value))
            {
                return null;
            }

            return match.Value;
        }

        private bool Authorize(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                string JwtToken = null;

                //--------------------------------------------------------------------------------
                //CHECK HEADER AUTHENTICATION
                System.Net.Http.Headers.AuthenticationHeaderValue header = actionContext.Request.Headers.Authorization;
                if (header == null)
                {
                    //CHECK ACCESS_TOKEN in QUERY
                    JwtToken = GetAccessTokenOnQuery(actionContext);
                }
                else
                {
                    JwtToken = header.Parameter;
                }
                //--------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------
                // CHECK EXISTENCE
                if (JwtToken == null)
                {
                    
                    _errorCode = "BEARER_TOKEN_NOT_FOUND";
                    return false;
                }
                //--------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------
                // CHECK JWT TOKEN
                var JwtVerifier = Gale.Security.Oauth.Jwt.Manager.ValidateToken(JwtToken);
                System.Web.HttpContext.Current.User = JwtVerifier;
                //--------------------------------------------------------------------------------

                //--------------------------------------------------------------------------------
                // IF RESTRICTED TO ROLEs, CHECK ALSO USERS (BASE METHOD CHECKER :P)
                if(!this.IsAuthorized(actionContext)){
                    _errorCode = "ACCESS_UNAUTHORIZED";
                    return false;
                }
                //--------------------------------------------------------------------------------

                return true;
                
            }
            catch (System.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                _errorCode = "TOKEN_EXPIRED";
                return false;
            }
            catch (System.Exception)
            {
                _errorCode = "INVALID_TOKEN";
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

