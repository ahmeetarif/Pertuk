using System;
using AutoMapper;
using Pertuk.Contracts.V1.Requests.UserManager;
using Pertuk.Dto.Models;

namespace Pertuk.Business.Mappings
{
    public class RequestToDtoMapping : Profile
    {
        public RequestToDtoMapping()
        {
            CreateMap<StudentUserRequestModel, StudentUsersDto>().ReverseMap();
        }
    }
}
