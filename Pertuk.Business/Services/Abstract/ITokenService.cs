using Pertuk.Entities.Models;

namespace Pertuk.Business.Services.Abstract
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
