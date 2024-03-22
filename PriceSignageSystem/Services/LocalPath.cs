using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Services
{
    public class LocalPath
    {
        public static string CreateLocalFilePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}