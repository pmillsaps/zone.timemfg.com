using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Time.IT.Helpers
{
    public static class UploadAttachments
    {
        private static string _AttachmentPath;

        public static void Upload(this HttpPostedFileBase file, string ByModelOrId, string Model, int compId)
        {
            var buf = new byte[file.ContentLength];
            file.InputStream.Read(buf, 0, file.ContentLength);
            var fn = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1).ToLower();

            if (ByModelOrId == "Model")// Checking if is by model
            {
                _AttachmentPath = HttpContext.Current.Server.MapPath(String.Format(@"~\Modules\Time.IT\Content\AttachmentFiles\ByComputerModel\{0}\", Model));
            }
            else// Else by computer specific
            {
                _AttachmentPath = HttpContext.Current.Server.MapPath(String.Format(@"~\Modules\Time.IT\Content\AttachmentFiles\ByComputerId\{0}\", compId));
            }
            if (!Directory.Exists(_AttachmentPath)) Directory.CreateDirectory(_AttachmentPath);
            var fullpath = Path.Combine(_AttachmentPath, fn);
            File.WriteAllBytes(fullpath, buf);
        }
    }
}