using System;
using System.Threading.Tasks;
using AutoMapper;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Common.MiddleWare;
using Pertuk.Contracts.V1.Requests.UserManager;
using Pertuk.Contracts.V1.Responses.UserManager;
using Pertuk.DataAccess.UnitOfWork;
using Pertuk.Dto.Models;
using Pertuk.Entities.Models;

namespace Pertuk.Business.Services.Concrete
{
    public class UserManagerService : IUserManagerService
    {
        private readonly PertukUserManager _pertukUserManager;
        private readonly IMapper _mapper;
        private readonly CurrentUser _currentUser;

        public UserManagerService(
            PertukUserManager pertukUserManager,
            IMapper mapper,
            CurrentUser currentUser)
        {
            _pertukUserManager = pertukUserManager;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<UserManagerResponseModel> SetUserStudentAsync(StudentUserRequestModel studentUserRequest)
        {
            if (studentUserRequest == null) throw new PertukApiException("Please proide required information!");

            ApplicationUser getUserDetails = _pertukUserManager.GetUserDetails(_currentUser.Id);
            if (getUserDetails == null) throw new PertukApiException("User not found!");

            var studentDetails = _mapper.Map<StudentUsersDto>(studentUserRequest);

            bool studentAdded = await _pertukUserManager.SetUserStudentAsync(getUserDetails, studentDetails);

            if (studentAdded)
            {
                return new UserManagerResponseModel
                {
                    Message = "You're now a student!"
                };
            }

            throw new PertukApiException();
        }


        public async Task<UserManagerResponseModel> SetUserTeacherAsync(TeacherUserRequestModel teacherUserRequest)
        {
            if (teacherUserRequest == null) throw new PertukApiException("Please provide required information!");

            var userDetails = _pertukUserManager.GetUserDetails(_currentUser.Id);
            if (userDetails == null) throw new PertukApiException("User not found!");

            var teacherDetails = _mapper.Map<TeacherUsersDto>(teacherUserRequest);

            bool teacherAdded = await _pertukUserManager.SetUserTeacherAsync(userDetails, teacherDetails);

            if (teacherAdded)
            {
                return new UserManagerResponseModel
                {
                    Message = "You're now a Teacher! Please wait for Pertuk Developer to activate your account!"
                };
            }

            throw new PertukApiException();
        }

        public ApplicationUserDto GetUserDetail()
        {
            var userDetails = _pertukUserManager.GetUserDetails(_currentUser.Id);

            if (userDetails == null) throw new PertukApiException("User not found!");

            var mappedUserDetails = _mapper.Map<ApplicationUserDto>(userDetails);

            return mappedUserDetails;
        }

        #region Private Functions

        private void Users()
        {

        }

        #endregion
    }
}