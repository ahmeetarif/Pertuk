using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pertuk.DataAccess.BaseRepository;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.Entities.Models;
using System;
using System.Threading.Tasks;

namespace Pertuk.DataAccess.Repositories.Concrete
{
    public class TeacherUsersRepository : BaseRepository<TeacherUsers, string>, ITeacherUsersRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public TeacherUsersRepository(PertukDbContext pertukDbContext,
                                      UserManager<ApplicationUser> userManager)
            : base(pertukDbContext)
        {
            _userManager = userManager;
        }

        public override async Task<EntityState> Add(TeacherUsers entity)
        {
            using (var transaction = _pertukDbContext.BeginTransaction())
            {
                try
                {
                    var result = await _userManager.CreateAsync(entity.User);
                    if (result.Succeeded)
                    {
                        _pertukDbContext.TeacherUsers.Add(entity);
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
