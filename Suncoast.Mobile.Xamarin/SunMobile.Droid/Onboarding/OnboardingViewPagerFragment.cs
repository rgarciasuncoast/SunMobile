using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.OnBoarding;
using SunBlock.DataTransferObjects.Session;
using SunMobile.Shared;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Onboarding
{
	public class OnboardingViewPagerFragment : BaseFragment
	{
		public event Action<string> Completed = delegate { };
		private OnboardingCarousel _onboardingCarousel;
		private OnboardingViewPager viewPager;
		private LinearLayout dotsLayout;
		private TextView[] _dots;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.OnboardingViewPager, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
		}

		public override async void OnActivityCreated(Bundle savedInstanceState)
		{
            try
            {
                base.OnActivityCreated(savedInstanceState);

                ((MainActivity)Activity).SupportActionBar.Hide();

                viewPager = Activity.FindViewById<OnboardingViewPager>(Resource.Id.viewPager);
                viewPager.PageSelected += (sender, e) =>
                {
                    OnPageSelected(e.Position);
                };

                dotsLayout = Activity.FindViewById<LinearLayout>(Resource.Id.viewPagerCountDots);

                await LoadOnboardingInfo();

                if (_onboardingCarousel != null)
                {
                    _dots = new TextView[_onboardingCarousel.CarouselItems.Count];

                    for (int i = 0; i < _dots.Length; i++)
                    {
                        _dots[i] = new TextView(Activity);
                        #pragma warning disable CS0618 // Type or member is obsolete
                        _dots[i].Text = Html.FromHtml("&#8226;").ToString();
                        #pragma warning restore CS0618 // Type or member is obsolete
                        _dots[i].TextSize = 30;
                        dotsLayout.AddView(_dots[i]);
                    }

                    ShowActivityIndicator();

                    viewPager.StoreAdapter(new OnboardingViewPagerAdapter(Activity.SupportFragmentManager, _onboardingCarousel.CarouselItems, this));

                    HideActivityIndicator();

                    OnPageSelected(0);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "OnboardingViewPagerFragment:OnActivityCreated");
                Skip();
            }
            finally
            {
                RetainedSettings.Instance.ShowOnboardingFirstTime = false;
                RetainedSettings.Instance.ShowOnboardingUpdate = false;
            }
		}

		public void OnPageSelected(int position)
		{
			for (int i = 0; i < _dots.Length; i++)
			{
				_dots[i].SetTextColor(Color.Black);
			}

			_dots[position].SetTextColor(Color.White);
		}

		private async Task LoadOnboardingInfo()
		{
			try
			{
				var version = string.Empty;

				var authenticationMethods = new AuthenticationMethods();
				var settingsRequest = new GetStartupSettingsRequest();
				var settingsResponse = await authenticationMethods.GetStartupSettings(settingsRequest, null);

				if (settingsResponse != null)
				{
					var dict = new Dictionary<string, string>();

					for (int i = 0; i < settingsResponse.Keys.Count; i++)
					{
						dict.Add(settingsResponse.Keys[i], settingsResponse.Values[i]);
					}

					SessionSettings.Instance.GetStartupSettings = dict;

					var enableOnboarding = SessionSettings.Instance.GetStartupSettings["EnableOnboarding"];

					if (enableOnboarding == "true")
					{
						if (RetainedSettings.Instance.ShowOnboardingFirstTime)
						{
							version = "0";
						}
						else if (RetainedSettings.Instance.ShowOnboardingUpdate)
						{
							version = GeneralUtilities.GetAppShortVersionNumber(Activity);
						}

						var methods = new OnboardingMethods();
						var request = new GetOnboardingInfoRequest { Version = version, PictureType = OnboardingPictureTypes.Standard.ToString() };

						ShowActivityIndicator();

						var response = await methods.GetOnboardingInfo(request, Activity);

						if (response?.Result?.CarouselItems != null && response.Result.CarouselItems.Count > 0)
						{
							_onboardingCarousel = response.Result;
						}
						else if (!string.IsNullOrEmpty(version))
						{
							request.Version = "0";
							response = await methods.GetOnboardingInfo(request, Activity);

							if (response?.Result?.CarouselItems != null && response.Result.CarouselItems.Count > 0)
							{
								_onboardingCarousel = response.Result;
							}
						}

						HideActivityIndicator();
					}
				}

				if (_onboardingCarousel?.CarouselItems == null || _onboardingCarousel.CarouselItems.Count == 0)
				{
					Skip();
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "OnboardingViewPagerFragment:LoadOnboardingInfo");
                Skip();
			}
		}

		public void Skip()
		{
			((MainActivity)Activity).SupportActionBar.Show();
			Completed("Success");
		}
	}
}