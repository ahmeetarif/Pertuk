using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pertuk.Business.BunnyCDN;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public virtual async Task<string> UploadProfilePicture(IFormFile formFile, string fileName)
        {
            if (formFile == null)
            {
                return _mediaOptions.EmptyProfilePicture;
            }
            var imagePath = _mediaOptions.ProfileImages + fileName + ".jpg";
            var filePath = _mediaOptions.StorageZoneName + _mediaOptions.ProfileImages;
            var destination = Path.Combine(filePath, fileName + ".jpg");

            bool flag = false;

            var checkFileExist = await _bunnyCDNService.GetStorageObjectsAsync(filePath);
            var isExist = checkFileExist.FirstOrDefault(x => x.ObjectName == fileName + ".jpg");

            if (isExist != null)
            {
                await _bunnyCDNService.DeleteObjectAsync(destination);
                flag = true;
            }

            try
            {
                using (Stream reader = formFile.OpenReadStream())
                {
                    await _bunnyCDNService.UploadAsync(reader, destination);
                }

                if (flag)
                {
                    await PurgeUrl(imagePath);
                }

                return imagePath;
            }
            catch (Exception)
            {
                return _mediaOptions.EmptyProfilePicture;
            }
        }

        public virtual async Task PurgeImageUrl(string url)
        {
            await PurgeUrl(url);
        }

        #region Private Functions

        private async Task PurgeUrl(string destination)
        {
            var baseAddress = new Uri("https://bunnycdn.com/");
            var purgeImageUrl = _mediaOptions.SitePath + destination;

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accesskey", "1ef93e03-2878-46cb-bd38-0244415c153853767b93-907d-431c-bf6e-91224dc974f9");

                using (var content = new StringContent("", Encoding.Default, "application/json"))
                {
                    using (var response = await httpClient.PostAsync($"api/purge?url={purgeImageUrl}", content))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        #endregion
    }
}