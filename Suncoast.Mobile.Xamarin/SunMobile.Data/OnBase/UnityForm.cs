using System.Collections.Generic;
using System.Runtime.Serialization;
using SunBlock.DataTransferObjects.Security;

namespace SunBlock.DataTransferObjects.OnBase
{
	[DataContract]
	public class UnityForm
	{
		[DataMember]
		public string DocumentType { get; set; }
		[DataMember]
		public string DocumentDate { get; set; }
		[DataMember]
		public string FileType { get; set; }
		[DataMember]
		public string Comment { get; set; }
		[DataMember]
		public bool SkipWorkflow { get; set; }
		[DataMember]
		public List<UnityField> Fields { get; set; }
		[DataMember]
		public List<UnityDocument> Documents { get; set; }
		[DataMember]
		public List<SecurityScanRequest> DocumentsIds { get; set; }
	}

	[DataContract]
	public class UnityField
	{
		[DataMember]
		public string FieldName { get; set; }
		[DataMember]
		public string ObType { get; set; }
		[DataMember]
		public string DataType { get; set; }
		[DataMember]
		public string Value { get; set; }
	}

	[DataContract]
	public class UnityDocument
	{
		[DataMember]
		public string FileExtension { get; set; }
		[DataMember]
		public byte[] DocumentData { get; set; }
	}
}