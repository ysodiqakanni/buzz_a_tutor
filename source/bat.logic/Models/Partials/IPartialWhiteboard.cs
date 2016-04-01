using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Partials
{
    public interface IPartialWhiteboard
    {
        bat.data.Lesson lesson { get; set; }
        List<bat.data.AccountAttachment> attachments { get; set; }
    }
}
