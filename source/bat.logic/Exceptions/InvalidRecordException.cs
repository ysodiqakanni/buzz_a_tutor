using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Exceptions
{
    public class InvalidRecordException : Exception
    {
        public InvalidRecordException() { }
        public InvalidRecordException(string message) : base(message) { }
    }
}
