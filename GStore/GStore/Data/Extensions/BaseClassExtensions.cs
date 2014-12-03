using GStore.Models;
using GStore.Models.BaseClasses;
using System;
using System.Web;

namespace GStore.Data
{
	public static class BaseClassExtensions
	{
		/// <summary>
		/// Sets defaults for the record (for new records)
		/// </summary>
		public static void SetDefaults(this AuditFieldsAllRequired record, UserProfile userProfile)
		{
			record.UpdateAuditFields(userProfile);
		}

		/// <summary>
		/// Updates UpdateDateTimeUtc and updateuserid to current, fills in createDateTimeUtc and CreatedBy if null
		/// </summary>
		public static void UpdateAuditFields(this AuditFieldsAllRequired record, UserProfile userProfile)
		{
			if (userProfile == null && record.CreatedBy == null && record.UpdatedBy == null)
			{
				throw new ApplicationException("User profile cannot be null for CreateUser/UpdateUser using AuditableFieldsAllRequired");
			}
			record.UpdateAuditFields(userProfile.UserProfileId);
		}

		public static void UpdateAuditFields(this AuditFieldsAllRequired record, int userProfileId)
		{
			//new record fill create info
			if (record.CreateDateTimeUtc == null || record.CreateDateTimeUtc == DateTime.MinValue)
			{
				record.CreateDateTimeUtc = DateTime.UtcNow;
			}
			if (record.CreatedBy == null)
			{
				record.CreatedBy_UserProfileId = userProfileId;
			}
			record.UpdateDateTimeUtc = DateTime.UtcNow;
			record.UpdatedBy_UserProfileId = userProfileId;
		}

		public static string StoreFrontVirtualDirectoryToMap(this StoreFrontRecord record, string applicationPath)
		{
			return record.ClientVirtualDirectoryToMap(applicationPath) + "/StoreFronts/" + HttpUtility.UrlEncode(record.StoreFront.Folder);
		}

		public static bool IsActiveDirect(this GStoreEntity record)
		{
			return record.IsActiveDirect(DateTime.UtcNow);
		}
		public static bool IsActiveDirect(this GStoreEntity record, DateTime dateTime)
		{
			if (!record.IsPending && (record.StartDateTimeUtc < dateTime) && (record.EndDateTimeUtc > dateTime))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Sets defaults for the record (for new records)
		/// </summary>
		public static void SetDefaults(this AuditFieldsUserProfileOptional record, UserProfile userProfile)
		{
			record.UpdateAuditFields(userProfile);
		}

		/// <summary>
		/// Updated UpdateDateTimeUtc and updateuserid to current
		/// </summary>
		public static void UpdateAuditFields(this AuditFieldsUserProfileOptional record, UserProfile userProfile)
		{
			int? userProfileId = null;
			if (userProfile != null)
			{
				userProfileId = userProfile.UserProfileId;
			}
			record.UpdateAuditFields(userProfileId);
		}

		public static void UpdateAuditFields(this AuditFieldsUserProfileOptional record, int? userProfileId)
		{
			//new record fill create info
			if (record.CreateDateTimeUtc == null || record.CreateDateTimeUtc == DateTime.MinValue)
			{
				record.CreateDateTimeUtc = DateTime.UtcNow;
				record.CreatedBy_UserProfileId = userProfileId;
			}
			record.UpdateDateTimeUtc = DateTime.UtcNow;
			record.UpdatedBy_UserProfileId = userProfileId;
		}

		public static TEntity CopyValuesToEntity<TEntity>(this TEntity copyFrom, TEntity copyTo) where TEntity : GStoreEntity, new()
		{
			if (copyTo == null)
			{
				throw new ArgumentNullException("copyTo");
			}
			if (copyFrom == null)
			{
				throw new ArgumentNullException("copyFrom");
			}
			return CopyEntityValues(copyFrom, copyTo);
		}

		public static TEntity UpdateValuesFromEntity<TEntity>(this TEntity copyTo, TEntity copyFrom) where TEntity : GStoreEntity, new()
		{
			if (copyTo == null)
			{
				throw new ArgumentNullException("copyTo");
			}
			if (copyFrom == null)
			{
				throw new ArgumentNullException("copyFrom");
			}

			return CopyEntityValues(copyFrom, copyTo);
		}

		public static TEntity CopyEntityValues<TEntity>(TEntity copyFrom, TEntity copyTo) where TEntity : GStoreEntity, new()
		{
			if (copyTo == null)
			{
				throw new ArgumentNullException("copyTo");
			}
			if (copyFrom == null)
			{
				throw new ArgumentNullException("copyFrom");
			}
			try
			{
				System.Reflection.PropertyInfo[] properties = typeof(TEntity).GetProperties();
				foreach (System.Reflection.PropertyInfo prop in properties)
				{
					if (prop.PropertyType.IsValueType)
					{
						//copy value types
						prop.SetValue(copyTo, prop.GetValue(copyFrom));
					}
					else if (prop.PropertyType.IsClass && prop.PropertyType.UnderlyingSystemType.Equals(typeof(System.String)))
					{
						//copy string values
						prop.SetValue(copyTo, prop.GetValue(copyFrom));
					}
					else
					{
						//does not map complex types or classes or child nav props
					}
				}

				return copyTo;
			}
			catch (Exception ex)
			{
				string error = "Error copying entity values from " + copyFrom.GetType().FullName + " to " + copyTo.GetType().FullName + " Exception: " + ex.Message;
				throw new ApplicationException(error, ex);
			}
		}

	}
}