using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bat.data;

namespace bat.logic.Rules
{
    public class ExamPapers
    {
        public static MemoryStream DownloadPaper(int paperId)
        {
            try
            {
                using (var conn = new dbEntities())
                {
                    var paper = conn.SubjectExamPapers.FirstOrDefault(s => s.ID == paperId);
                    if (paper == null) throw new Exception("The exam paper does not exist.");

                    using (var memoryStream = new MemoryStream())
                    {
                        logic.Helpers.AzureStorage.StoredResources.DownloadExamPaper(memoryStream, paper.StorageName);
                        return memoryStream;
                    }
                }
            }
            catch (Exception ex)
            {
                var ignore = ex;
            }
            return null;
        }
    }
}
