using Pertuk.Entities.Models;

namespace Pertuk.Business.Services.Abstract
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser userInfo);
    }
}
