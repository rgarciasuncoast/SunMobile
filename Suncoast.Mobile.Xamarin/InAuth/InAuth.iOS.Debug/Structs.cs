using System.Runtime.InteropServices;
using Foundation;

namespace InAuth.iOS.Debug
{
	public enum MMELogOptions : uint
	{
		EmptyLogSet = 0,
		Accelerometer = (1 << 0),
		Battery = (1 << 1),
		Contact = (1 << 2),
		DataUsage = (1 << 3),
		Device = (1 << 4),
		Gps = (1 << 5),
		Hardware = (1 << 6),
		Malware = (1 << 7),
		Media = (1 << 8),
		Processes = (1 << 9),
		Root = (1 << 10),
		Telephone = (1 << 11),
		Wifi = (1 << 12),
		Whitebox = (1 << 23),
		//IpaDigest = (1 << 31),
		TotalLogSet = 4095 | Telephone | Wifi | Whitebox
	}

	public enum GreyListOptions : uint
	{
		Accessibility = (1 << 14),
		AugmentedContact = (1 << 15),
		AugmentedDevice = (1 << 16),
		AugmentedGps = (1 << 17),
		Calendar = (1 << 18),
		Pedometer = (1 << 19),
		Photos = (1 << 20),
		Twitter = (1 << 21),
		TotalGreylistSet = Accessibility | AugmentedContact | AugmentedDevice | AugmentedGps | Calendar | Pedometer | Photos | Twitter
	}

	public enum MMEListType : ushort
	{
		RootList = (1 << 1),
		MalwareList = (1 << 2),
		LogConfig = (1 << 3),
		TotalListSet = RootList | MalwareList | LogConfig
	}

	public enum StorageTypes : byte
	{
		Undefined = 0,
		Sse,
		Hse
	}

	public enum MMEWhiteBoxPolicyOptions : ushort
	{
		RootedPolicy = (1 << 0),
		MalwarePolicy = (1 << 1),
		LocationPolicy = (1 << 2),
		WifiPolicy = (1 << 3),
		RootCloakPolicy = (1 << 4)
	}

	/*
	public enum InAuthStates : ushort
	{
		Error = (1 << 1),
		Rooted = (1 << 2),
		NotRooted = ~(Rooted),
		MalwareDetected = (1 << 3),
		NoMalwareDetected = ~(MalwareDetected),
		Compromised = (1 << 4),
		Undefined = (1 << 8)
	}
	*/

	static class CFunctions
	{
		// extern NSString * requestTypeAsString (MMEListType type);
		[DllImport ("__Internal")]
		//[Verify (PlatformInvoke)]
		static extern NSString requestTypeAsString (MMEListType type);

		// extern NSString * stateToNSString (InAuthState state);
		[DllImport ("__Internal")]
		//[Verify (PlatformInvoke)]
		static extern NSString stateToNSString (ushort state);
	}
}
