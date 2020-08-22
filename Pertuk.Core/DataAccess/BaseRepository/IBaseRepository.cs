using Pertuk.Core.Entities;
using System.Collections.Generic;

namespace Pertuk.Core.DataAccess.BaseRepository
{
    public interface IBaseRepository<TEntity, IdType> where TEntity : class, IEntity, new()
    {
        IEnumerable<TEntity> GetAll();
        TEntity Get(IdType id);
        void Add(TEntity entity);
        void Remove(IdType id);
        void Update(TEntity entity);
    }
}
