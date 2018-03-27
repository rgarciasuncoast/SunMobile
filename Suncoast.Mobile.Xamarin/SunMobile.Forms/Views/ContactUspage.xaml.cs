using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SunMobile.Forms.Views
{
    public partial class ContactUspage : ContentPage
    {
        public ContactUspage()
        {
            InitializeComponent();

            //Guesture for layouts
            PhoneNumberLayout.GestureRecognizers.Add(
                                 new TapGestureRecognizer()
                                 {
                                     Command = new Command(() =>
                                     {
                                         Device.OpenUri(new System.Uri("tel:8009995887"));
                                     })
                                 });

            EmailLayout.GestureRecognizers.Add(
                                 new TapGestureRecognizer()
                                 {
                                     Command = new Command(() =>
                                     {
                                         Device.OpenUri(new System.Uri("mailto:"));
                                     })
                                 });

            CreditCardDetails.GestureRecognizers.Add(
                                 new TapGestureRecognizer()
                                 {
                                     Command = new Command(() =>
                                     {
                                         Device.OpenUri(new System.Uri("tel:8006457728"));
                                     })
                                 });

            DebitCardDetails.GestureRecognizers.Add(
                                 new TapGestureRecognizer()
                                 {
                                     Command = new Command(() =>
                                     {
                                         Device.OpenUri(new System.Uri("tel:8009995887"));
                                     })
                                 });
            DebitCardDetailsafterHours.GestureRecognizers.Add(
                               new TapGestureRecognizer()
                               {
                                   Command = new Command(() =>
                                   {
                                       Device.OpenUri(new System.Uri("tel:8007544128"));
                                   })
                               });

            EmailTechnicalSupportLayout.GestureRecognizers.Add(
                               new TapGestureRecognizer()
                               {
                                   Command = new Command(() =>
                                   {
                                       Device.OpenUri(new System.Uri("mailto:digitalbanking@suncoastcreditunion.com"));
                                   })
                               });

            SunCoast.GestureRecognizers.Add(
                               new TapGestureRecognizer()
                               {
                                   Command = new Command(() =>
                                   {
                                       NavigateToWebView("http://www.suncoastcreditunion.com", "Suncoast Credit Union");
                                   })
                               });

            Facebook.GestureRecognizers.Add(
                              new TapGestureRecognizer()
                              {
                                  Command = new Command(() =>
                                  {
                                      NavigateToWebView("https://www.facebook.com/SuncoastCreditUnion", "Suncoast Facebook");
                                  })
                              });

            Twitter.GestureRecognizers.Add(
                            new TapGestureRecognizer()
                            {
                                Command = new Command(() =>
                                {
                                    NavigateToWebView("https://twitter.com/SuncoastCU", "Suncoast Twitter");
                                })
                            });

        }

        void NavigateToWebView(String URL, String title)
        {
            var webViewController = new SunMobile.Forms.Views.WebLoadView(URL);

            webViewController.Title = title;
            this.Navigation.PushModalAsync(webViewController);


        }


    }
}
