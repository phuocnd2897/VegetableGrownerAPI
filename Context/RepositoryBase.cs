using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace VG.Context
{
    public class RepositoryBase<T, K> : IRepository<T, K> where T : class
    {
        private readonly VGContext dbContext;
        private readonly DbSet<T> dbSet;
        private readonly IUnitOfWork _unitOfWork;
        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this.dbContext = this._unitOfWork.dbContext;
            dbSet = this.dbContext.Set<T>();
        }
        public T Add(T entity)
        {
            return (dbSet.Add(entity)).Entity;
        }

        public void Add(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public IQueryable<T> AsNoTracking()
        {
            return dbSet.AsNoTracking();
        }

        public void BeginTransaction()
        {
            if (this.dbContext.Database.CurrentTransaction == null)
                this.dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            this.dbContext.SaveChanges();
        }

        public void CommitTransaction()
        {
            this.dbContext.Database.CurrentTransaction?.Commit();
            this.dbContext.Database.CurrentTransaction?.Dispose();
        }

        public bool Contains(Expression<Func<T, bool>> where)
        {
            return dbSet.Count<T>(where) > 0;
        }

        public int Count(Expression<Func<T, bool>> where)
        {
            return dbSet.Count(where);
        }

        public void Delete(K id)
        {
            var entity = dbSet.Find(id);
            if (entity != null)
                dbSet.Remove(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbSet.Remove(obj);
        }

        public IEnumerable<T> GetAll(string[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = this.dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                return query.AsQueryable();
            }
            return this.dbSet.AsQueryable();
        }

        public DbSet<U> GetDbSet<U>() where U : class
        {
            return this.dbContext.Set<U>();
        }

        public IEnumerable<U> GetDbSet<U>(Expression<Func<U, bool>> where, string[] includes = null, Expression<Func<U, DateTime>> partionColumn = null, string strfilegroup = null) where U : class
        {
            IQueryable<U> query;
            var _dbSet = this.dbContext.Set<U>();
            if (includes != null && includes.Count() > 0)
            {
                query = _dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
            }
            else
                query = _dbSet;
            return query.Where<U>(where).AsQueryable<U>();
        }

        public int GetMaxStt(Expression<Func<T, int>> exp)
        {
            if (dbSet.Count() > 0)
                return dbSet.Max(exp);
            return 0;
        }

        public IEnumerable<T> GetMulti(Expression<Func<T, bool>> where, string[] includes = null, Expression<Func<T, DateTime>> partionColumn = null, string strfilegroup = null)
        {
            IQueryable<T> query;
            if (includes != null && includes.Count() > 0)
            {
                query = dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
            }
            else
                query = dbSet;
            return query.Where<T>(where).AsQueryable<T>();
        }

        public IEnumerable<T> GetMultiPaging(Expression<Func<T, bool>> where, Expression<Func<T, string>> sort, out int total, int index = 0, int size = 50, string[] includes = null)
        {
            int skipCount = index * size;
            IQueryable<T> _resetSet;
            if (includes != null && includes.Count() > 0)
            {
                var query = dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
                _resetSet = where != null ? query.Where<T>(where).AsQueryable() : query.AsQueryable();
            }
            else
            {
                _resetSet = where != null ? dbSet.Where<T>(where).AsQueryable() : dbSet.AsQueryable();
            }
            _resetSet = _resetSet.OrderBy(sort);
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public T GetSingle(K id)
        {
            return dbSet.Find(id);
        }

        public T GetSingle(Expression<Func<T, bool>> where, string[] includes = null, Expression<Func<T, DateTime>> partionColumn = null, string strfilegroup = null)
        {
            IQueryable<T> query;
            if (includes != null && includes.Count() > 0)
            {
                query = dbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query = query.Include(include);
            }
            else
                query = dbSet;

            return query.FirstOrDefault(where);
        }

        public void RollbackTransaction()
        {
            this.dbContext.Database.CurrentTransaction?.Rollback();
            this.dbContext.Database.CurrentTransaction?.Dispose();
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);
            this.dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Update(IEnumerable<T> entities)
        {
            foreach (var e in entities)
                dbContext.Entry(e).State = EntityState.Modified;
        }
    }
}
