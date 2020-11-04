using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Common.MiddleWare;
using Pertuk.Contracts.V1.Requests.UserManager;
using Pertuk.Contracts.V1.Responses.UserManager;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.DataAccess.UnitOfWork;
using Pertuk.Dto.Models;
using Pertuk.Entities.Models;

namespace Pertuk.Business.Services.Concrete
{
    public class UserManagerService : IUserManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PertukUserManager _pertukUserManager;
        private readonly IMapper _mapper;
        private readonly CurrentUser _currentUser;

        public UserManagerService(
            PertukUserManager pertukUserManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            CurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _pertukUserManager = pertukUserManager;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<UserManagerResponseModel> SetUserStudentAsync(StudentUserRequestModel studentUserRequest)
        {
            if (studentUserRequest == null) throw new PertukApiException("Please provide required information!");

            var isUserExist = await _pertukUserManager.FindByIdAsync(studentUserRequest.UserId);
            if (isUserExist == null) throw new PertukApiException("User not found!");

            var isStudentUserExist = _unitOfWork.StudentUsers.GetById(studentUserRequest.UserId);
            if (isStudentUserExist != null) throw new PertukApiException("You're already a student!");

            try
            {
                await _unitOfWork.StudentUsers.Add(new StudentUsers { UserId = studentUserRequest.UserId });

                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw new PertukApiException();
            }

            // Succedded
            return new UserManagerResponseModel
            {
                Message = "You're now a Student!"
            };
        }

        public async Task<UserManagerResponseModel> SetUserTeacherAsync(TeacherUserRequestModel teacherUserRequest)
        {
            if (teacherUserRequest == null) throw new PertukApiException("Please provide required information!");

            var isUserExist = await _pertukUserManager.FindByIdAsync(teacherUserRequest.UserId);
            if (isUserExist == null) throw new PertukApiException("User not found!");

            var isStudentUserExist = _unitOfWork.TeacherUsers.GetById(teacherUserRequest.UserId);
            if (isStudentUserExist != null) throw new PertukApiException("You're already a teacher!");

            try
            {
                await _unitOfWork.TeacherUsers.Add(new TeacherUsers { UserId = teacherUserRequest.UserId });

                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw new PertukApiException();
            }

            return new UserManagerResponseModel
            {
                Message = "You're now a Teacher! Please wait for Pertuk Developer to activate your account!"
            };

            throw new PertukApiException();
        }

        public ApplicationUserDto GetUserDetail()
        {
            var userDetails = _pertukUserManager.GetUserDetailsAsync(_currentUser.Id);

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