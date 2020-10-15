using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Services.Abstract;
using Pertuk.DataAccess.Repositories.Abstract;

namespace Pertuk.Business.Services.Concrete
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserManagerService : IUserManagerService
    {
        private readonly PertukUserManager _pertukUserManager;
        private readonly IStudentUsersRepository _studentUsersRepository;
        private readonly ITeacherUsersRepository _teacherUsersRepository;
        private readonly IUploadImageService _uploadImageService;
        public UserManagerService(
            PertukUserManager pertukUserManager,
            IStudentUsersRepository studentUsersRepository,
            ITeacherUsersRepository teacherUsersRepository,
            IUploadImageService uploadImageService)
        {
            _pertukUserManager = pertukUserManager;
            _studentUsersRepository = studentUsersRepository;
            _teacherUsersRepository = teacherUsersRepository;
            _uploadImageService = uploadImageService;
        }
    }
}