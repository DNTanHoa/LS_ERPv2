using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Helpers
{
    public class FileHelpers
    {
        public static bool SaveFile(IFormFile file, string directory ,out string fullPath, out string subPath)
        {
            fullPath = string.Empty;
            subPath = string.Empty;

            if (!string.IsNullOrEmpty(directory))
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (file.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + DateTime.Now.ToString("yyyyMMddHHmmssfff")
                                    + Path.GetExtension(file.FileName);
                    var filePath = directory + fileName;

                    using (var stream = System.IO.File.Create(filePath, (int)file.Length))
                    {
                         file.CopyTo(stream);
                    }

                    fullPath = filePath;
                    subPath = fileName;

                    return true;
                }
            }

            return false;
        }
    }
}
