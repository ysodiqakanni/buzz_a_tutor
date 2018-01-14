using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels
{
    public class PaginatedLists
    {
        internal string page = "";
        internal int pageSize = 20;
        public int PageTabSize => 8;
        public int TotalRows { get; set; }
        
        public int CurrentPage
        {
            get
            {
                var ret = 1;
                return int.TryParse(this.page, out ret) ? ret : 1;
            }
        }

        public int TotalPages
        {
            get
            {
                var totrows = this.TotalRows;
                if (totrows < this.pageSize) return 0;
                var tot = totrows / this.pageSize;
                if (tot == 0) return 0;
                if ((totrows % this.pageSize) > 0) tot++;
                return tot;
            }
        }
    }
}
