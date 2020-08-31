using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pertuk.Business.BunnyCDN;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Concrete
{
    public class UploadImageService : IUploadImageService
    {
        private readonly BunnyCDNService _bunnyCDNService;
        private readonly MediaOptions _mediaOptions;

        public UploadImageService(BunnyCDNService bunnyCDNService,
                                  IOptions<MediaOptions> mediaOptions)
        {
            _bunnyCDNService = bunnyCDNService;
            _mediaOptions = mediaOptions.Value;
        }

        public virtual async Task UploadProfilePicture(IFormFile formFile)
        {
            try
            {
                var filePath = _mediaOptions.StorageZoneName + _mediaOptions.ProfileImages;
                var destination = Path.Combine(filePath, Guid.NewGuid().ToString() + ".jpg");

                using (Stream reader = formFile.OpenReadStream())
                {
                    await _bunnyCDNService.UploadAsync(reader, destination);
                }

                await Task.CompletedTask;
            }
            catch (Exception)
            {
                throw new PertukApiException();
            }
        }
    }
}
