using System;
using System.Runtime.Serialization;

namespace SunBlock.DataTransferObjects.Collections
{
    [DataContract]
    public class NameValueItem
    {
        #region Declarations...

        #endregion

        #region Properties...

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        #endregion

        #region Methods...

        public NameValueItem()
        {}

        public NameValueItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        #endregion
    }
}
