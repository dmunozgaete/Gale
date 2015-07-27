using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.Security.Oauth.Jwt
{
    public class Wrapper
    {
        private DateTime _expiresIn;
        private string _type;
        private string _token;

        /// <summary>
        /// Retrieves the time for which token will be expires
        /// </summary>
        public int expires_in
        {
            get
            {
                //UNIX TIME VALUE
                return  (int)Math.Truncate((_expiresIn.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            }
        }
        
        /// <summary>
        /// Retrieves the token type
        /// </summary>
        public string token_type
        {
            get
            {
                return _type;
            }
        }
        
        
        /// <summary>
        /// Retrieves the jwt token
        /// </summary>
        public string access_token
        {
            get
            {
                return _token;
            }
        }

        public Wrapper(DateTime expiresIn, string type, string token)
        {
            _expiresIn = expiresIn;
            _type = type;
            _token = token;
        }
    }
}
