using AutoMapper;
using Pertuk.Dto.Models;
using Pertuk.Entities.Models;

namespace Pertuk.Business.Mappings
{
    public class DomainDtoMapping : Profile
    {
        public DomainDtoMapping()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<ApplicationUserDto, ApplicationUser>();
            CreateMap<StudentUsers, StudentUsersDto>();
            CreateMap<StudentUsersDto, StudentUsers>();
            CreateMap<TeacherUsers, TeacherUsersDto>();
            CreateMap<TeacherUsersDto, TeacherUsers>();
        }
    }
}