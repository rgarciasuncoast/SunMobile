using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile
{
	[DataContract]
	public class RegistrationResponse
	{
		[DataMember(Name = "deviceResponse")]
		public string DeviceResponse { get; set; }

		[DataMember(Name = "newRegistration")]
		public bool NewRegistration { get; set; }

		[DataMember(Name = "registrationRequest")]
		public RegistrationInfo RegistrationInfo { get; set; }

		[DataMember(Name = "deviceInfo")]
		public Deviceinfo DeviceInfo { get; set; }

		[DataMember(Name = "metadata")]
		public Metadata MetaData { get; set; }
	}

	[DataContract]
	public class RegistrationInfo
	{
		[DataMember(Name = "type")]
		public string RegistrationInfoType { get; set; }

		[DataMember(Name = "data")]
		public RegistrationInfoData RegistrationInfoData { get; set; }
	}

	[DataContract]
	public class Pid
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "value")]
		public string Value { get; set; }
	}

	[DataContract]
	public class Deviceinfo
	{
		[DataMember(Name = "permanentId")]
		public string PermanentId { get; set; }

		[DataMember(Name = "publicSigningKey")]
		public string PublicSigningKey { get; set; }
	}

	[DataContract]
	public class Metadata
	{
		[DataMember(Name = "build_protection")]
		public string BuildProtection { get; set; }
	}

	[DataContract]
	public class RegistrationInfoData
	{
		[DataMember(Name = "platform")]
		public string Platform { get; set; }

		[DataMember(Name = "publicSigningKey")]
		public string PublicSigningKey { get; set; }

		[DataMember(Name = "publicEncryptionKey")]
		public string PublicEncryptionKey { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

		[DataMember(Name = "accountGUID")]
		public string AccountGuid { get; set; }

		[DataMember(Name = "sdkVersion")]
		public string SdkVersion { get; set; }

		[DataMember(Name = "broadcastSupported")]
		public bool BroadcastSupported { get; set; }

		[DataMember(Name = "clientShortVersion")]
		public string ClientShortVersion { get; set; }

		[DataMember(Name = "clientBundleVersion")]
		public string ClientBundleVersion { get; set; }

		[DataMember(Name = "clientAppName")]
		public string ClientAppName { get; set; }

		[DataMember(Name = "pid")]
		public Pid[] Pid { get; set; }
	}
}