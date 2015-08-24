using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gale.Exception.SqlClient
{
    /// <summary>
    /// Defines a Custom Database Exception in SQL SERVER , (ErrorNumber > 5000)
    /// https://msdn.microsoft.com/en-us/library/ms178592.aspx
    /// </summary>
    public class CustomDatabaseException : System.Exception
    {
        private string _code;

        public CustomDatabaseException(string code, string message)
            : base(message)
        {
            this._code = code;
        }

        public String Code
        {
            get
            {
                return _code;
            }
        }


    }
}
