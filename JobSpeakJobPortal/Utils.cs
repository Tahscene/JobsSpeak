using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSpeakJobPortal
{
    public class Utils
    {
        public static bool IsValidExtension(string fileName)
        {
            bool isValid = false;
            string[] fileExtension = { ".jpg", ".png", ".jpeg" };
            for (int i = 0; i < fileExtension.Length; i++)
            {
                if (fileName.EndsWith(fileExtension[i], StringComparison.OrdinalIgnoreCase))
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        public static bool IsValidExtension4Resume(string fileName)
        {
            bool isValid = false;
            string[] fileExtensions = { ".doc", ".docx", ".pdf" };
            for (int i = 0; i < fileExtensions.Length; i++)
            {
                if (fileName.EndsWith(fileExtensions[i], StringComparison.OrdinalIgnoreCase))
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }
    }
}
