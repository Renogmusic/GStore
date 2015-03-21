namespace GStoreData
{
	public static class Settings
	{
		private static Properties.Settings _projectSettings = Properties.Settings.Default;

		public static string EnvironmentOrAppSettingString(string key)
		{
			string value = System.Environment.GetEnvironmentVariable("APPSETTING_" + key);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}
			return System.Web.Configuration.WebConfigurationManager.AppSettings.Get(key);
		}

		public static bool? EnvironmentOrAppSettingBool(string key)
		{
			string value = System.Environment.GetEnvironmentVariable("APPSETTING_" + key);
			if (string.IsNullOrWhiteSpace(value))
			{
				value = System.Web.Configuration.WebConfigurationManager.AppSettings.Get(key);
			}

			if (string.IsNullOrEmpty(value))
			{
				return null;
			}

			value = value.ToLower();
			if (value == "true" || value == "1" || value == "y" || value == "yes" || value == "t")
			{
				return true;
			}
			return false;
		}

		public static string GStoreDB
		{
            get
			{
				return EnvironmentOrAppSettingString("GStoreDB") ?? _projectSettings.GStoreDB;
            }
        }

		public static string AspNetIdentityNameOrConnectionString
		{
            get
			{
				return EnvironmentOrAppSettingString("AspNetIdentityNameOrConnectionString") ?? _projectSettings.AspNetIdentityNameOrConnectionString;
            }
        }

		public static bool AppEnableEmail
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppEnableEmail") ?? _projectSettings.AppEnableEmail;
            }
        }
        
		public static string AppDefaultTimeZoneId
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultTimeZoneId") ?? _projectSettings.AppDefaultTimeZoneId;
			}
		}

        public static bool AppEnableSMS
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppEnableSMS") ?? _projectSettings.AppEnableSMS;
            }
        }

		public static string AppDefaultLayoutName
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultLayoutName") ?? _projectSettings.AppDefaultLayoutName;
			}
		}

		public static bool AppEnablePageViewLog
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppEnablePageViewLog") ?? _projectSettings.AppEnablePageViewLog;
            }
        }

		public static bool AppEnableUserActionLog
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableUserActionLog") ?? _projectSettings.AppEnableUserActionLog;
			}
		}

		public static bool AppUseFriendlyErrorPages
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppUseFriendlyErrorPages") ?? _projectSettings.AppUseFriendlyErrorPages;
            }
        }

		public static bool AppUseFileNotFoundImage
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppUseFileNotFoundImage") ?? _projectSettings.AppUseFileNotFoundImage;
            }
        }

		public static string SystemAdminThemeFolderName
		{
			get
			{
				return EnvironmentOrAppSettingString("SystemAdminThemeFolderName") ?? _projectSettings.SystemAdminThemeFolderName;
			}
		}

		public static bool AppEnableBindingAutoMapToFirstStoreFront
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableBindingAutoMapToFirstStoreFront") ?? _projectSettings.AppEnableBindingAutoMapToFirstStoreFront;
			}
		}

		public static bool AppEnableBindingAutoMapCatchAll
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableBindingAutoMapCatchAll") ?? _projectSettings.AppEnableBindingAutoMapCatchAll;
			}
		}

		public static bool AppEnableAutomaticHomePageCreation
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableAutomaticHomePageCreation") ?? _projectSettings.AppEnableAutomaticHomePageCreation;
			}
		}

		public static string AppDefaultPageTemplateName
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultPageTemplateName") ?? _projectSettings.AppDefaultPageTemplateName;
			}
		}

		public static string AppDefaultPageTemplateViewName
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultPageTemplateViewName") ?? _projectSettings.AppDefaultPageTemplateViewName;
			}
		}

		public static bool AppEnableStoresVirtualFolders
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableStoresVirtualFolders") ?? _projectSettings.AppEnableStoresVirtualFolders;
			}
		}

		public static bool AppDoNotSeedDatabase
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppDoNotSeedDatabase") ?? _projectSettings.AppDoNotSeedDatabase;
			}
		}

		public static string AppDefaultThemeFolderName
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultThemeFolderName") ?? _projectSettings.AppDefaultThemeFolderName;
			}
		}

		public static bool AppSeedSampleProducts
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppSeedSampleProducts") ?? _projectSettings.AppSeedSampleProducts;
			}
		}

		public static bool AppLogSystemEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogSystemEventsToDb") ?? _projectSettings.AppLogSystemEventsToDb;
            }
        }

		public static bool AppLogSecurityEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogSecurityEventsToDb") ?? _projectSettings.AppLogSecurityEventsToDb;
            }
        }

		public static bool AppLogBadRequestEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogBadRequestEventsToDb") ?? _projectSettings.AppLogBadRequestEventsToDb;
            }
        }

		public static bool AppLogFileNotFoundEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogFileNotFoundEventsToDb") ?? _projectSettings.AppLogFileNotFoundEventsToDb;
            }
        }

		public static bool AppLogPageViewEventsToDb
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogPageViewEventsToDb") ?? _projectSettings.AppLogPageViewEventsToDb;
			}
		}

		public static bool AppLogUserActionEventsToDb
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogUserActionEventsToDb") ?? _projectSettings.AppLogUserActionEventsToDb;
			}
		}

		public static bool AppLogLogExceptionsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogLogExceptionsToFile") ?? _projectSettings.AppLogLogExceptionsToFile;
			}
		}

		public static bool AppLogSystemEventsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogSystemEventsToFile") ?? _projectSettings.AppLogSystemEventsToFile;
			}
		}

		public static bool AppLogSecurityEventsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogSecurityEventsToFile") ?? _projectSettings.AppLogSecurityEventsToFile;
			}
		}

		public static bool AppLogBadRequestEventsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogBadRequestEventsToFile") ?? _projectSettings.AppLogBadRequestEventsToFile;
			}
		}

		public static bool AppLogFileNotFoundEventsToFile
		{
            get
			{
				return EnvironmentOrAppSettingBool("AppLogFileNotFoundEventsToFile") ?? _projectSettings.AppLogFileNotFoundEventsToFile;
            }
        }

		public static bool AppLogPageViewEventsToFile
		{
            get
			{
				return EnvironmentOrAppSettingBool("AppLogPageViewEventsToFile") ?? _projectSettings.AppLogPageViewEventsToFile;
            }
        }

		public static bool AppLogUserActionEventsToFile
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogUserActionEventsToFile") ?? _projectSettings.AppLogUserActionEventsToFile;
            }
        }

		public static bool AppLogEmailSentToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogEmailSentToFile") ?? _projectSettings.AppLogEmailSentToFile;
			}
		}

		public static bool AppLogEmailSentToDb
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogEmailSentToDb") ?? _projectSettings.AppLogEmailSentToDb;
			}
		}

		public static bool AppLogSmsSentToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogSmsSentToFile") ?? _projectSettings.AppLogSmsSentToFile;
			}
		}

		public static bool AppLogSmsSentToDb
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogSmsSentToDb") ?? _projectSettings.AppLogSmsSentToDb;
			}
		}

		public static string RepositoryProvider
		{
            get
			{
                return EnvironmentOrAppSettingString("RepositoryProvider") ?? _projectSettings.RepositoryProvider;
            }
        }

		public static bool InitializeEFCodeFirstMigrateLatest
		{
            get
			{
                return EnvironmentOrAppSettingBool("InitializeEFCodeFirstMigrateLatest") ?? _projectSettings.InitializeEFCodeFirstMigrateLatest;
            }
        }

		public static bool InitializeEFCodeFirstDropCreate
		{
            get
			{
                return EnvironmentOrAppSettingBool("InitializeEFCodeFirstDropCreate") ?? _projectSettings.InitializeEFCodeFirstDropCreate;
            }
        }

		public static bool IdentityEnableNewUserRegisteredBroadcast
		{
            get
			{
				return EnvironmentOrAppSettingBool("IdentityEnableNewUserRegisteredBroadcast") ?? _projectSettings.IdentityEnableNewUserRegisteredBroadcast;
            }
        }

		public static string IdentitySendGridMailFromEmail
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailFromEmail") ?? _projectSettings.IdentitySendGridMailFromEmail;
            }
        }

		public static string IdentitySendGridMailFromName
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailFromName") ?? _projectSettings.IdentitySendGridMailFromName;
            }
        }

		public static string IdentitySendGridMailAccount
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailAccount") ?? _projectSettings.IdentitySendGridMailAccount;
            }
        }

		public static string IdentitySendGridMailPassword
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailPassword") ?? _projectSettings.IdentitySendGridMailPassword;
            }
        }

		public static string IdentityTwilioSid
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwilioSid") ?? _projectSettings.IdentityTwilioSid;
            }
        }

		public static string IdentityTwilioToken
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwilioToken") ?? _projectSettings.IdentityTwilioToken;
            }
        }

		public static string IdentityTwilioFromPhone
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwilioFromPhone") ?? _projectSettings.IdentityTwilioFromPhone;
            }
        }

		public static string IdentityTwoFactorSignature
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwoFactorSignature") ?? _projectSettings.IdentityTwoFactorSignature;
            }
        }

		public static bool IdentityEnableTwoFactorAuth
		{
            get
			{
                return EnvironmentOrAppSettingBool("IdentityEnableTwoFactorAuth") ?? _projectSettings.IdentityEnableTwoFactorAuth;
            }
        }


	}
}
