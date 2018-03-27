using System;
using Foundation;
using UIKit;

namespace SunMobile.iOS.Common
{
    public class PopUpViewer : UIView
    {
        public event Action<bool> CloseSelected = delegate{};    

        public PopUpViewer(UIView parent)
        {
            BackgroundColor = UIColor.Yellow;
            Frame = new CoreGraphics.CGRect(40, 40, parent.Frame.Width - 80, parent.Frame.Height - 180);

            var button = new UIButton(UIButtonType.RoundedRect);
            button.TouchUpInside += (sender, e) => CloseSelected(true);
            button.SetTitle("Close", UIControlState.Normal);
            button.Center = Center;
            button.Frame = new CoreGraphics.CGRect(button.Frame.X, Frame.Height - 40, button.Frame.Width, button.Frame.Height);
            AddSubview(button);

            var webView = new UIWebView();
            //webView.Frame = new CoreGraphics.CGRect(Frame.X, Frame.Y, Frame.Width, Frame.Height - 60);
            webView.Frame = Frame;
            webView.ScalesPageToFit = true;
            //AddSubview(webView);
            var url = "https://cbsnews2.cbsistatic.com/hub/i/r/2018/02/06/810f9f99-8065-4f21-8db3-6dd61d27fdc3/resize/620x/00b3622dd86293cd9ac5d136d9e26694/tesla-in-space.gif";
            webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
        }
    }
}