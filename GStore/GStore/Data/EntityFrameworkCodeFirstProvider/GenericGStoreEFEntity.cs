using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using GStore.Models;

namespace GStore.Data.EntityFrameworkCodeFirstProvider
{
	/// <summary>
	/// Generic wrapper for GStore data tables and simple common operations
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class GenericGStoreEFEntity<TEntity> : IGStoreRepository<TEntity> where TEntity : class
	{
		private DbContext _context = null;
		private DbSet<TEntity> _dbSet = null;

		public GenericGStoreEFEntity(DbContext context)
		{
			_context = context;
			_dbSet = context.Set<TEntity>();
		}

		public virtual DbSet<TEntity> DbSet()
		{
			return _dbSet;
		}

		public virtual DbContext DbContext()
		{
			return _context;
		}

		public virtual IQueryable<TEntity> All()
		{
			return _dbSet;
		}

		public virtual bool IsEmpty()
		{
			return !_dbSet.Any();
		}

		public virtual IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			IQueryable<TEntity> query = _dbSet;
			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}
			return query;
		}

		public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
		{
			return _dbSet.Where(predicate);
		}

		public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
		{
			return _dbSet.Single(predicate);
		}

		public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return _dbSet.SingleOrDefault(predicate);
		}

		public virtual TEntity FindById(int id)
		{
			return _dbSet.Find(id);
		}

		public virtual TEntity Add(TEntity entity)
		{
			return _dbSet.Add(entity);
		}

		public virtual void Delete(TEntity entity)
		{
			_dbSet.Remove(entity);
		}

		public virtual void DeleteById(int id, bool throwErrorIfNotFound = false)
		{
			var entity = _dbSet.Find(id);
			if (entity == null && throwErrorIfNotFound)
			{
				throw new ApplicationException("Record not found to delete. " + typeof(TEntity).Name + " id: " + id);
			}
			_dbSet.Remove(entity);
		}

		public virtual TEntity Create()
		{
			return _dbSet.Create();
		}

		public virtual void Update(TEntity entity)
		{
			_dbSet.Attach(entity);
			_context.Entry<TEntity>(entity).State = EntityState.Modified;
		}
	}
}