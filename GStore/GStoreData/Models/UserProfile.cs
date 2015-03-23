using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	/// <summary>
	/// User Profile information and settings
	/// </summary>
	[Table("UserProfile")]
	public class UserProfile : BaseClasses.AuditFieldsUserProfileOptional
	{
		/// <summary>
		/// PK (counter) UserProfile.UserProfileId
		/// </summary>
		[Editable(false)]
		[Key]
		[Required]
		[Display(Name = "User Profile Id")]
		public int UserProfileId { get; set; }

		/// <summary>
		/// UserId (string) from Identity
		/// </summary>
		[Editable(false)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(255)]
		[Required]
		[Display(Name = "Identity User Id (email)")]
		[DataType(DataType.EmailAddress)]
		public string UserId { get; set; }

		/// <summary>
		/// User name from identity (same as email)
		/// </summary>
		[Display(Name = "User Name (email)")]
		[Editable(false)]
		[Required]
		[Index(IsUnique=true)]
		[MaxLength(255)]
		[DataType(DataType.EmailAddress)]
		public string UserName { get; set; }

		/// <summary>
		/// Email address from identity
		/// </summary>
		[Editable(false)]
		[Required]
		[Index(IsUnique = true)]
		[MaxLength(255)]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[ForeignKey("StoreFrontId")]
		[Display(Name = "Store Front")]
		public virtual StoreFront StoreFront { get; set; }

		[Display(Name = "Store Front Id")]
		public int? StoreFrontId { get; set; }

		[Display(Name = "Client Id")]
		public int? ClientId { get; set; }

		[ForeignKey("ClientId")]
		public virtual Client Client { get; set; }

		public int Order { get; set; }

		/// <summary>
		/// Name as displayed in messages and site logon/logoff
		/// </summary>
		[Display(Name = "Full Name")]
		[Required]
		public string FullName { get; set; }

		/// <summary>
		/// Notes entered at signup
		/// </summary>
		[Display(Name = "Signup Notes")]
		[DataType(DataType.MultilineText)]
		public string SignupNotes { get; set; }

		[Display(Name = "Session Entry Url")]
		public string EntryUrl { get; set; }

		[Display(Name = "Session Entry Raw Url")]
		public string EntryRawUrl { get; set; }

		[Display(Name = "Session Entry Referrer")]
		public string EntryReferrer { get; set; }

		[Display(Name = "Session Entry Date and Time")]
		public DateTime EntryDateTime { get; set; }

		[Display(Name = "Time Zone")]
		public string TimeZoneId { get; set; }

		[Display(Name = "Register Web Form Response Id")]
		public int? RegisterWebFormResponseId { get; set; }

		[ForeignKey("RegisterWebFormResponseId")]
		[Display(Name = "Register Web Form Response")]
		public virtual WebFormResponse RegisterWebFormResponse { get; set; }

		/// <summary>
		/// User has asked for more info at signup
		/// </summary>
		[Display(Name = "Send Me More Info by Email")]
		public bool SendMoreInfoToEmail { get; set; }

		[MaxLength(100)]
		[Display(Name = "Address Line 1")]
		public string AddressLine1 { get; set; }

		[MaxLength(100)]
		[Display(Name = "Address Line 2")]
		public string AddressLine2 { get; set; }

		[MaxLength(50)]
		[Display(Name = "City")]
		public string City { get; set; }
	
		[MaxLength(50)]
		[Display(Name = "State / Province")]
		public string State { get; set; }

		[MaxLength(12)]
		[Display(Name="ZIP Code")]
		public string PostalCode { get; set; }

		[Display(Name = "Country")]
		public CountryCodeEnum? CountryCode { get; set; }



		#region Notification, messaging, subscription settings	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-

		/// <summary>
		/// True if user allows other users to send them messages through site
		/// </summary>
		[Display(Name = "Allow users to send me site messages")]
		public bool AllowUsersToSendSiteMessages { get; set; }

		/// <summary>
		/// When true, notifies all users (and anonymous) when user logs  on
		/// </summary>
		[Display(Name = "Display pop-up to all users when loging on/off")]
		public bool NotifyAllWhenLoggedOn { get; set; }

		/// <summary>
		/// When true, notifies user by email for site updates
		/// </summary>
		[Display(Name = "Email me when this site is updated")]
		public bool NotifyOfSiteUpdatesToEmail { get; set; }

		/// <summary>
		/// True if user is subscribed to the newsletter
		/// </summary>
		[Display(Name = "Subscribe to Newsletter Email")]
		public bool SubscribeToNewsletterEmail { get; set; }

		/// <summary>
		/// When true, notifies user by email for all site messages
		/// </summary>
		[Display(Name = "Email me when there is a site message for me")]
		public bool SendSiteMessagesToEmail { get; set; }

		/// <summary>
		/// When true, notifies user by SMS text message for all site messages
		/// </summary>
		[Display(Name = "Send a Text Message to my phone when there is a site message for me")]
		public bool SendSiteMessagesToSms { get; set; }

		#endregion

		#region Historical dates for auditing and notifications	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-

		/// <summary>
		/// Date and time in UTC of last successful logon
		/// </summary>
		[Editable(false)]
		[Display(Name = "Last Login")]
		public DateTime? LastLogonDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of more info sent to user
		/// </summary>
		[Editable(false)]
		[Display(Name = "Last Sent More Info")]
		public DateTime? SentMoreInfoToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last site update notification by email
		/// </summary>
		[Editable(false)]
		[Display(Name = "Last Sent Site Update")]
		public DateTime? LastSiteUpdateSentToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last newsletter sent by email
		/// </summary>
		[Editable(false)]
		[Display(Name = "Last Sent Newsletter to Email")]
		public DateTime? LastNewsletterSentToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last site message notification by email
		/// </summary>
		[Editable(false)]
		[Display(Name = "Last Sent Notification to Email")]
		public DateTime? LastSiteMessageSentToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last logon failure notification by email
		/// </summary>
		[Editable(false)]
		[Display(Name = "Last Lockout Failure Notice")]
		public DateTime? LastLockoutFailureNoticeDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Last Order Admin Visit")]
		public DateTime? LastOrderAdminVisitDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Last Catalog Admin Visit")]
		public DateTime? LastCatalogAdminVisitDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Last Store Admin Visit")]
		public DateTime? LastStoreAdminVisitDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Last System Admin Visit")]
		public DateTime? LastSystemAdminVisitDateTimeUtc { get; set; }

		#endregion


		/// <summary>
		/// Notifications table data for user
		/// </summary>
		public virtual ICollection<Notification> Notifications { get; set; }

		/// <summary>
		/// Notifications sent from the user user
		/// </summary>
		[Display(Name = "Sent Notifications")]
		public virtual ICollection<Notification> NotificationsSent { get; set; }

		[Display(Name = "Client User Roles")]
		public virtual ICollection<ClientUserRole> ClientUserRoles { get; set; }

		[Display(Name = "Welcome Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> WelcomeStoreFrontConfigurations { get; set; }

		[Display(Name = "Account Admin Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> AccountAdminStoreFrontConfigurations { get; set; }

		[Display(Name = "Order Admin Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> OrderAdminStoreFrontConfigurations { get; set; }

		[Display(Name = "Registered Notify Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> RegisteredNotifyStoreFrontConfigurations { get; set; }

		public virtual ICollection<Order> Orders { get; set; }

	}

	public enum CountryCodeEnum : int
	{
		[Display(Name = "United States", Order=-1)]
		US = 1,

		[Display(Name = "ALBANIA")]
		AL,

		[Display(Name = "ALGERIA")]
		DZ,
		
		[Display(Name = "ANDORRA")]
		AD,
		
		[Display(Name = "ANGOLA")]
		AO,
		
		[Display(Name = "ANGUILLA")]
		AI,
		
		[Display(Name = "ANTIGUA AND BARBUDA")]
		AG,
		
		[Display(Name = "ARGENTINA")]
		AR,
		
		[Display(Name = "ARMENIA")]
		AM,
		
		[Display(Name = "ARUBA")]
		AW,
		
		[Display(Name = "AUSTRALIA")]
		AU,
		
		[Display(Name = "AUSTRIA")]
		AT,
		
		[Display(Name = "AZERBAIJAN")]
		AZ,
		
		[Display(Name = "BAHAMAS")]
		BS,
		
		[Display(Name = "BAHRAIN")]
		BH,
		
		[Display(Name = "BARBADOS")]
		BB,
		
		[Display(Name = "BELGIUM")]
		BE,
		
		[Display(Name = "BELIZE")]
		BZ,
		
		[Display(Name = "BENIN")]
		BJ,
		
		[Display(Name = "BERMUDA")]
		BM,
		
		[Display(Name = "BHUTAN")]
		BT,
		
		[Display(Name = "BOLIVIA")]
		BO,
		
		[Display(Name = "BOSNIA-HERZEGOVINA")]
		BA,
		
		[Display(Name = "BOTSWANA")]
		BW,
		
		[Display(Name = "BRAZIL")]
		BR,
		
		[Display(Name = "BRUNEI DARUSSALAM")]
		BN,
		
		[Display(Name = "BULGARIA")]
		BG,
		
		[Display(Name = "BURKINA FASO")]
		BF,
		
		[Display(Name = "BURUNDI")]
		BI,
		
		[Display(Name = "CAMBODIA")]
		KH,
		
		[Display(Name = "CANADA")]
		CA,
		
		[Display(Name = "CAPE VERDE")]
		CV,
		
		[Display(Name = "CAYMAN ISLANDS")]
		KY,
		
		[Display(Name = "CHAD")]
		TD,
		
		[Display(Name = "CHILE")]
		CL,
		
		[Display(Name = "CHINA")]
		CN,
		
		[Display(Name = "COLOMBIA")]
		CO,
		
		[Display(Name = "COMOROS")]
		KM,
		
		[Display(Name = "DEMOCRATIC REPUBLIC OF CONGO")]
		CD,
		
		[Display(Name = "CONGO")]
		CG,
		
		[Display(Name = "COOK ISLANDS")]
		CK,
		
		[Display(Name = "COSTA RICA")]
		CR,
		
		[Display(Name = "CROATIA")]
		HR,
		
		[Display(Name = "CYPRUS")]
		CY,
		
		[Display(Name = "CZECH REPUBLIC")]
		CZ,
		
		[Display(Name = "DENMARK")]
		DK,
		
		[Display(Name = "DJIBOUTI")]
		DJ,
		
		[Display(Name = "DOMINICA")]
		DM,
		
		[Display(Name = "DOMINICAN REPUBLIC")]
		DO,
		
		[Display(Name = "ECUADOR")]
		EC,
		
		[Display(Name = "EGYPT")]
		EG,
		
		[Display(Name = "EL SALVADOR")]
		SV,
		
		[Display(Name = "ERITERIA")]
		ER,
		
		[Display(Name = "ESTONIA")]
		EE,
		
		[Display(Name = "ETHIOPIA")]
		ET,
		
		[Display(Name = "FALKLAND ISLANDS (MALVINAS)")]
		FK,
		
		[Display(Name = "FIJI")]
		FJ,
		
		[Display(Name = "FINLAND")]
		FI,
		
		[Display(Name = "FRANCE")]
		FR,
		
		[Display(Name = "FRENCH GUIANA")]
		GF,
		
		[Display(Name = "FRENCH POLYNESIA")]
		PF,
		
		[Display(Name = "GABON")]
		GA,
		
		[Display(Name = "GAMBIA")]
		GM,
		
		[Display(Name = "GEORGIA")]
		GE,
		
		[Display(Name = "GERMANY")]
		DE,
		
		[Display(Name = "GIBRALTAR")]
		GI,
		
		[Display(Name = "GREECE")]
		GR,
		
		[Display(Name = "GREENLAND")]
		GL,
		
		[Display(Name = "GRENADA")]
		GD,
		
		[Display(Name = "GUADELOUPE")]
		GP,
		
		[Display(Name = "GUAM")]
		GU,
		
		[Display(Name = "GUATEMALA")]
		GT,
		
		[Display(Name = "GUINEA")]
		GN,
		
		[Display(Name = "GUINEA BISSAU")]
		GW,
		
		[Display(Name = "GUYANA")]
		GY,
		
		[Display(Name = "HOLY SEE (VATICAN CITY STATE)")]
		VA,
		
		[Display(Name = "HONDURAS")]
		HN,
		
		[Display(Name = "HONG KONG")]
		HK,
		
		[Display(Name = "HUNGARY")]
		HU,
		
		[Display(Name = "ICELAND")]
		IS,
		
		[Display(Name = "INDIA")]
		IN,
		
		[Display(Name = "INDONESIA")]
		ID,
		
		[Display(Name = "IRELAND")]
		IE,
		
		[Display(Name = "ISRAEL")]
		IL,
		
		[Display(Name = "ITALY")]
		IT,
		
		[Display(Name = "JAMAICA")]
		JM,
		
		[Display(Name = "JAPAN")]
		JP,
		
		[Display(Name = "JORDAN")]
		JO,
		
		[Display(Name = "KAZAKHSTAN")]
		KZ,
		
		[Display(Name = "KENYA")]
		KE,
		
		[Display(Name = "KIRIBATI")]
		KI,
		
		[Display(Name = "KOREA")]
		KR,
		
		[Display(Name = "KUWAIT")]
		KW,
		
		[Display(Name = "KYRGYZSTAN")]
		KG,
		
		[Display(Name = "LAOS")]
		LA,
		
		[Display(Name = "LATVIA")]
		LV,
		
		[Display(Name = "LESOTHO")]
		LS,
		
		[Display(Name = "LIECHTENSTEIN")]
		LI,
		
		[Display(Name = "LITHUANIA")]
		LT,
		
		[Display(Name = "LUXEMBOURG")]
		LU,
		
		[Display(Name = "MADAGASCAR")]
		MG,
		
		[Display(Name = "MALAWI")]
		MW,
		
		[Display(Name = "MALAYSIA")]
		MY,
		
		[Display(Name = "MALDIVES")]
		MV,
		
		[Display(Name = "MALI")]
		ML,
		
		[Display(Name = "MALTA")]
		MT,
		
		[Display(Name = "MARSHALL ISLANDS")]
		MH,
		
		[Display(Name = "MARTINIQUE")]
		MQ,
		
		[Display(Name = "MAURITANIA")]
		MR,
		
		[Display(Name = "MAURITIUS")]
		MU,
		
		[Display(Name = "MAYOTTE")]
		YT,
		
		[Display(Name = "MEXICO")]
		MX,
		
		[Display(Name = "MICRONESIA, FEDERATED STATES OF")]
		FM,
		
		[Display(Name = "MONGOLIA")]
		MN,
		
		[Display(Name = "MONTSERRAT")]
		MS,
		
		[Display(Name = "MOROCCO")]
		MA,
		
		[Display(Name = "MOZAMBIQUE")]
		MZ,
		
		[Display(Name = "NAMIBIA")]
		NA,
		
		[Display(Name = "NAURU")]
		NR,
		
		[Display(Name = "NEPAL")]
		NP,
		
		[Display(Name = "NETHERLANDS")]
		NL,
		
		[Display(Name = "NETHERLANDS ANTILLES")]
		AN,
		
		[Display(Name = "NEW CALEDONIA")]
		NC,
		
		[Display(Name = "NEW ZEALAND")]
		NZ,
		
		[Display(Name = "NICARAGUA")]
		NI,
		
		[Display(Name = "NIGER")]
		NE,
		
		[Display(Name = "NIUE")]
		NU,
		
		[Display(Name = "NORFOLK ISLAND")]
		NF,
		
		[Display(Name = "NORWAY")]
		NO,
		
		[Display(Name = "OMAN")]
		OM,
		
		[Display(Name = "PALAU")]
		PW,
		
		[Display(Name = "PANAMA")]
		PA,
		
		[Display(Name = "PAPUA NEW GUINEA")]
		PG,
		
		[Display(Name = "PERU")]
		PE,
		
		[Display(Name = "PHILIPPINES")]
		PH,
		
		[Display(Name = "PITCAIRN")]
		PN,
		
		[Display(Name = "POLAND")]
		PL,
		
		[Display(Name = "PORTUGAL")]
		PT,
		
		[Display(Name = "QATAR")]
		QA,
		
		[Display(Name = "REUNION")]
		RE,
		
		[Display(Name = "ROMANIA")]
		RO,
		
		[Display(Name = "RUSSIAN FEDERATION")]
		RU,
		
		[Display(Name = "RWANDA")]
		RW,
		
		[Display(Name = "SAINT HELENA")]
		SH,
		
		[Display(Name = "SAINT KITTS AND NEVIS")]
		KN,
		
		[Display(Name = "SAINT LUCIA")]
		LC,
		
		[Display(Name = "SAINT PIERRE AND MIQUELON")]
		PM,
		
		[Display(Name = "SAINT VINCENT AND THE GRENADINES")]
		VC,
		
		[Display(Name = "SAMOA")]
		WS,
		
		[Display(Name = "SAN MARINO")]
		SM,
		
		[Display(Name = "SAO TOME AND PRINCIPE")]
		ST,
		
		[Display(Name = "SAUDI ARABIA")]
		SA,
		
		[Display(Name = "SENEGAL")]
		SN,
		
		[Display(Name = "SERBIA")]
		RS,
		
		[Display(Name = "SEYCHELLES")]
		SC,
		
		[Display(Name = "SIERRA LEONE")]
		SL,
		
		[Display(Name = "SINGAPORE")]
		SG,
		
		[Display(Name = "SLOVAKIA")]
		SK,
		
		[Display(Name = "SLOVENIA")]
		SI,
		
		[Display(Name = "SOLOMON ISLANDS")]
		SB,
		
		[Display(Name = "SOMALIA")]
		SO,
		
		[Display(Name = "SOUTH AFRICA")]
		ZA,
		
		[Display(Name = "SPAIN")]
		ES,
		
		[Display(Name = "SRI LANKA")]
		LK,
		
		[Display(Name = "SURINAME")]
		SR,
		
		[Display(Name = "SVALBARD AND JAN MAYEN")]
		SJ,
		
		[Display(Name = "SWAZILAND")]
		SZ,
		
		[Display(Name = "SWEDEN")]
		SE,
		
		[Display(Name = "SWITZERLAND")]
		CH,
		
		[Display(Name = "TAIWAN, PROVINCE OF CHINA")]
		TW,
		
		[Display(Name = "TAJIKISTAN")]
		TJ,
		
		[Display(Name = "TANZANIA, UNITED REPUBLIC OF")]
		TZ,
		
		[Display(Name = "THAILAND")]
		TH,
		
		[Display(Name = "TOGO")]
		TG,
		
		[Display(Name = "TONGA")]
		TO,
		
		[Display(Name = "TRINIDAD AND TOBAGO")]
		TT,
		
		[Display(Name = "TUNISIA")]
		TN,
		
		[Display(Name = "TURKEY")]
		TR,
		
		[Display(Name = "TURKMENISTAN")]
		TM,
		
		[Display(Name = "TURKS AND CAICOS ISLANDS")]
		TC,
		
		[Display(Name = "TUVALU")]
		TV,
		
		[Display(Name = "UGANDA")]
		UG,
		
		[Display(Name = "UKRAINE")]
		UA,
		
		[Display(Name = "UNITED ARAB EMIRATES")]
		AE,
		
		[Display(Name = "UNITED KINGDOM")]
		GB,
		
		[Display(Name = "URUGUAY")]
		UY,
		
		[Display(Name = "VANUATU")]
		VU,
		
		[Display(Name = "VENEZUELA")]
		VE,
		
		[Display(Name = "VIETNAM")]
		VN,
		
		[Display(Name = "VIRGIN ISLANDS, BRITISH")]
		VG,
		
		[Display(Name = "WALLIS AND FUTUNA")]
		WF,
		
		[Display(Name = "YEMEN")]
		YE,
		
		[Display(Name = "ZAMBIA")]
		ZM 

	}
}
