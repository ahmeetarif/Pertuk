﻿using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.DataAccess.Repositories.Concrete;

namespace Pertuk.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public PertukDbContext _pertukDbContext;
        private IStudentUsersRepository _studentUsersRepository;
        private ITeacherUsersRepository _teacherUsersRepository;
        private IBannedUsersRepository _bannedUsersRepository;
        private IDeletedUsersRepository _deletedUsersRepository;
        public UnitOfWork(PertukDbContext pertukDbContext)
        {
            _pertukDbContext = pertukDbContext;
        }

        public IStudentUsersRepository StudentUsers => _studentUsersRepository = _studentUsersRepository ?? (new StudentUsersRepository(_pertukDbContext));

        public ITeacherUsersRepository TeacherUsers => _teacherUsersRepository = _teacherUsersRepository ?? new TeacherUsersRepository(_pertukDbContext);

        public IDeletedUsersRepository DeletedUsers => _deletedUsersRepository = _deletedUsersRepository ?? new DeletedUsersRepository(_pertukDbContext);

        public IBannedUsersRepository BannedUsers => _bannedUsersRepository = _bannedUsersRepository ?? new BannedUsersRepository(_pertukDbContext);

        public void Rollback()
        {
            _pertukDbContext.Rollback();
        }

        public int Commit()
        {
            return _pertukDbContext.SaveChanges();
        }

        public void Dispose()
        {
            _pertukDbContext.Dispose();
        }

        public void BeginTransaction()
        {
            _pertukDbContext.Database.BeginTransaction();
        }
    }
}