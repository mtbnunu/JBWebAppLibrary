using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBWebappLibrary.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<TEntity> Repository<TEntity>() where TEntity : class;

        void ExecCommand(string procedureName);
        
        void Save();
    }
}
