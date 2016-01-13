using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Exceptions
{
    public class MasterModelNotInitialised : Exception
    {
        public MasterModelNotInitialised() { }
        public MasterModelNotInitialised(string message) : base(message) { }
    }
}
