// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SunMobile.iOS.Common
{
    [Register ("CustomPickerViewController")]
    partial class CustomPickerViewController
    {
        [Outlet]
        UIKit.UIButton lblSelect { get; set; }


        [Outlet]
        UIKit.UIPickerView pickerView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (pickerView != null) {
                pickerView.Dispose ();
                pickerView = null;
            }
        }
    }
}