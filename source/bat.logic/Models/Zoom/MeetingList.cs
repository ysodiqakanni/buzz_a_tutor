using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Zoom
{
    public class MeetingList
    {
        public int page_count { get; set; }
        public int total_records { get; set; }
        public int page_number { get; set; }
        public int page_size { get; set; }
        public List<Meeting> meetings { get; set; }
    }
}
