using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ApiViewModels.Chat
{
    public class Chat
    {
        public class message
        {
            public string name { get; set; }
            public string msg { get; set; }
        }
        public List<message> chatHistory { get; set; }
    }
}
