using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunBlock.DataTransferObjects.Attributes
{
    /// <summary>
    /// Determines if a property contains nested fields that should be promoted to the parent level when serialized to Azure Storage
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryableMembersAttribute : Attribute
    {
    }
}
