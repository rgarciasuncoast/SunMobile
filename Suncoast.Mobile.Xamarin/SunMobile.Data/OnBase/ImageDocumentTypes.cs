using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.OnBase
{
    [DataContract]
    public enum ImageDocumentTypes
    {
        [EnumMember]
        NotDefined,
        [EnumMember]
        BeneficiaryCardRequest,
        [EnumMember]
        AllBeneficiaryCardsRequest,
        [EnumMember]
        DriversLicenseImages,
        [EnumMember]
        CheckImages,
        [EnumMember]
        ElectronicStatement,
        [EnumMember]
        CreditCardStatementImages,
        [EnumMember]
        TaxReturnForm,
        [EnumMember]
        Enotices,
        [EnumMember]
        JointOwnerInformationForm,
        [EnumMember]
        SignatureCardForm,
        [EnumMember]
        RemoteDepositCaptureCheck,
        [EnumMember]
        MortgageStatement
    }
} 