namespace SunMobile.Shared.Data
{
	public class FileInformation
	{
		public string FileId { get; set; }
		public string FileName { get; set; }
		public string PathAndFileName { get; set; }
		public string Base64String { get; set; }
		public byte[] FileBytes { get; set; }
		public string MimeType { get; set; }
		public string Status { get; set; }
	}
}