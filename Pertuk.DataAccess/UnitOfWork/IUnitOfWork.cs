using System;
using Pertuk.DataAccess.Repositories.Abstract;

namespace Pertuk.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentUsersRepository StudentUsers { get; }
        ITeacherUsersRepository TeacherUsers { get; }
        IBannedUsersRepository BannedUsers { get; }
        IDeletedUsersRepository DeletedUsers { get; }
        void BeginTransaction();
        void Rollback();
        int Commit();
    }
}
