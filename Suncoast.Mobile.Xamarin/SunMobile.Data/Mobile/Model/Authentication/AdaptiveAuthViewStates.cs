namespace SunBlock.DataTransferObjects.Mobile.Model.Authentication
{
	public enum AdaptiveAuthViewStates
	{
		Unauthorized,
		UpdateImagePhrase,
		UpdateChallengeQuestions,
		Authenticated,
		AuthenticateHost,
		NotEnrolled,
		Challenge,
		Unlock,
		Analyze,
		AuthenticateHostUnlock,
		EndProcessWithError,
		HostLockOut,
		ContinueProcessWithError, //TODO: this is generic and can be broken out for all processes
		AccountVerificationRequired
	}

	public class AdaptiveAuthViewStatesHelper
	{
	}
}