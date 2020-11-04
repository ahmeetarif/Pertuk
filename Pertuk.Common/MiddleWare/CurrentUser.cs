using Microsoft.AspNetCore.Http;
using Pertuk.Common.MiddleWare.Statics;
using System.Linq;

namespace Pertuk.Common.MiddleWare
{
    public class CurrentUser
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Id
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == UserClaimTypes.UserId).Value;
            }
        }

        public string Email
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == UserClaimTypes.Email).Value;
            }
        }

    }
}