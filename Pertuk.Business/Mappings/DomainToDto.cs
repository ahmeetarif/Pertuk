using AutoMapper;
using Pertuk.Dto.Models;
using Pertuk.Entities.Models;

namespace Pertuk.Business.Mappings
{
    public class DomainToDto : Profile
    {
        public DomainToDto()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<StudentUsers, StudentUsersDto>();
            CreateMap<TeacherUsers, TeacherUsersDto>();
        }
    }
}