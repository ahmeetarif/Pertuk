using Newtonsoft.Json;
using System;

namespace Pertuk.Business.Externals.Contracts
{
    public class FacebookUserInfoResult
    {
        [JsonProperty("first_name")]
        public string Firstname { get; set; }

        [JsonProperty("last_name")]
        public string Lastname { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("picture")]
        public FacebookPicture FacebookPicture { get; set; }
    }

    public class FacebookPicture
    {
        [JsonProperty("data")]
        public FacebookPictureData FacebookPictureData { get; set; }
    }

    public class FacebookPictureData
    {
        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }
    }
}
