using Microsoft.AspNetCore.Http;
using System.IO;

namespace Pertuk.Common.Helpers
{
    public static class UploadImageHelper
    {

        public static string CreateFileName(IFormFile formFile, string email, string lastWordExtension = null)
        {
            var uploadFileExtension = Path.GetExtension(formFile.FileName);
            var newFileName = email + lastWordExtension ?? string.Empty + uploadFileExtension;
            return newFileName;
        }

    }
}