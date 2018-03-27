namespace SunBlock.DataTransferObjects.Host
{
    public enum StopPaymentResults
    {
        UnexpectedError = -1,
        Success = 0,
        CheckRangeTooLarge = 37,
        OneOrMoreDraftsHasCleared = 38,
        StopPaymentExistsAlreadyForAllDrafts = 39,
        StopExistsForAllAndOneOrMoreHasCleared = 40
    }
}

