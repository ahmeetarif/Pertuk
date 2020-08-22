using Microsoft.EntityFrameworkCore;
using Pertuk.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Pertuk.Core.DataAccess.BaseRepository
{
    public class BaseRepository<TEntity, IdType, TContext> : IBaseRepository<TEntity, IdType>
        where TEntity : class, IEntity, new()
        where TContext : DbContext, new()
    {
        public virtual void Add(TEntity entity)
        {
            using (var dbContext = new TContext())
            {
                dbContext.Set<TEntity>().Add(entity);
            }
        }

        public virtual TEntity Get(IdType id)
        {
            using (var dbContext = new TContext())
            {
                return dbContext.Set<TEntity>().Find(id);
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var dbContext = new TContext())
            {
                return dbContext.Set<TEntity>().ToList();
            }
        }

        public virtual void Remove(IdType id)
        {
            using (var dbContext = new TContext())
            {
                TEntity entity = dbContext.Set<TEntity>().Find(id);
                dbContext.Set<TEntity>().Remove(entity);
            }
        }

        public virtual void Update(TEntity entity)
        {
            using (var dbContext = new TContext())
            {
                dbContext.Set<TEntity>().Update(entity);
            }
        }
    }
}
