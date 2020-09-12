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
        public StudentUsersRepository(PertukDbContext pertukDbContext) : base(pertukDbContext)
        {
        }

        public EntityState AddUsersAndStudent(StudentUsers entity)
        {
            using (var transaction = _pertukDbContext.BeginTransaction())
            {
                try
                {
                    _pertukDbContext.Users.Add(entity.User);
                    _pertukDbContext.StudentUsers.Add(entity);
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

        public EntityState AddStudent(StudentUsers studentUsers)
        {
            try
            {
                _pertukDbContext.StudentUsers.Add(new StudentUsers
                {
                    Grade = studentUsers.Grade,
                    UserId = studentUsers.UserId
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