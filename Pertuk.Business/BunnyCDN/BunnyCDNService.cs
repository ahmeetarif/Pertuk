using BunnyCDN.Net.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Pertuk.Business.Options;
using Pertuk.Common.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pertuk.Business.BunnyCDN
{
    public class BunnyCDNService : BunnyCDNStorage
    {
        private const string ApiKey = "d5e1d437-9fa3-4da4-98d3cbb50920-d00a-4a46";
        private const string ZoneName = "pertukmedia";
        private const string ZoneRegion = "de";
        private readonly MediaOptions _mediaOptions;

        public BunnyCDNService(IOptions<MediaOptions> mediaOptions)
            : base(ZoneName, ApiKey, ZoneRegion)
        {
            _mediaOptions = mediaOptions.Value;
        }

        public virtual async Task<string> UploadProfilePictureAsync(IFormFile formFile, string newFilename, string fileExtension = null)
        {
            if (formFile == null) return _mediaOptions.EmptyProfilePicture;

            string newFilenameForUpload = UploadImageHelper.CreateFileName(formFile, newFilename, fileExtension);
            string uploadPathDirectory = _mediaOptions.StorageZoneName + _mediaOptions.ProfileImages;
            string uploadDestination = Path.Combine(uploadPathDirectory, newFilenameForUpload);

            bool flag = false;

            var getAllFiles = await GetStorageObjectsAsync(uploadPathDirectory);
            var isFileExist = getAllFiles.FirstOrDefault(x => x.ObjectName == newFilenameForUpload);

            if (isFileExist != null)
            {
                await DeleteObjectAsync(uploadDestination);
                flag = true;
            }

            try
            {
                using (var stream = formFile.OpenReadStream())
                {
                    await UploadAsync(stream, uploadDestination);
                }

                if (flag)
                {
                    string purgeUrlDestination = _mediaOptions.ProfileImages + newFilenameForUpload;
                    await PurgeCache(purgeUrlDestination);
                }

                return _mediaOptions.ProfileImages + newFilenameForUpload;
            }
            catch (Exception)
            {
                return _mediaOptions.EmptyProfilePicture;
            }
        }

        #region Private Functions

        /// <summary>
        /// Delete Cache on the CDN Service..
        /// </summary>
        /// <param name="destination">Folder + Filename NOT:(Without Storagename)</param>
        /// <returns></returns>
        private async Task PurgeCache(string destination)
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