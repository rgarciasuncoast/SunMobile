using SunBlock.DataTransferObjects.UserInterface.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts
{
    [DataContract]
    public class TransactionModel<TListItem> : ListItemModel<TListItem> where TListItem : ListItem
    {

        [DataMember]
        public Transaction TransactionInformation { get; set; }
    }
}
