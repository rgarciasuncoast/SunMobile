using System;

namespace SunBlock.DataTransferObjects.Collections
{
    [Serializable]
    public class NameValueCollection: System.Collections.ObjectModel.KeyedCollection<string, NameValueItem>
    {
        #region Declarations...

        #endregion

        #region Properties...

        #endregion

        #region Methods...

        protected override string GetKeyForItem(NameValueItem item)
        {
            return item.Name;
        }

        public void Add(string name, string value)
        {
            NameValueItem itemToAdd;
            if (!Contains(name))
            {
                itemToAdd = new NameValueItem(name, value);
                Add(itemToAdd);
            }
            else
            {
                itemToAdd = this[name];
                itemToAdd.Value = value;
            }
            
        }


        #endregion
    }
}
