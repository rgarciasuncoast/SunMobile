using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SunMobile.Forms.Views
{
    public partial class WebLoadView : ContentPage
    {
        //public String URL;
        public WebLoadView( String URL)
        {
            InitializeComponent();
            //WebView loading
            var browser = new WebView();
            browser.BackgroundColor = Color.Red;
            browser.Source = URL;
            Content = browser;

        }
    }
}
