using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Utilities.Serialization;

#if __IOS__
using SunMobile.iOS;
#endif

#if __ANDROID__
using Android.Content;
using Android.App;
using Android.Views.InputMethods;
using SunMobile.Droid;
#endif

namespace SunMobile.Shared.Utilities.Web
{
	public class SunBlockServiceBaseOld
	{
		public async Task<TResponseType> PostToSunBlock<TResponseType>(string url, object request, string token, object view)
		{
			TResponseType response = default(TResponseType);

			HttpClient httpClient;
			HttpResponseMessage result;

			bool isTimeout = false;
			bool isNetworkError = false;

			try
			{
				httpClient = new HttpClient();

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

				var body = Json.Serialize(request);
#if DEBUG
				Console.WriteLine(string.Format("\n\nREQUEST:\n----------\n{0}\n{1}\n", url, body));
#endif

				result = await httpClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
				string resultContent = await result.Content.ReadAsStringAsync();
#if DEBUG
				Console.WriteLine(string.Format("\n\nRESPONSE:\n----------\n{0}\n", resultContent));
#endif

				if (result.IsSuccessStatusCode)
				{
					response = Json.Deserialize<TResponseType>(resultContent);

					if (EqualityComparer<TResponseType>.Default.Equals(response, default(TResponseType)))
					{
						Logging.Logging.Log("Error in SunBlockServiceBase: Response is null.");
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