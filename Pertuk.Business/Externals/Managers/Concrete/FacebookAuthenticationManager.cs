﻿using Newtonsoft.Json;
using Pertuk.Business.Externals.Contracts;
using Pertuk.Business.Externals.Managers.Abstract;
using Pertuk.Business.Options;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pertuk.Business.Externals.Managers.Concrete
{
    public class FacebookAuthenticationManager : IFacebookAuthenticationManager
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,picture,email&access_token={0}";

        private readonly FacebookAuthOptions _facebookAuthOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        public FacebookAuthenticationManager(
            FacebookAuthOptions facebookAuthOptions,
            IHttpClientFactory httpClientFactory)
        {
            _facebookAuthOptions = facebookAuthOptions;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            var formattedUrl = string.Format(UserInfoUrl, accessToken);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken)
        {
            var formattedUrl = string.Format(TokenValidationUrl, accessToken, _facebookAuthOptions.AppId, _facebookAuthOptions.AppSecret);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }
    }
}