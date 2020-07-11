using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OECS.Services
{
    public static class FileMng
    {
        public static void RemoveFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public static string UploadFile(HttpPostedFileBase file, string directory)  //upload file to directory
        {
            var fname = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            fname = fname + DateTime.Now.ToString("yymmssff") + extension;
            file.SaveAs(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(directory), fname));
            return fname;
        }
    }
}