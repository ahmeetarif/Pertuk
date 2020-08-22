using Microsoft.EntityFrameworkCore;
using Pertuk.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pertuk.DataAccess.BaseRepository
{
    public class BaseRepository<TEntity, IdType> : IBaseRepository<TEntity, IdType>
        where TEntity : class, IEntity, new()
    {
        protected readonly PertukDbContext _pertukDbContext;
        protected readonly DbSet<TEntity> table = null;
        public BaseRepository(PertukDbContext pertukDbContext)
        {
            _pertukDbContext = pertukDbContext;
            table = _pertukDbContext.Set<TEntity>();
        }

        public virtual Task<EntityState> Add(TEntity entity)
        {
            var res = table.Add(entity);
            return Task.FromResult(res.State);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return table.ToList();
        }

        public virtual TEntity GetById(IdType id)
        {
            TEntity entity = table.Find(id);
            return entity;
        }

        public virtual EntityState Remove(IdType id)
        {
            TEntity entity = table.Find(id);
            var result = table.Remove(entity);
            return result.State;
        }

        public virtual EntityState Update(TEntity entity)
        {
            var result = table.Update(entity);
            return result.State;
        }
    }
}
