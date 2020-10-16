using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pertuk.Business.CustomIdentity;
using Pertuk.Business.Services.Abstract;
using Pertuk.Common.Exceptions;
using Pertuk.Contracts.Requests.UserManager;
using Pertuk.Contracts.Responses.UserManager;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.DataAccess.UnitOfWork;
using Pertuk.Entities.Models;

namespace Pertuk.Business.Services.Concrete
{
    public class UserManagerService : IUserManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PertukUserManager _pertukUserManager;
        private readonly IUploadImageService _uploadImageService;
        public UserManagerService(
            PertukUserManager pertukUserManager,
            IUploadImageService uploadImageService,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _pertukUserManager = pertukUserManager;
            _uploadImageService = uploadImageService;
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

        public async Task<UsersDetailsResponseModel> GetUserDetail(string userId)
        {
            if (userId == null) throw new PertukApiException("Please provide required information!");

            var userDetail = await _pertukUserManager.FindByIdAsync(userId);
            if (userDetail == null) throw new PertukApiException("User not found!");

            return new UsersDetailsResponseModel
            {

            };

        }

        #region Private Functions

        private void Users()
        {

        }

        #endregion
    }
}