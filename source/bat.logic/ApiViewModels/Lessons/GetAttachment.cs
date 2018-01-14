using bat.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace bat.logic.ApiViewModels.Lessons
{
    public class GetAttachment
    {
        public class blackboardImage
        {
            public string id { get; set; }
            public string title { get; set; }
        }
    }
}
