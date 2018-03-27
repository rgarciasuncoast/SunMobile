using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public enum RequestNames
    {
        NotDefined,
        Connect,
        Disconnect,
        QueryDocuments,
        GetDocumentData,
        StoreDocument,
        GetCollection,
        ServerSignDocument,
        VerifyDigitalSignature,
        CustomQuery
    }
}