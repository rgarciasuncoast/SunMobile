using System;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Shared.Culture
{
	public static class CultureTextProvider
	{
		public static string GetMobileResourceText(string viewId, string resourceId, string defaultText = "")
		{
			var returnValue = string.Empty;

			try
			{
				var cultureConfig = RetainedSettings.Instance.Culture;

				if (cultureConfig != null)
				{
					CultureView cultureView = null;

					foreach (var viewConfiguration in cultureConfig.ViewConfigurations)
					{
						if (viewConfiguration.Id.ToUpper() == viewId.ToUpper())
						{
							cultureView = viewConfiguration;
						}
					}

					if (cultureView != null)
					{
						CultureText cultureText = null;

						foreach (var textConfig in cultureView.TextConfigurations)
						{
							if (textConfig.Id.ToUpper() == resourceId.ToUpper())
							{
								cultureText = textConfig;
							}
						}

						if (cultureText != null)
						{
							var languageType = SessionSettings.Instance.Language;

							switch (languageType)
							{
								case LanguageTypes.English:
									returnValue = cultureText.EnglishText;
									break;
								case LanguageTypes.Spanish:
									returnValue = cultureText.SpanishText;
									break;
							}
						}
					}
				}

				if (string.IsNullOrEmpty(returnValue) && !string.IsNullOrEmpty(defaultText))
				{
					returnValue = defaultText;
				}

                returnValue = returnValue.Replace("\\n", "\n");
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "CultureTextProvider:GetMobileTextResource");
			}

			return returnValue;
		}

		public static CultureTextResponse GetMobileResourceTextAndFontSize(string viewId, string resourceId, string defaultText = "")
		{
			var returnValue = new CultureTextResponse { Text = string.Empty, FontSize = FontSizes.Default };

			try
			{
				var cultureConfig = RetainedSettings.Instance.Culture;

				if (cultureConfig != null)
				{
					CultureView cultureView = null;

					foreach (var viewConfiguration in cultureConfig.ViewConfigurations)
					{
						if (viewConfiguration.Id.ToUpper() == viewId.ToUpper())
						{
							cultureView = viewConfiguration;
						}
					}

					if (cultureView != null)
					{
						CultureText cultureText = null;

						foreach (var textConfig in cultureView.TextConfigurations)
						{
							if (textConfig.Id.ToUpper() == resourceId.ToUpper())
							{
								cultureText = textConfig;
							}
						}

						if (cultureText != null)
						{
							var languageType = SessionSettings.Instance.Language;

							switch (languageType)
							{
								case LanguageTypes.English:
									returnValue.Text = cultureText.EnglishText;
									returnValue.FontSize = FontSizes.Default;
									break;
								case LanguageTypes.Spanish:
									returnValue.Text = cultureText.SpanishText;
									returnValue.FontSize = FontSizes.Default;
									try
									{
										returnValue.FontSize = (FontSizes)Enum.Parse(typeof(FontSizes), cultureText.SpanishFontSize, true);
									}
									catch { }
									break;
							}
						}
					}
				}

				if (string.IsNullOrEmpty(returnValue.Text) && !string.IsNullOrEmpty(defaultText))
				{
					returnValue.Text = defaultText;
				}

                returnValue.Text = returnValue.Text.Replace("\\n", "\n");
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "CultureTextProvider:GetMobileTextResource");
			}

			return returnValue;
		}

		public static void SetMobileResourceText(object view, string viewId, string resourceId, string defaultText = "")
		{
			try
			{
				var viewResponse = GetMobileResourceTextAndFontSize(viewId, resourceId, defaultText);

				if (!string.IsNullOrEmpty(viewResponse.Text) && viewResponse.Text != defaultText)
				{
                    #if __IOS__
                    if (view is UIKit.UILabel)
                    {
                        ((UIKit.UILabel)view).Text = viewResponse.Text;
                        ((UIKit.UILabel)view).AdjustsFontSizeToFitWidth = true;
                    }
                    if (view is UIKit.UITextField)
                    {
                        ((UIKit.UITextField)view).Placeholder = viewResponse.Text;
                        ((UIKit.UITextField)view).AdjustsFontSizeToFitWidth = true;
                    }
                    if (view is UIKit.UITextView)
                    {
                        ((UIKit.UITextView)view).Text = viewResponse.Text;
                    }
                    if (view is UIKit.UIButton)
                    {
                        ((UIKit.UIButton)view).SetTitle(viewResponse.Text, UIKit.UIControlState.Normal);
                        ((UIKit.UIButton)view).TitleLabel.AdjustsFontSizeToFitWidth = true;
                    }
					if (view is UIKit.UIBarButtonItem)
					{
                        ((UIKit.UIBarButtonItem)view).Title = viewResponse.Text;						
					}
                    #endif

                    #if __ANDROID__
					var textSize = 14f;
					var microTextSize = 10f;

                    if (view is Android.Widget.EditText)
                    {
                        ((Android.Widget.EditText)view).Hint = viewResponse.Text;
                        textSize = ((Android.Widget.TextView)view).TextSize;
                    }
                    else if (view is Android.Widget.TextView)
                    {
                        ((Android.Widget.TextView)view).Text = viewResponse.Text;
                        textSize = ((Android.Widget.TextView)view).TextSize;
                    }
                    else if (view is Android.Widget.Button)
                    {
                        ((Android.Widget.Button)view).Text = viewResponse.Text;
                        textSize = ((Android.Widget.Button)view).TextSize;
                    }
                    else if (view is Android.Widget.SearchView)
                    {
						var linearLayout1 = (Android.Widget.LinearLayout)((Android.Widget.SearchView)view).GetChildAt(0);
						var linearLayout2 = (Android.Widget.LinearLayout)linearLayout1.GetChildAt(2);
						var linearLayout3 = (Android.Widget.LinearLayout)linearLayout2.GetChildAt(1);
						var autoComplete = (Android.Widget.AutoCompleteTextView)linearLayout3.GetChildAt(0);
                        autoComplete.Text = viewResponse.Text;
                        textSize = autoComplete.TextSize;
                    }

					var viewWidth = ((Android.Views.View)view).Width;
					((Android.Views.View)view).Measure(0, 0);
					var textWidth = ((Android.Views.View)view).MeasuredWidth;

					// Adjust Font
					while (textWidth > viewWidth && viewWidth != 0 && textSize > microTextSize && viewResponse.Text != defaultText)
					{
						textSize--;

						if (view is Android.Widget.EditText)
						{
							((Android.Widget.EditText)view).TextSize = textSize;
						}
						else if (view is Android.Widget.TextView)
						{
							((Android.Widget.TextView)view).TextSize = textSize;
						}
						else if (view is Android.Widget.Button)
						{
							((Android.Widget.Button)view).TextSize = textSize;
						}
                        else if (view is Android.Widget.SearchView)
                        {							
                            var linearLayout1 = (Android.Widget.LinearLayout)((Android.Widget.SearchView)view).GetChildAt(0);
                            var linearLayout2 = (Android.Widget.LinearLayout)linearLayout1.GetChildAt(2);
                            var linearLayout3 = (Android.Widget.LinearLayout)linearLayout2.GetChildAt(1);
                            var autoComplete = (Android.Widget.AutoCompleteTextView)linearLayout3.GetChildAt(0);
                            autoComplete.TextSize = textSize;
                        }

						viewWidth = ((Android.Views.View)view).Width;
						((Android.Views.View)view).Measure(0, 0);
						textWidth = ((Android.Views.View)view).MeasuredWidth;
					}
                    #endif
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "CultureTextProvider:SetMobileTextResource");
			}
		}

		public static string SELECT()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "0033F3CF-6EE6-4656-82F1-A002754C1E71", "Select");
		}

		public static string YES()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "34312383-4086-46F0-BB9F-1BEEDFB653C0", "Yes");
		}

		public static string NO()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "81F81C76-9BBB-4F6D-A317-51170F1F9CC9", "No");
		}

		public static string OK()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "E3FD18D4-A325-402B-998B-58A4082A225F", "OK");
		}

        public static string CONTINUE()
        {
            return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "E7EA9EA8-FBF4-4FBF-AF27-6D0FE87DDDC1", "Continue");   
        }

		public static string SUBMIT()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "D44241A8-E555-44F2-AC15-50109A425ABA", "Submit");
		}

		public static string NOREVIEW()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "BCC2404D-991C-4D88-91E0-D137E07EB55D", "No, Review");
		}

		public static string OPTIONAL()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "F42B3837-8C7A-4A30-B7AA-527DD36DF71D", "(Optional)");
		}

		public static string DONE()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "F9049F1D-FABD-47CE-81A7-96DE6B4F6ABA", "Done");
		}

		public static string BACK()
		{
			return GetMobileResourceText("62D45F28-550E-41B2-94BD-FC0E8C943713", "D22D8AA2-A7CE-42AC-9BC2-4AAF928C8F4E", "Back");
		}
	}
}