namespace GStore.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Settings {
        
        public Settings() {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //

        }

		public static CurrentSettings Current
		{
			get
			{
				return CurrentSettings.Create();
			}
		}

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
    }

	public partial class CurrentSettings
	{
		private static Settings _projectSettings = Properties.Settings.Default;

		public CurrentSettings()
		{
		}

		public static CurrentSettings Create()
		{
			return new CurrentSettings();
		}

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

		public string GStoreDB
		{
            get
			{
				return EnvironmentOrAppSettingString("GStoreDB") ?? _projectSettings.GStoreDB;
            }
        }
        
        public string AspNetIdentityNameOrConnectionString
		{
            get
			{
				return EnvironmentOrAppSettingString("AspNetIdentityNameOrConnectionString") ?? _projectSettings.AspNetIdentityNameOrConnectionString;
            }
        }
        
        public bool AppEnableEmail
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppEnableEmail") ?? _projectSettings.AppEnableEmail;
            }
        }
        
        public bool AppEnableSMS
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppEnableSMS") ?? _projectSettings.AppEnableSMS;
            }
        }
        
        public bool AppEnablePageViewLog
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppEnablePageViewLog") ?? _projectSettings.AppEnablePageViewLog;
            }
        }
        
        public bool AppUseFriendlyErrorPages
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppUseFriendlyErrorPages") ?? _projectSettings.AppUseFriendlyErrorPages;
            }
        }
        
        public bool AppUseFileNotFoundImage
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppUseFileNotFoundImage") ?? _projectSettings.AppUseFileNotFoundImage;
            }
        }

		public string SystemAdminThemeFolderName
		{
			get
			{
				return EnvironmentOrAppSettingString("SystemAdminThemeFolderName") ?? _projectSettings.SystemAdminThemeFolderName;
			}
		}

		public bool AppEnableBindingAutoMapToFirstStoreFront
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableBindingAutoMapToFirstStoreFront") ?? _projectSettings.AppEnableBindingAutoMapToFirstStoreFront;
			}
		}

		public string AppDefaultLayoutName
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultLayoutName") ?? _projectSettings.AppDefaultLayoutName;
			}
		}

		public string AppDefaultThemeFolderName
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultThemeFolderName") ?? _projectSettings.AppDefaultThemeFolderName;
			}
		}

		public bool AppEnableBindingAutoMapCatchAll
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableBindingAutoMapCatchAll") ?? _projectSettings.AppEnableBindingAutoMapCatchAll;
			}
		}

		public bool AppEnableAutomaticHomePageCreation
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableAutomaticHomePageCreation") ?? _projectSettings.AppEnableAutomaticHomePageCreation;
			}
		}

		public string AppDefaultPageTemplateViewName
		{
			get
			{
				return EnvironmentOrAppSettingString("AppDefaultPageTemplateViewName") ?? _projectSettings.AppDefaultPageTemplateViewName;
			}
		}

		public bool AppEnableStoresVirtualFolders
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppEnableStoresVirtualFolders") ?? _projectSettings.AppEnableStoresVirtualFolders;
			}
		}

		public bool AppSeedSampleProducts
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppSeedSampleProducts") ?? _projectSettings.AppSeedSampleProducts;
			}
		}

		public bool AppLogSystemEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogSystemEventsToDb") ?? _projectSettings.AppLogSystemEventsToDb;
            }
        }
        
        public bool AppLogSecurityEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogSecurityEventsToDb") ?? _projectSettings.AppLogSecurityEventsToDb;
            }
        }

        public bool AppLogBadRequestEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogBadRequestEventsToDb") ?? _projectSettings.AppLogBadRequestEventsToDb;
            }
        }

        public bool AppLogFileNotFoundEventsToDb
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogFileNotFoundEventsToDb") ?? _projectSettings.AppLogFileNotFoundEventsToDb;
            }
        }

		public bool AppLogPageViewEventsToDb
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogPageViewEventsToDb") ?? _projectSettings.AppLogPageViewEventsToDb;
			}
		}

		public bool AppLogUserActionEventsToDb
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogUserActionEventsToDb") ?? _projectSettings.AppLogUserActionEventsToDb;
			}
		}

		public bool AppLogLogExceptionsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogLogExceptionsToFile") ?? _projectSettings.AppLogLogExceptionsToFile;
			}
		}

		public bool AppLogSystemEventsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogSystemEventsToFile") ?? _projectSettings.AppLogSystemEventsToFile;
			}
		}

		public bool AppLogSecurityEventsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogSecurityEventsToFile") ?? _projectSettings.AppLogSecurityEventsToFile;
			}
		}

		public bool AppLogBadRequestEventsToFile
		{
			get
			{
				return EnvironmentOrAppSettingBool("AppLogBadRequestEventsToFile") ?? _projectSettings.AppLogBadRequestEventsToFile;
			}
		}

		public bool AppLogFileNotFoundEventsToFile
		{
            get
			{
				return EnvironmentOrAppSettingBool("AppLogFileNotFoundEventsToFile") ?? _projectSettings.AppLogFileNotFoundEventsToFile;
            }
        }
        
        public bool AppLogPageViewEventsToFile
		{
            get
			{
				return EnvironmentOrAppSettingBool("AppLogPageViewEventsToFile") ?? _projectSettings.AppLogPageViewEventsToFile;
            }
        }
        
        public bool AppLogUserActionEventsToFile
		{
            get
			{
                return EnvironmentOrAppSettingBool("AppLogUserActionEventsToFile") ?? _projectSettings.AppLogUserActionEventsToFile;
            }
        }

		public string RepositoryProvider
		{
            get
			{
                return EnvironmentOrAppSettingString("RepositoryProvider") ?? _projectSettings.RepositoryProvider;
            }
        }
        
        public bool InitializeEFCodeFirstMigrateLatest
		{
            get
			{
                return EnvironmentOrAppSettingBool("InitializeEFCodeFirstMigrateLatest") ?? _projectSettings.InitializeEFCodeFirstMigrateLatest;
            }
        }
        
        public bool InitializeEFCodeFirstDropCreate
		{
            get
			{
                return EnvironmentOrAppSettingBool("InitializeEFCodeFirstDropCreate") ?? _projectSettings.InitializeEFCodeFirstDropCreate;
            }
        }
        
        public bool IdentityEnableNewUserRegisteredBroadcast
		{
            get
			{
				return EnvironmentOrAppSettingBool("IdentityEnableNewUserRegisteredBroadcast") ?? _projectSettings.IdentityEnableNewUserRegisteredBroadcast;
            }
        }
        
        public string IdentitySendGridMailFromEmail
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailFromEmail") ?? _projectSettings.IdentitySendGridMailFromEmail;
            }
        }
        
        public string IdentitySendGridMailFromName
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailFromName") ?? _projectSettings.IdentitySendGridMailFromName;
            }
        }
        
        public string IdentitySendGridMailAccount
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailAccount") ?? _projectSettings.IdentitySendGridMailAccount;
            }
        }
        
        public string IdentitySendGridMailPassword
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentitySendGridMailPassword") ?? _projectSettings.IdentitySendGridMailPassword;
            }
        }
        
        public string IdentityTwilioSid
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwilioSid") ?? _projectSettings.IdentityTwilioSid;
            }
        }
        
        public string IdentityTwilioToken
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwilioToken") ?? _projectSettings.IdentityTwilioToken;
            }
        }
        
        public string IdentityTwilioFromPhone
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwilioFromPhone") ?? _projectSettings.IdentityTwilioFromPhone;
            }
        }
        
        public string IdentityTwoFactorSignature
		{
            get
			{
                return EnvironmentOrAppSettingString("IdentityTwoFactorSignature") ?? _projectSettings.IdentityTwoFactorSignature;
            }
        }
        
        public bool IdentityEnableTwoFactorAuth
		{
            get
			{
                return EnvironmentOrAppSettingBool("IdentityEnableTwoFactorAuth") ?? _projectSettings.IdentityEnableTwoFactorAuth;
            }
        }


	}
}
