namespace SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums
{
    public enum CardStockCodes
    {
        NotDefined = 0,
        AtmCardAccountNumber = (int)'C',
        VisaStockBlank = (int)' ',
        VisaStock = (int)'S',
        VisaGold = (int)'G',
        Business = (int)'B',
        Hsa = (int)'H',
        FlowerInstantIssue = (int)'F',
        ExecutiveInstantIssue = (int)'E',
        PalmTreeInstantIssue = (int)'P',
        MoneyInstantIssue = (int)'M',
        SolarDebitCard = (int)'L'
    }
}
