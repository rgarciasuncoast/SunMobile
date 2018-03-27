using System;
using SunBlock.DataTransferObjects.Messages.SecuredMessaging;

namespace SunMobile.Shared.Data
{
	public class MessageViewModel
	{
		public string Id { get; set; }
		public MessageTypes MessageType { get; set; }
		public string Subject { get; set; }
		public DateTime DateReceived { get; set; }
		public string FriendlyDate { get; set; }
		public string Body { get; set; }
		public string BodyStrippedOfHtml { get; set; }
		public bool IsRead { get; set; }
		public bool IsHtml { get; set; }
		public MessageThread Thread { get; set; }
	}
}