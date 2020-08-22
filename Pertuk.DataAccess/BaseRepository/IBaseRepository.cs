using Microsoft.EntityFrameworkCore;
using Pertuk.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pertuk.DataAccess.BaseRepository
{
    public interface IBaseRepository<TEntity, IdType> where TEntity : class, IEntity, new()
    {
        IEnumerable<TEntity> GetAll();
        TEntity GetById(IdType id);
        Task<EntityState> Add(TEntity entity);
        EntityState Remove(IdType id);
        EntityState Update(TEntity entity);
    }
}
