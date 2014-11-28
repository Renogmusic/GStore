using GStore.Models;
using GStore.Models.BaseClasses;
using System;
using System.Web;

namespace GStore.Data
{
	public static class GStoreDBExtensions
	{
		public static void SeedDatabase(this IGstoreDb db)
		{
			SeedDataExtensions.AddSeedData(db);
		}
		public static StoreBinding AutoMapBinding(this IGstoreDb db, Controllers.BaseClass.BaseController baseController)
		{
			if (Properties.Settings.Current.AppEnableBindingAutoMapCatchAll)
			{
				return db.AutoMapBindingToCatchAll(baseController);
			}
			else
			{
				return db.AutoMapBindingToCurrentUrl(baseController);
			}
		}

		public static StoreBinding AutoMapBindingToCurrentUrl(this IGstoreDb db, Controllers.BaseClass.BaseController baseController)
		{
			return SeedDataExtensions.CreatAutoMapStoreBindingToCurrentUrl(db, baseController);
		}

		public static StoreBinding AutoMapBindingToCatchAll(this IGstoreDb db, Controllers.BaseClass.BaseController baseController)
		{
			return SeedDataExtensions.CreatAutoMapStoreBindingToCatchAll(db, baseController);
		}

		/// <summary>
		/// Returns the best guess for the user profile to use auto-map
		/// </summary>
		/// <param name="db"></param>
		/// <returns></returns>
		public static UserProfile AutoMapUserProfileTarget(this IGstoreDb db)
		{
			return SeedDataExtensions.SeedAutoMapUserBestGuess(db);
		}

		/// <summary>
		/// Returns the best guess for the store front to activate for auto-map
		/// </summary>
		/// <param name="db"></param>
		/// <returns></returns>
		public static StoreFront AutoMapStoreFrontTarget(this IGstoreDb db)
		{
			return SeedDataExtensions.SeedAutoMapStoreFrontBestGuess(db);
		}

		public static Page AutoCreateHomePage(this IGstoreDb db, HttpRequestBase request, StoreFront storeFront, Controllers.BaseClass.BaseController baseController)
		{
			return SeedDataExtensions.CreateAutoHomePage(db, request, storeFront, baseController);
		}

		public static void SaveEntityToFile<TEntity>(this TEntity entity, string folder, string fileName) where TEntity : Models.BaseClasses.GStoreEntity, new()
		{
			if (!System.IO.Directory.Exists(folder))
			{
				System.IO.Directory.CreateDirectory(folder);
				System.Diagnostics.Trace.WriteLine("--SaveEntityToFile File System: Created folder: " + folder);
			}

			System.Xml.XmlWriterSettings xmlSettings = new System.Xml.XmlWriterSettings()
			{
				Async = false,
				CloseOutput = true,
				Encoding = System.Text.Encoding.UTF8,
				Indent = true,
				IndentChars = "\t",
				NewLineHandling = System.Xml.NewLineHandling.None,
				WriteEndDocumentOnClose = true
			};

			System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(folder + "\\" + fileName, xmlSettings);

			System.Xml.Serialization.XmlSerializer xmlSerializer = null;
			try
			{
				TEntity poco = null;
				if (entity.IsPoco())
				{
					poco = entity;
				}
				else
				{
					poco = entity.GetPoco();
				}
				xmlSerializer = new System.Xml.Serialization.XmlSerializer(poco.GetType());
				xmlSerializer.Serialize(xmlWriter, poco);
				System.Diagnostics.Trace.WriteLine("--SaveEntityToFile: Wrote entity type " + entity.GetType().FullName + " to Folder: " + folder + " FileName: " + fileName);
			}
			catch (Exception ex)
			{
				string message = "Error in SaveEntityToFile: Error saving entity type " + entity.GetType().FullName + " to Folder: " + folder + " FileName: " + fileName + " Exception: " + ex.ToString();
				System.Diagnostics.Trace.WriteLine("--" + message);
				throw new ApplicationException(message, ex);
			}
			finally
			{
				xmlWriter.Flush();
				xmlWriter.Close();
				xmlWriter.Dispose();
			}
		}

		/// <summary>
		/// Untested: reads an entity from an XML file and returns the POCO object
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static TEntity ReadEntityFromFile<TEntity>(string filePath) where TEntity : GStoreEntity, new()
		{
			System.Xml.Serialization.XmlSerializer xmlSerializer = null;
			System.IO.FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
			object result = xmlSerializer.Deserialize(fileStream);
			return (TEntity)(result);
		}

		public static TEntity GetPoco<TEntity>(this TEntity entity) where TEntity : GStoreEntity, new()
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (entity.IsPoco())
			{
				return entity;
			}

			TEntity newPoco = new TEntity();
			newPoco.UpdateValuesFromEntity(entity);
			return newPoco;
		}

		public static Type GetPocoType<TEntity>(this TEntity entity) where TEntity : GStoreEntity, new()
		{
			return typeof(TEntity);
		}

		/// <summary>
		/// Returns true if this object is an entity framework proxy object, not a POCO
		/// </summary>
		/// <param name="testObject"></param>
		/// <returns></returns>
		public static bool IsProxy(this GStoreEntity entity)
		{
			//check if the current object type is the base type, or an entity parent type
			return (entity != null) && (System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(entity.GetType()) != entity.GetType());
		}

		/// <summary>
		/// Returns true if this object is a POCO, not an entity framework proxy object
		/// </summary>
		/// <param name="testObject"></param>
		/// <returns></returns>
		public static bool IsPoco(this GStoreEntity entity)
		{
			return !entity.IsProxy();
		}

	}
}