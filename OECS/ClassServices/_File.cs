using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OECS.ClassServices
{
    public static class _File
    {
        public static void RemoveFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}