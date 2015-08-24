using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace System.Web
{
    public static class FileUploadHelpers
    {
        public static void StoreLoadListFile(this HttpPostedFileBase file, int LoadListId)
        {
            var buf = new byte[file.ContentLength];
            file.InputStream.Read(buf, 0, file.ContentLength);
            var fn = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1).ToLower();
            var _AttachmentPath = HttpContext.Current.Server.MapPath(String.Format(@"\Content\LoadListImages\{0}\", LoadListId));
            if (!Directory.Exists(_AttachmentPath)) Directory.CreateDirectory(_AttachmentPath);
            var fullpath = _AttachmentPath + fn;
            System.IO.File.WriteAllBytes(fullpath, buf);
        }
    }
}