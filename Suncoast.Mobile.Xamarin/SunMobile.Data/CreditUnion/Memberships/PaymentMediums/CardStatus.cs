namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums
{
    public enum CardStatus
    {
        NotActive = 0,
        Open = 1,
        Lost = 2,
        Stolen = 3,
        Restricted = 4,
        Expired = 8,
        Closed = 9
    }
}