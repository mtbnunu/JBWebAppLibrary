using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBWebappLibrary.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext context;
        private bool disposed;

        public UnitOfWork(DbContext context)
        {
            this.context = context;
            this.disposed = false;
        }

        public IBaseRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return new BaseRepository<TEntity>(context);    
        }

        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void ExecCommand(string procedureName)
        {
            context.Database.ExecuteSqlCommand(procedureName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
