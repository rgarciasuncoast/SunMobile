using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
    [DataContract]
    public class ProductType<T>
    {
        [DataMember]
        public string ProductTypeName { get; set; }
        [DataMember]
        public Collection<T> Products { get; set; }
    }
}
