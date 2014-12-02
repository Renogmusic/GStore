using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using GStore.Models;
using GStore.Data;

namespace GStore.Data.EntityFrameworkCodeFirstProvider
{
	/// <summary>
	/// Generic wrapper for GStore data tables and simple common operations
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class GenericGStoreEFEntity<TEntity> : IGStoreRepository<TEntity> where TEntity : Models.BaseClasses.GStoreEntity, new()
	{
		private DbContext _context = null;
		private DbSet<TEntity> _dbSet = null;

		public GenericGStoreEFEntity(DbContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			_context = context;
			_dbSet = context.Set<TEntity>();
		}

		public virtual bool IsEmpty()
		{
			try
			{
				return !_dbSet.Any();
			}
			catch (Exception ex)
			{
				throw new Exceptions.DatabaseErrorException("IsEmpty database query failed. Check database initializer and database connection string", ex);
			}
		}

		public virtual TEntity Create()
		{
			return _dbSet.Create();
		}

		/// <summary>
		/// Does a shallow copy of properties from a object into a new entity object dynamic proxy
		/// </summary>
		/// <param name="valuesToCopy"></param>
		/// <returns></returns>
		public virtual TEntity Create(TEntity valuesToCopy)
		{
			if (valuesToCopy == null)
			{
				throw new ArgumentNullException("valuesToCopy");
			}

			return CreateNewProxyFromPoco(valuesToCopy);
		}

		public virtual TEntity Add(TEntity entity)
		{
			//check if entity is a POCO or a dynamic proxy
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			if (entity.IsPoco())
			{
				entity = CreateNewProxyFromPoco(entity);
			}
			return _dbSet.Add(entity);
		}

		public virtual TEntity FindById(int id)
		{
			return _dbSet.Find(id);
		}

		public virtual bool DeleteById(int id, bool throwErrorIfNotFound = false)
		{
			var entity = _dbSet.Find(id);
			if (entity == null && throwErrorIfNotFound)
			{
				throw new ApplicationException("Record not found to delete. " + typeof(TEntity).Name + " id: " + id);
			}
			if (entity == null)
			{
				return false;
			}
			_dbSet.Remove(entity);
			return true;
		}

		public virtual bool Delete(TEntity entity, bool throwErrorIfNotFound = true)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			////note: there may an issue if entity is a POCO; might need to call DeleteById
			//if (_dbSet.Contains(entity))
			//{
			//	_dbSet.Remove(entity);
			//	return true;
			//}
			return DeleteById(GetKeyFieldValue(entity), throwErrorIfNotFound);
		}

		public virtual TEntity Update(TEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			//check if entity is a POCO or a dynamic proxy
			TEntity proxy = null;
			if (entity.IsProxy())
			{
				proxy = entity;
			}
			else
			{
				//find a dynamic proxy with the same key
				TEntity proxyOriginalValues = _dbSet.Find(GetKeyFieldValue(entity));
				if (proxyOriginalValues == null)
				{
					throw new ApplicationException("Could not find matching entity to update. " + KeyFieldPropertyName(entity) + ": " + GetKeyFieldValue(entity));
				}
				proxy = entity.CopyValuesToEntity(proxyOriginalValues);
			}
			_context.Entry<TEntity>(proxy).State = EntityState.Modified;

			return proxy;
		}

		public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
		{
			return _dbSet.Single(predicate);
		}

		public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return _dbSet.SingleOrDefault(predicate);
		}

		public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
		{
			return _dbSet.Where(predicate);
		}

		public virtual IQueryable<TEntity> All()
		{
			return _dbSet;
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

		public virtual DbSet<TEntity> DbSet()
		{
			return _dbSet;
		}

		public virtual DbContext DbContext()
		{
			return _context;
		}

		public virtual TEntity CreateNewProxyFromPoco(TEntity poco)
		{
			TEntity proxy = _dbSet.Create();
			proxy = proxy.UpdateValuesFromEntity(poco);
			return proxy;
		}

		public virtual TEntity CreateNewPocoFromProxy(TEntity proxy)
		{
			TEntity poco = new TEntity();
			poco = poco.UpdateValuesFromEntity(proxy);
			return poco;
		}

		public void SetKeyFieldValue(TEntity entity, int id)
		{
			string keyField = KeyFieldPropertyName(entity);
			entity.GetType().GetProperty(keyField).SetValue(entity, id);
		}

		public int GetKeyFieldValue(TEntity entity)
		{
			string keyField = KeyFieldPropertyName(entity);
			object keyFieldValue = entity.GetType().GetProperty(keyField).GetValue(entity);
			return (int)keyFieldValue;
		}

		public string KeyFieldPropertyName(TEntity entity)
		{
			Type type = null;
			if (entity.IsProxy())
			{
				type = entity.GetPocoType();
			}
			else
			{
				type = entity.GetType();
			}

			string entityName = type.Name.ToLower();
			foreach (System.Reflection.PropertyInfo prop in type.GetProperties())
			{
				if (prop.Name.ToLower() == entityName + "id")
				{
					return prop.Name;
				}
				if (prop.Name.ToLower() == entityName + "_id")
				{
					return prop.Name;
				}
				if (prop.Name.ToLower() == "id")
				{
					return prop.Name;
				}
			}

			throw new ApplicationException("Cannot find ID field for entity: " + type.Name);

		}

	}
}