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
    public class Upload
    {
        public class lessonAttachment
        {
            public int id { get; set; }
            public string title { get; set; }
        }
        public List<lessonAttachment> lessonAttachments { get; set; }
    }
}
