using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Constants
{
    class Azure
    {
        public const string AZURE_STORAGE_ACCOUNT_NAME = "buzzatutor";
        public const string AZURE_STORAGE_ACCOUNT_KEY = "QclHbmZT/KdWpk9860ztq7CAuuELw2rSEZqKg5CMetRiWniuJNigIscljGWwllpHexBsGmuWaEV8fgi5RjSvhw==";

        public static string AZURE_UPLOADED_LESSON_RESOURCES_STORAGE_CONTAINER
        {
            get
            {
#if DEBUG
                return "lesson-files-dev";
#endif
                return "lesson-files";
            }
        }

        public static string AZURE_UPLOADED_PROFILE_IMAGES_STORAGE_CONTAINER
        {
            get
            {
#if DEBUG
                return "profiles-dev";
#endif
                return "profiles";
            }
        }

        public static string AZURE_UPLOADED_EXAM_PAPERS_STORAGE_CONTAINER
        {
            get
            {
#if DEBUG
                return "exampapers-dev";
#endif
                return "exampapers";
            }
        }
    }
}
