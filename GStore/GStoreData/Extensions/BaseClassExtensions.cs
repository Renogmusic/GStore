using System;
using System.Web.Mvc;
using System.Web.Routing;
using GStoreData.ControllerBase;
using GStoreData.Models;
using GStoreData.Models.BaseClasses;

namespace GStoreData
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

		public static bool IsActiveDirect(this GStoreEntity record)
		{
			return record.IsActiveDirect(DateTime.UtcNow);
		}
		public static bool IsActiveDirect(this GStoreEntity record, DateTime dateTime)
		{
			if (record != null && !record.IsPending && (record.StartDateTimeUtc < dateTime) && (record.EndDateTimeUtc > dateTime))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Sets defaults for the record (for new records)
		/// </summary>
		public static void SetDefaults(this AuditFieldsUserProfileOptional record, UserProfile userProfileOrNull)
		{
			record.UpdateAuditFields(userProfileOrNull);
		}

		/// <summary>
		/// Updated UpdateDateTimeUtc and updateuserid to current
		/// </summary>
		public static void UpdateAuditFields(this AuditFieldsUserProfileOptional record, UserProfile userProfileOrNull)
		{
			int? userProfileIdOrNull = null;
			if (userProfileOrNull != null)
			{
				userProfileIdOrNull = userProfileOrNull.UserProfileId;
			}
			record.UpdateAuditFields(userProfileIdOrNull);
		}

		public static void UpdateAuditFields(this AuditFieldsUserProfileOptional record, int? userProfileIdOrNull)
		{
			//new record fill create info
			if (record.CreateDateTimeUtc == null || record.CreateDateTimeUtc == DateTime.MinValue)
			{
				record.CreateDateTimeUtc = DateTime.UtcNow;
				record.CreatedBy_UserProfileId = userProfileIdOrNull;
			}
			record.UpdateDateTimeUtc = DateTime.UtcNow;
			record.UpdatedBy_UserProfileId = userProfileIdOrNull;
		}

		/// <summary>
		/// Copies values from current entity to another entity. Performs a shallow copy
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="copyTo"></param>
		/// <param name="copyFrom"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Copies values from another entity to current entity. Performs a shallow copy
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="copyTo"></param>
		/// <param name="copyFrom"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Copies values from one entity to another. Performs a shallow copy
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="copyFrom"></param>
		/// <param name="copyTo"></param>
		/// <returns></returns>
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

		public static ActionResult RedirectToActionResult(this BaseController controller, string action, string controllerName)
		{
			var routeValues = new RouteValueDictionary();
			routeValues["controller"] = controllerName;
			routeValues["action"] = action;
			return new RedirectToRouteResult(routeValues);
		}

	}
}