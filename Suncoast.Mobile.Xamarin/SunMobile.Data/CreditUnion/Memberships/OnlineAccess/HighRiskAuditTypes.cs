namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess
{
    public enum HighRiskAuditTypes
    {
        MemberChallenged,
        VerificationPassed,
        VerificationFailed,
        VerificationLockedOut,
        TokenGenerated,
        InternalDataChange,
        TokenUpdated
    }
}
