using System;

namespace SunBlock.DataTransferObjects.Attributes
{
    /// <summary>
    /// Determines if a property should become a field when serialized to Azure Storage
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryableAttribute : Attribute
    {
       
    }
}
