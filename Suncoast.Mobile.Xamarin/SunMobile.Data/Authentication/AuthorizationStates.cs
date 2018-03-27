using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Authentication
{
    [DataContract]
    public enum AuthorizationStates
    {
        [EnumMember]
        AdaptiveAuthAnalyze = 1,
        [EnumMember]
        AuthenticateHost = 3,
        [EnumMember]
        AuthenticateHostUnlock = 5,
        [EnumMember]
        AdaptiveAuthUnlock = 7,
        [EnumMember]
        AdaptiveAuthChallenge = 9,
        [EnumMember]
        AdaptiveAuthNotEnrolled = 11,
        [EnumMember]
        AdaptiveAuthUpdateChallengeQuestions = 13,
        [EnumMember]
        AdaptiveAuthUpdateImagePhrase = 15,
        [EnumMember]
        AdaptiveAuthLockOut = 17,
        [EnumMember]
        AdaptiveAuthSessionExpired = 19,
        [EnumMember]
        Unauthorized = 21,
        [EnumMember]
        HostLockOut = 23,
        [EnumMember]
        Authenticated = 25,
        [EnumMember]
        UnexpectedFailure = 27,
        [EnumMember]
        AuthorizedTemporaryPin = 99
    }
}