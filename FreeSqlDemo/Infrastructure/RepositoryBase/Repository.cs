using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using FreeSqlDemo.Infrastructure.DomainBase;
using FreeSqlDemo.Infrastructure.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace FreeSqlDemo.Infrastructure.Repository
{
    /// <summary>
    /// 做这个通用仓储主要是用Uow来作为数据库依赖
    /// 这样可以保证每次请求都通过同一个uow来请求并同时提交
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnitOfWorkRepository<T, TKey> : IRepository<T>, IRepKey where T : EntityBase<TKey>
    {
        private readonly BaseRepository<T> _baseRep;
        public UnitOfWorkRepository(IServiceProvider service)
        {
            _baseRep = service.GetRequiredService<IFreeSql>().GetRepository<T>();
            _baseRep.UnitOfWork = service.GetRequiredService<IRepositoryUnitOfWork>();
            var terant = service.GetService<CurrentUser>()?.Terant;
            if (terant != null)
            {
                _baseRep.DataFilter.Apply($"{nameof(Terant)}", t => t.TerantId == terant.Id);
            }

        }

        public void Dispose()
        {
            _baseRep.Dispose();
        }

        public void AsType(Type entityType)
        {
            _baseRep.AsType(entityType);
        }

        public Type EntityType => _baseRep.EntityType;

        public IUnitOfWork UnitOfWork
        {
            get => _baseRep.UnitOfWork;
            set { }
        }

        public IFreeSql Orm
        {
            get => _baseRep.Orm;
        }
        public ISelect<T> Where(Expression<Func<T, bool>> exp)
        {
            return _baseRep.Where(exp);
        }

        public ISelect<T> WhereIf(bool condition, Expression<Func<T, bool>> exp)
        {
            return _baseRep.WhereIf(condition, exp);
        }

        public IDataFilter<T> DataFilter => _baseRep.DataFilter;

        public ISelect<T> Select => _baseRep.Select;

        public T Insert(T entity)
        {
            return _baseRep.Insert(entity);
        }

        public List<T> Insert(IEnumerable<T> entitys)
        {
            return _baseRep.Insert(entitys);
        }

        public Task<T> InsertAsync(T entity)
        {
            return _baseRep.InsertAsync(entity);
        }

        public Task<List<T>> InsertAsync(IEnumerable<T> entitys)
        {
            return _baseRep.InsertAsync(entitys);
        }

        public void Attach(T entity)
        {
            _baseRep.Attach(entity);
        }

        public void Attach(IEnumerable<T> entity)
        {
            _baseRep.Attach(entity);
        }

        public int Update(T entity)
        {
            return _baseRep.Update(entity);
        }

        public int Update(IEnumerable<T> entitys)
        {
            return _baseRep.Update(entitys);
        }

        public Task<int> UpdateAsync(T entity)
        {
            return _baseRep.UpdateAsync(entity);
        }

        public Task<int> UpdateAsync(IEnumerable<T> entitys)
        {
            return _baseRep.UpdateAsync(entitys);
        }

        public T InsertOrUpdate(T entity)
        {
            return _baseRep.InsertOrUpdate(entity);
        }

        public Task<T> InsertOrUpdateAsync(T entity)
        {
            return _baseRep.InsertOrUpdateAsync(entity);
        }

        public int Delete(T entity)
        {
            return _baseRep.Delete(entity);
        }

        public int Delete(IEnumerable<T> entitys)
        {
            return _baseRep.Delete(entitys);
        }

        public Task<int> DeleteAsync(T entity)
        {
            return _baseRep.DeleteAsync(entity);
        }

        public Task<int> DeleteAsync(IEnumerable<T> entitys)
        {
            return _baseRep.DeleteAsync(entitys);
        }

        public IUpdate<T> UpdateDiy => _baseRep.UpdateDiy;
        public int Delete(Expression<Func<T, bool>> predicate)
        {
            return _baseRep.Delete(predicate);
        }

        public Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return _baseRep.DeleteAsync(predicate);
        }
    }
}
