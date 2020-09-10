using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Pertuk.MVC.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Pertuk.MVC.PertukApiServices
{
    public class PertukApiService
    {
        private IHttpContextAccessor _httpContextAccessor;
        public PertukApiService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiAuthenticationResponseModel> LoginAsync(ApiLoginRequestModel apiLoginRequest)
        {
            HttpClient client = new HttpClient();
            var jsonData = JsonConvert.SerializeObject(apiLoginRequest);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var result = await client.PostAsync("http://localhost:61693/Auth/LoginUser", stringContent);

            var data = await result.Content.ReadAsStringAsync();

            var deserializedData = JsonConvert.DeserializeObject<ApiAuthenticationResponseModel>(data);

            return deserializedData;
        }

        public async Task<ApiAuthenticationResponseModel> SendEmailConfirmation(string userId)
        {
            HttpClient client = new HttpClient();
            var jsonData = JsonConvert.SerializeObject(userId);

            var bearerToken = _httpContextAccessor.HttpContext.Session.GetString("userToken");

            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var result = await client.PostAsync("http://localhost:61693/Auth/SendEmailConfirmation", stringContent);

            var data = await result.Content.ReadAsStringAsync();

            var deserializedData = JsonConvert.DeserializeObject<ApiAuthenticationResponseModel>(data);

            return deserializedData;
        }
    }
}