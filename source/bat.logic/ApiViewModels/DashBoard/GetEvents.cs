using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ApiViewModels.DashBoard
{
    public class GetEvents
    {
        public class calEvent
        {
            public int id { get; set; }
            public string title { get; set; }
            public string start { get; set; }
            public string color { get; set; }
        }
        public List<calEvent> CalEvents { get; set; }
    }
}
