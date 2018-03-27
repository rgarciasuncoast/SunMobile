using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Utilities.Serialization;
using ModernHttpClient;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.Mobile;

#if __IOS__
using SunMobile.iOS;
using SunMobile.iOS.Authentication;
using SunMobile.Shared.Culture;
using UIKit;
#endif

#if __ANDROID__
using Android.App;
using Android.Content;
using SunMobile.Droid;
using SunMobile.Droid.Authentication;
using SunMobile.Shared.Data;
using Plugin.CurrentActivity;
using SunMobile.Shared.Culture;
#endif

namespace SunMobile.Shared.Utilities.Web
{
	public class SunBlockServiceBase
	{
		public async Task<TResponseType> PostToSunBlock<TResponseType>(string url, object request, string token, object view, bool suppressNetworkError = false, OutOfBandTransactionTypes outOfBandTransactionType = OutOfBandTransactionTypes.Profile, object navigationController = null, object viewToRunAfterValidation = null)
		{
			#if __ANDROID__ 
			if (view == null)
			{
				view = CrossCurrentActivity.Current.Activity;
			}
			#endif

			TResponseType response = default(TResponseType);

			HttpClient httpClient;
			HttpResponseMessage result;

			bool isTimeout = false;
			bool isNetworkError = false;

			try
			{
				var handler = new NativeMessageHandler(false, true);
				handler.DisableCaching = true;
				httpClient = new HttpClient(handler);

				#if DEBUG
				// Disable https certificate check.
				ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
				#endif

				httpClient.Timeout = TimeSpan.FromSeconds(60);

				// Add the Token to the header
				if (!string.IsNullOrEmpty(token))
				{
					httpClient.DefaultRequestHeaders.Add("SessionToken", token);
				}

				var body = string.Empty;

				if (request != null)
				{
					body = Json.Serialize(request);
				}

				Logging.Logging.Log(string.Format("\n\nREQUEST:\n----------\n{0}\n{1}\n", url, body));

				result = await httpClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
				string resultContent = await result.Content.ReadAsStringAsync();
				Logging.Logging.Log(string.Format("\n\nRESPONSE:\n----------\n{0}\n", resultContent));

				if (result.IsSuccessStatusCode)
				{
					response = Json.Deserialize<TResponseType>(resultContent);

					if (EqualityComparer<TResponseType>.Default.Equals(response, default(TResponseType)))
					{
						Logging.Logging.Log("Error in SunBlockServiceBase: Response is null.");
					}
					else
					{
						if (response is MobileStatusResponse)
						{
							var mobileStatusResponse = Json.Deserialize<MobileStatusResponse>(resultContent);

							if (mobileStatusResponse.OutOfBandChallengeRequired)
							{
								#if __IOS__
								var accountVerificationViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountVerificationViewController") as AccountVerificationViewController;
								accountVerificationViewController.Header = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "14D29C9D-B918-4864-9006-CD0B3F7FA7A7", "Account Verification");
								accountVerificationViewController.OutOfBandTransactionType = outOfBandTransactionType.ToString();
								accountVerificationViewController.CanUseAtmLastEight = mobileStatusResponse.CanUseAtmLastEight;

								accountVerificationViewController.Completed += (isValidated) =>
								{
									if (navigationController != null)
									{
										((UINavigationController)navigationController).PopViewController(true);
									}
									else
									{
										AppDelegate.MenuNavigationController.PushViewController((UIViewController)viewToRunAfterValidation, true);
									}

									var currentViewController = AppDelegate.MenuNavigationController.CurrentViewController;

									if (currentViewController is IVerificationViewController)
									{
										((IVerificationViewController)currentViewController).OnAccountVerified();
									}
								};

								if (navigationController != null)
								{
									((UINavigationController)navigationController).PushViewController(accountVerificationViewController, true);
								}
								else
								{
									AppDelegate.MenuNavigationController.PushViewController(accountVerificationViewController, true);
								}
								#endif
								#if __ANDROID__
								if (viewToRunAfterValidation != null)
								{
									var accountVerificationFragment = new AccountVerificationFragment();
									accountVerificationFragment.Header = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "14D29C9D-B918-4864-9006-CD0B3F7FA7A7", "Account Verification");
									accountVerificationFragment.OutOfBandTransactionType = outOfBandTransactionType.ToString();
									accountVerificationFragment.CanUseAtmLastEight = mobileStatusResponse.CanUseAtmLastEight;
									accountVerificationFragment.Completed += (isValidated) =>
									{
										((MainActivity)view).SupportFragmentManager.BeginTransaction()
											.Replace(Droid.Resource.Id.content_frame, (Android.Support.V4.App.Fragment)viewToRunAfterValidation)
											.Commit();
									};

									((MainActivity)view).SupportFragmentManager.BeginTransaction()
										.Replace(Droid.Resource.Id.content_frame, accountVerificationFragment)
										.AddToBackStack(null)
										.Commit();
								}
								else
								{
									var currentFragment = ((MainActivity)view).SupportFragmentManager.Fragments[0];
									if (currentFragment == null && ((MainActivity)view).SupportFragmentManager.Fragments.Count > 1)
									{
										currentFragment = ((MainActivity)view).SupportFragmentManager.Fragments[1];
									}
									var intent = new Intent((Activity)view, typeof(AccountVerificationActivity));
									intent.PutExtra("OutOfBandTransactionType", outOfBandTransactionType.ToString());
									intent.PutExtra("CanUseAtmLastEight", mobileStatusResponse.CanUseAtmLastEight);
									((Android.Support.V4.App.Fragment)currentFragment).StartActivityForResult(intent, (int)ActivityResults.AccountVerification);
								}
								#endif
							}
						}
					}
				}
				else
				{
					if (result.Headers.Contains("SessionStatus"))
					{
						bool isExpired = false;

						var status = result.Headers.GetValues("SessionStatus");

						foreach (string s in status)
						{
							if (s == "Expired")
							{
								isExpired = true;
							}
						}

						if (isExpired)
						{
							isTimeout = true;
						}
						else
						{
							isNetworkError = true;
						}
					}
					else
					{
						isNetworkError = true;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "SunBlockServiceBase:PostToSunBlock");
				isNetworkError = true;
			}

			try
			{
				if (suppressNetworkError)
				{
					isNetworkError = false;
				}

				if (isTimeout)
				{
					#if __IOS__
					AppDelegate.MenuNavigationController.TimeOut();
					#endif

					#if __ANDROID__
					((MainActivity)view).Timeout();
					#endif
				}
				else if (isNetworkError)
				{
					#if __IOS__
					AppDelegate.MenuNavigationController.NetworkError();
					#endif

					#if __ANDROID__
					((MainActivity)view).NetworkError();
					#endif
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "SunBlockServiceBase:PostToSunBlock");
			}

			return response;
		}
	}
}