namespace GStore.Models
{
	public enum SystemEventLevel : int
	{
		Information = 1000,
		Warning = 2000,
		Error = 3000,
		CriticalError = 4000,
		ApplicationException = 9000,
		UnknownException = 9999
	}

	public enum SecurityEventLevel : int
	{
		LoginSuccess = 100,
		LoginFailure = 200,
		LoginNeedsVerification = 220,
		VerificationCodeSent = 250,
		VerificationCodeSuccess = 270,
		VerificationCodeFailedBadCode = 280,
		VerificationCodeFailedLockedOut = 290,
		Lockout = 300,
		NewRegister = 400,
		EmailConfirmed = 500,
		EmailConfirmFailed = 510,
		EmailConfirmFailedUnknownUser = 520,
		PhoneConfirmed = 600,
		ForgotPasswordSuccess = 700,
		ForgotPasswordFailed = 710,
		ForgotPasswordFailedUnknownUser = 710,
		PasswordResetSuccess = 800,
		PasswordResetFailed = 810,
		PasswordResetFailedUnknownUser = 820,
		NotAuthorized = 2400,
		FileNotFound = 2500,
		BadRequest = 2600,
	}

	public enum EventLogType
	{
		System = 1000,
		Security = 2000,
		UserAction = 3000,
		PageView = 4000
	}
}