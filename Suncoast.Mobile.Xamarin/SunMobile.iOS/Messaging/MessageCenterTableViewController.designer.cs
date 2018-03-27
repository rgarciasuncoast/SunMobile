// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SunMobile.iOS.Messaging
{
    [Register ("MessageCenterTableViewController")]
    partial class MessageCenterTableViewController
    {
        [Outlet]
        UIKit.UILabel lblAlertsInboxUnreadCount { get; set; }


        [Outlet]
        UIKit.UILabel lblMessageCenterComposeMessage { get; set; }


        [Outlet]
        UIKit.UILabel lblMessageCenterInbox { get; set; }


        [Outlet]
        UIKit.UILabel lblMessageCenterNotifications { get; set; }


        [Outlet]
        UIKit.UILabel lblMessageCenterSent { get; set; }


        [Outlet]
        UIKit.UILabel lblSecuredInboxUnreadCount { get; set; }


        [Outlet]
        UIKit.UILabel lblSecuredNotificationsUnreadCount { get; set; }


        [Outlet]
        UIKit.UITableView tableViewMenu { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lblMessageCenterComposeMessage != null) {
                lblMessageCenterComposeMessage.Dispose ();
                lblMessageCenterComposeMessage = null;
            }

            if (lblMessageCenterInbox != null) {
                lblMessageCenterInbox.Dispose ();
                lblMessageCenterInbox = null;
            }

            if (lblMessageCenterNotifications != null) {
                lblMessageCenterNotifications.Dispose ();
                lblMessageCenterNotifications = null;
            }

            if (lblMessageCenterSent != null) {
                lblMessageCenterSent.Dispose ();
                lblMessageCenterSent = null;
            }

            if (lblSecuredInboxUnreadCount != null) {
                lblSecuredInboxUnreadCount.Dispose ();
                lblSecuredInboxUnreadCount = null;
            }

            if (lblSecuredNotificationsUnreadCount != null) {
                lblSecuredNotificationsUnreadCount.Dispose ();
                lblSecuredNotificationsUnreadCount = null;
            }

            if (tableViewMenu != null) {
                tableViewMenu.Dispose ();
                tableViewMenu = null;
            }
        }
    }
}