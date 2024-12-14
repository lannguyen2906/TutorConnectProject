using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Data;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;

namespace TutoRum.Data.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        #region Properties
        private ApplicationDbContext dataContext;
        private readonly DbSet<T> dbSet;

        protected IDbFactory DbFactory { get; private set; }

        protected ApplicationDbContext DbContext
        {
            get { return dataContext ?? (dataContext = DbFactory.Init()); }
        }
        #endregion

        protected RepositoryBase(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
            dbSet = DbContext.Set<T>();
        }

        public virtual T Add(T entity)
        {
            dbSet.Add(entity);
            return entity;
        }

        public virtual T Delete(int id)
        {
            var entity = dbSet.Find(id);
            if (entity != null)
            {
                dbSet.Remove(entity);
            }
            return entity;
        }

        public virtual void DeleteMulti(Expression<Func<T, bool>> where)
        {
            var entities = dbSet.Where(where).ToList();
            foreach (var entity in entities)
            {
                dbSet.Remove(entity);
            }
        }

        public virtual T GetSingleById(int? id)
        {
            return dbSet.Find(id);
        }

        public virtual T GetSingleByGuId(Guid id)
        {
            return dbSet.Find(id);
        }


        public virtual T GetSingleByCondition(Expression<Func<T, bool>> expression, string[] includes = null)
        {
            IQueryable<T> query = dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.FirstOrDefault(expression);
        }

        public virtual IEnumerable<T> GetAll(string[] includes = null)
        {
            IQueryable<T> query = dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.ToList();
        }

        public virtual IEnumerable<T> GetMulti(Expression<Func<T, bool>> predicate, string[] includes = null)
        {
            IQueryable<T> query = dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.Where(predicate).ToList();
        }

        public virtual IEnumerable<T> GetMultiPaging(
            Expression<Func<T, bool>> predicate,
            out int total,
            int index = 0,
            int size = 20,
            string[] includes = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = dbSet;

            // Bao gồm các bảng liên quan
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Áp dụng bộ lọc predicate
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Đếm tổng số bản ghi
            total = query.Count();

            // Áp dụng sắp xếp nếu có
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Trả về dữ liệu theo phân trang
            return query.Skip(index * size).Take(size).ToList();
        }



        public virtual int Count(Expression<Func<T, bool>> where)
        {
            return dbSet.Count(where);
        }

        public virtual bool CheckContains(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }

        public virtual void Update(T entity)
        {
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            dataContext.Set<T>().AddRange(entities);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.FirstOrDefaultAsync(predicate);
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return await DbContext.Database.BeginTransactionAsync(isolationLevel);
        }

        public IExecutionStrategy GetExecutionStrategy()
        {
            return DbContext.Database.CreateExecutionStrategy();
        }


        public virtual IQueryable<T> GetMultiAsQueryable(Expression<Func<T, bool>> predicate, string[] includes = null)
        {
            IQueryable<T> query = dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.Where(predicate); // Return IQueryable<T> without calling ToList()
        }

    }
}
