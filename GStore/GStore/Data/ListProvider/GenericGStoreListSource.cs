using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using GStore.Models;

namespace GStore.Data.ListProvider
{
	/// <summary>
	/// Generic wrapper for GStore List objects as data sources for empty DB and testing repository pattern
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class GenericGStoreListSourceRepository<TEntity> : IGStoreRepository<TEntity> where TEntity : Models.BaseClasses.GStoreEntity, new()
	{
		/// <summary>
		/// list for simplified mocks
		/// </summary>
		private static List<TEntity> _list = null;

		public GenericGStoreListSourceRepository(List<TEntity> list)
		{
			_list = list;
		}

		public GenericGStoreListSourceRepository(IEnumerable<TEntity> listData)
		{
			_list = new List<TEntity>(listData);
		}

		public GenericGStoreListSourceRepository()
		{
			if (_list == null)
			{
				_list = new List<TEntity>();
			}
		}

		public virtual TEntity Create()
		{
			return new TEntity();
		}

		public virtual TEntity Create(TEntity valuesToCopy)
		{
			return valuesToCopy;
		}

		public virtual List<TEntity> ListData()
		{
			return _list;
		}

		public virtual IQueryable<TEntity> All()
		{
			return _list.AsQueryable();
		}

		public virtual bool IsEmpty()
		{
			return _list.Count == 0;
		}


		public virtual IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
		{
			IQueryable<TEntity> query = _list.AsQueryable();
			foreach (var includeProperty in includeProperties)
			{
				query = query.Include(includeProperty);
			}
			return query;
		}

		public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
		{
			return _list.AsQueryable().Where(predicate);
		}

		public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
		{
			return _list.AsQueryable().Single(predicate);
		}

		public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
		{
			return _list.AsQueryable().SingleOrDefault(predicate);
		}

		public virtual TEntity FindById(int id)
		{
			return _list.Find(entity => GetKeyFieldValue(entity) == id);
		}

		public virtual TEntity Add(TEntity entity)
		{
			SetKeyFieldValue(entity, _list.Count() + 1);
			_list.Add(entity);
			
			return entity;
		}

		public virtual bool DeleteById(int id, bool throwErrorIfNotFound = false)
		{
			TEntity entity = FindById(id);
			if (entity == null)
			{
				if (throwErrorIfNotFound)
				{
					throw new ApplicationException("Could not find entity by its key value, id: " + id + " entity: " + entity.GetType().Name);
				}
				return false;
			}
			_list.Remove(entity);
			return true;
		}

		public virtual bool Delete(TEntity entity, bool throwErrorIfNotFound = true)
		{
			if (_list.Contains(entity))
			{
				_list.Remove(entity);
				return true;
			}
			return DeleteById(GetKeyFieldValue(entity), throwErrorIfNotFound);
		}

		public virtual TEntity Update(TEntity entity)
		{
			TEntity existingEntity = this.FindById(GetKeyFieldValue(entity));
			_list.Remove(existingEntity);
			_list.Add(entity);
			return entity;
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
			Type type = entity.GetType();
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