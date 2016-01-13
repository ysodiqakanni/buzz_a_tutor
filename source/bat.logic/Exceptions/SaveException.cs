using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Exceptions
{
    public class SaveException : Exception
    {
        public SaveException() { }
        public SaveException(string message) : base(message) { }
    }
}
