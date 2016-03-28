using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace JBWebappLibrary.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null, int? count = null,
            int? skip = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] properties);

        TEntity GetByID(object id, params Expression<Func<TEntity, object>>[] properties);

        void Insert(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);

        void AddOrUpdate(TEntity entity);

        int Count(Expression<Func<TEntity, bool>> filter = null);
    }
}
