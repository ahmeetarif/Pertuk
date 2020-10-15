using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IUploadImageService
    {
        Task<string> UploadProfilePicture(IFormFile formFile, string fileName);
        Task PurgeImageUrl(string url);
    }
}