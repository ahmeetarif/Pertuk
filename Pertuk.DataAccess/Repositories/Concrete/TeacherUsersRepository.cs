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
        public TeacherUsersRepository(PertukDbContext pertukDbContext)
            : base(pertukDbContext)
        {
        }

        public EntityState AddUsersAndTeacher(TeacherUsers entity)
        {
            using (var transaction = _pertukDbContext.BeginTransaction())
            {
                try
                {
                    _pertukDbContext.Users.Add(entity.User);
                    _pertukDbContext.TeacherUsers.Add(entity);
                    _pertukDbContext.Commit();
                    return EntityState.Added;
                }
                catch (Exception)
                {
                    _pertukDbContext.Rollback();
                    return EntityState.Unchanged;
                }
            }
        }

        public EntityState AddTeacher(TeacherUsers teacherUsers)
        {
            try
            {
                _pertukDbContext.TeacherUsers.Add(new TeacherUsers
                {
                    UserId = teacherUsers.UserId
                });
                _pertukDbContext.SaveChanges();
                return EntityState.Added;
            }
            catch (Exception)
            {
                return EntityState.Unchanged;
            }
        }

    }
}
