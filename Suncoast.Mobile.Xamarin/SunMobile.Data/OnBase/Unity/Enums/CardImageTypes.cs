using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase.Unity.Enums
{
	[DataContract]
	public enum CardImageTypes
	{		
		[EnumMember]
		STANDARD_IMAGE,
		[EnumMember]
		RAYS_DJ_KITTY_IMAGE,
		[EnumMember]
		RAYS_PLAYER_IMAGE
	}
}