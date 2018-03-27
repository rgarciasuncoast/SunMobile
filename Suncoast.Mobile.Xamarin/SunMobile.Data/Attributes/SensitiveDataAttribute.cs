using System;

namespace SunBlock.DataTransferObjects.Attributes
{
    /// <summary>
    /// Determines if a field needs to be encrypted before being stored on the Cloud. Applies only to System.String and System.Byte[] Properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveDataAttribute : Attribute
    {
    }
}
