using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class IPrincipalExtension
    {
        private static string GetClaim(string claim, System.Security.Principal.IPrincipal IPrincipal)
        {
            if (typeof(System.Security.Claims.ClaimsPrincipal).IsInstanceOfType(IPrincipal))
            {
                System.Security.Claims.ClaimsPrincipal _principal = (IPrincipal as System.Security.Claims.ClaimsPrincipal);
                Security.Claims.Claim _claim = _principal.FindFirst(claim);
                
                if(_claim != null) {
                    return _claim.Value;
                }else{
                    throw new Gale.Exception.GaleException("SECURITY_CLAIMS_NOT_FOUND");
                }

            }
            else
            {
                throw new Gale.Exception.GaleException("SECURITY_CLAIMS_NOT_IMPLEMENTED");
            }
        }

        /// <summary>
        /// Get the Primary SID of the current user (identifier)
        /// </summary>
        /// <param name="IPrincipal"></param>
        /// <returns></returns>
        public static String PrimarySid(this System.Security.Principal.IPrincipal IPrincipal)
        {
            return GetClaim(System.Security.Claims.ClaimTypes.PrimarySid, IPrincipal);
        }


        /// <summary>
        /// Get a Claim from the User Identity
        /// </summary>
        /// <param name="IPrincipal"></param>
        /// <param name="claim">Claim Name to retrieve (can be a value from 'System.Security.Claims.ClaimTypes')</param>
        /// <returns></returns>
        public static String Claim(this System.Security.Principal.IPrincipal IPrincipal, string claim)
        {
            return GetClaim(claim, IPrincipal);
        }
    }
}
