using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Exceptions
{
    public class ZoomException : Exception
    {
        public int Code { get; set; }

        public ZoomException() { }
        public ZoomException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
