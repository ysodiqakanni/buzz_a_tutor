using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.ViewModels.Partials
{
    public interface IPartialWhiteboard
    {
        bat.data.Lesson lesson { get; set; }
        List<Models.Lessons.Attachment> attachments { get; set; }
        bat.data.Account host { get; set; }
        bat.data.Account account { get; set; }
    }
}
