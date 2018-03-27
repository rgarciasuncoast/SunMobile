using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid.Common
{
	[Activity(Label = "BaseListActivity")]	
    public class BaseListActivity : ListActivity, ICultureConfigurationProvider
    {
        protected LinearLayout LayoutMain;
        protected ListView ListViewMain;        
		private ProgressBar _progressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
        }

        public virtual void SetupView()
        {
            SetupView(0);
        }

		public virtual void SetCultureConfiguration()
		{
		}

        protected void SetupView(int resource)
        {
            if (resource == 0)
            {
                SetContentView(Resource.Layout.SunBlockListView);
            }
            else
            {
                SetContentView(resource);
            }

            LayoutMain = (LinearLayout)FindViewById(Resource.Id.layoutMain);
            ListViewMain = (ListView)FindViewById(Android.Resource.Id.List);            
        }

		public void ShowActivityIndicator(string message = "Loading...")
		{
			_progressBar = AlertMethods.ShowProgressBar(this, _progressBar);
		}

		public void HideActivityIndicator()
		{
			AlertMethods.HideProgressBar(this, _progressBar);
		}

		protected override void OnResume()
		{
			base.OnResume();

            try
            {
                SetCultureConfiguration();
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "BaseListActivity:OnResume");
			}
		}

		protected override void OnPause()
		{
			base.OnPause();

			AlertMethods.HideProgressBar(this, _progressBar);
		}
    }
}