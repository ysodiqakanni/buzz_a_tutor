using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.logic.Exceptions;

namespace bat.logic.Models.Homepage
{
    public class Dashboard : Master
    {
        public void Load()
        {
            if (!this.initialised) throw new MasterModelNotInitialised();
        }
    }
}
