using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;
using System;
using System.Threading.Tasks;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class StudentUsersRepository : BaseRepository<StudentUsers, string>, IStudentUsersRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentUsersRepository(UserManager<ApplicationUser> userManager, PertukDbContext pertukDbContext) : base(pertukDbContext)
        {
            _userManager = userManager;
        }

        public override async Task<EntityState> Add(StudentUsers entity)
        {
            using (var transaction = _pertukDbContext.BeginTransaction())
            {
                try
                {
                    var result = await _userManager.CreateAsync(entity.User);
                    if (result.Succeeded)
                    {
                        _pertukDbContext.StudentUsers.Add(entity);
                        _pertukDbContext.Commit();
                        return EntityState.Added;
                    }
                    else
                    {
                        return EntityState.Unchanged;
                    }
                }
                catch (Exception)
                {
                    _pertukDbContext.Rollback();
                    return EntityState.Unchanged;
                }
            }
        }
    }
}