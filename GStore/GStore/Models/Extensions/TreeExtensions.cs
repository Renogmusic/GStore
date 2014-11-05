using System;
using System.Collections.Generic;
using System.Linq;

namespace GStore.Models.Extensions
{
	// Stefan Cruysberghs, http://www.scip.be, March 2008

	/// <summary>
	/// Tree node class which contains a nested collection of Tree (hierarchy) nodes
	/// </summary>
	/// <typeparam name="T">Entity</typeparam>
	public class TreeNode<T> where T : class
	{
		public T Entity { get; set; }
		public IEnumerable<TreeNode<T>> ChildNodes { get; set; }
		public int Depth { get; set; }
	}

	public static class LinqExtensionMethods
	{
		private static System.Collections.Generic.IEnumerable<TreeNode<TEntity>> CreateTree<TEntity, TProperty>
		  (IEnumerable<TEntity> allItems, TEntity parentItem,
		  Func<TEntity, TProperty> idProperty, Func<TEntity, TProperty> parentIdProperty, int depth) where TEntity : class
		{
			IEnumerable<TEntity> children;

			if (parentItem == null)
				children = allItems.Where(i => parentIdProperty(i).Equals(default(TProperty)));
			else
				children = allItems.Where(i => parentIdProperty(i).Equals(idProperty(parentItem)));

			if (children.Count() > 0)
			{
				depth++;

				foreach (var item in children)
					yield return new TreeNode<TEntity>()
					{
						Entity = item,
						ChildNodes = CreateTree<TEntity, TProperty>
							(allItems, item, idProperty, parentIdProperty, depth),
						Depth = depth
					};
			}
		}

		/// <summary>
		/// LINQ IEnumerable AsTree() extension method
		/// </summary>
		/// <typeparam name="TEntity">Entity class</typeparam>
		/// <typeparam name="TProperty">Property of entity class</typeparam>
		/// <param name="allItems">Flat collection of entities</param>
		/// <param name="idProperty">Reference to Id/Key of entity</param>
		/// <param name="parentIdProperty">Reference to parent Id/Key</param>
		/// <returns>Hierarchical structure of entities</returns>
		public static System.Collections.Generic.IEnumerable<TreeNode<TEntity>> AsTree<TEntity, TProperty>
		  (this IEnumerable<TEntity> allItems, Func<TEntity, TProperty> idProperty, Func<TEntity, TProperty> parentIdProperty)
		  where TEntity : class
		{
			return CreateTree(allItems, default(TEntity), idProperty, parentIdProperty, 0);
		}

		public static TreeNode<TEntity> FindEntity<TEntity>(this List<TreeNode<TEntity>> tree, TEntity entity) where TEntity : class
		{
			foreach (TreeNode<TEntity> item in tree)
			{
				if (item.Entity.Equals(entity))
				{
					return item;
				}
				foreach (TreeNode<TEntity> childItem in item.ChildNodes)
				{
					if (childItem.Entity.Equals(entity))
					{
						return childItem;
					}
					TreeNode<TEntity> found = FindEntityRecursive(childItem, entity);
					if (found != null)
					{
						return found;
					}
		 
				}
			}
			return null;
		}

		private static TreeNode<TEntity> FindEntityRecursive<TEntity>(this TreeNode<TEntity> node, TEntity entity) where TEntity : class
		{
			if (node.Entity.Equals(entity))
			{
				return node;
			}
			foreach (TreeNode<TEntity> childItem in node.ChildNodes)
			{
				if (childItem.Entity.Equals(entity))
				{
					return childItem;
				}
				TreeNode<TEntity> found = FindEntityRecursive(childItem, entity);
				if (found != null)
				{
					return found;
				}

			}
			return null;
		}


	}
}