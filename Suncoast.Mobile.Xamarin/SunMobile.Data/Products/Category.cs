using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Products
{
    [DataContract]
    public class Category<T>
    {
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public Collection<ProductType<T>> Products { get; set; } 
    }
}
