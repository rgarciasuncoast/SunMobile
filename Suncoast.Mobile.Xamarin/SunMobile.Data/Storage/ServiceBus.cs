namespace SunBlock.DataTransferObjects.Storage
{
    /// <summary>
    /// Processes the message that has been peeked off of the queue if an exception or false is returned, Abandon() will be called. If true is returned, Complete() is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="message"></param>
    /// <returns></returns>
    public delegate bool ProcessMessage<T>(T message);
}
