namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts
{
    public enum LoginState
    {
        PasswordMismatch, UserLockedOut, Authorized, Restricted,
        NoSuchAccount,
        UnknownError,
        AuthorizedTemporaryPin,
        TemporaryPinExpired
    }
}