using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IUploadImageService
    {
        Task UploadProfilePicture(IFormFile formFile);
    }
}