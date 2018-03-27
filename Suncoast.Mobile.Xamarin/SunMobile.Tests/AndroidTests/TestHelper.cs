using System;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;

namespace AndroidTests
{
	public static class TestHelper
	{
		public static void ScrollDownAndTap(this AndroidApp app, Func<AppQuery, AppQuery> lambda = null, string screenshot = null)
		{
			app.ScrollDownEnough(lambda);
			app.Tap(lambda);

			if (screenshot != null)
			{
				app.Screenshot(screenshot);
			}
		}

		public static void ScrollDownAndTap(this AndroidApp app, string screenshot, Func<AppQuery, AppQuery> lambda = null)
		{
			app.ScrollDownEnough(lambda);

			if (screenshot != null)
			{
				app.Screenshot(screenshot);
			}

			app.Tap(lambda);
		}

		public static bool GetHeightWidth(this IApp app, Func<AppQuery, AppQuery> query, out int outX, out int outY)
		{
			AppResult[] queryResult = app.Query(query);

			outX = 0;
			outY = 0;

			if (queryResult != null)
			{
				AppResult result = queryResult[0];
				var viewBounds = result.Rect;
				outX = Convert.ToInt32(viewBounds.Width);
				outY = Convert.ToInt32(viewBounds.Height);
			}

			return true;
		}

		/// <summary>
		/// Incrementally scrolls down until the desired elements are found
		/// </summary>
		public static AppResult[] ScrollDownEnough(this AndroidApp app, Func<AppQuery, AppQuery> lambda, string screenshot = null)
		{
			AppResult rootView = null;
			int count = 0;
			const int maxTries = 20;
			AppResult[] lastTry;

			while (count < maxTries)
			{
				lastTry = app.Query(lambda);

				if (lastTry.Any())
				{
					if (screenshot != null)
					{
						app.Screenshot(screenshot);
					}

					return lastTry;
				}

				if (rootView == null)
				{
					rootView = app.Query(e => e.All()).FirstOrDefault();

					if (rootView == null)
					{
						throw new Exception("Unable to get root view");
					}
				}

				//Will try to scroll +/-100 from the vertical center point
				float gap = 100;

				//Take into account where the screen is not large and the gap would be too big
				if (rootView.Rect.Height < gap * 2)
				{
					gap = rootView.Rect.Height / 4;
				}

				app.DragCoordinates(rootView.Rect.CenterX, rootView.Rect.CenterY + gap, rootView.Rect.CenterX, rootView.Rect.CenterY - gap);
				count++;
			}

			if (count == maxTries)
			{
				throw new Exception("Unable to scroll down to find element");
			}

			return new AppResult[0];
		}

		public static void WaitThenEnterText(this IApp app, Func<AppQuery, AppQuery> lambda, string text, string screenshot = null)
		{
			app.WaitForElement(lambda);
			app.EnterText(lambda, text);

			if (screenshot != null)
			{
				app.Screenshot(screenshot);
			}
		}

		public static void EnterText(this IApp app, Func<AppQuery, AppQuery> lambda, string text, string screenshot)
		{
			app.EnterText(lambda, text);
			app.Screenshot(screenshot);
		}

		public static void Tap(this IApp app, string screenshot, Func<AppQuery, AppQuery> lambda)
		{
			app.Screenshot(screenshot);
			app.Tap(lambda);
		}

		public static void Tap(this IApp app, Func<AppQuery, AppQuery> lambda, string screenshot)
		{
			app.Tap(lambda);
			app.Screenshot(screenshot);
		}

		public static void WaitThenTapIfExists(this IApp app, Func<AppQuery, AppQuery> lambda, int timeout = 5, string screenshot = null)
		{
			int count = 0;

			while (count < timeout && app.Query(lambda).Length == 0)
			{
				Thread.Sleep(1000);
				count++;
			}

			if (app.Query(lambda).Length > 0)
			{
				if (screenshot != null)
				{
					app.Screenshot(screenshot);
				}

				app.Tap(lambda);
			}
		}

		public static void WaitThenTap(this IApp app, string screenshot, Func<AppQuery, AppQuery> lambda, int seconds = 20)
		{
			app.WaitForElement(lambda, "Timed out waiting for element", TimeSpan.FromSeconds(seconds));

			if (screenshot != null)
			{
				app.Screenshot(screenshot);
			}

			app.Tap(lambda);
		}

		public static void WaitThenTap(this IApp app, Func<AppQuery, AppQuery> lambda, string screenshot = null)
		{
			app.WaitForElement(lambda);
			app.Tap(lambda);

			if (screenshot != null)
			{
				app.Screenshot(screenshot);
			}
		}

		public static string ToString(this AppResult[] result)
		{
			var sb = new StringBuilder();
			var index = 0;

			foreach (var res in result)
			{
				var innerSb = new StringBuilder();
				innerSb.AppendLine("{");
				innerSb.AppendLine(string.Format("    Index         - {0}", index));
				innerSb.AppendLine(string.Format("    Class         - {0}", res.Class));
				innerSb.AppendLine(string.Format("    Description   - {0}", res.Description));

				if (res.Text != null)
				{
					innerSb.AppendLine(string.Format("    Text           - {0}", res.Text));
				}

				innerSb.AppendLine(string.Format("    ID            - {0}", res.Id));
				innerSb.AppendLine(string.Format("    Rect          - {0} x {1}, {2} x {3}", res.Rect.X, res.Rect.Y, res.Rect.Width, res.Rect.Height));
				innerSb.AppendLine("}");
				innerSb.AppendLine("");

				sb.Append(innerSb.ToString());
				index++;
			}

			return sb.ToString();
		}

		public static bool IsTextDifferent(string oldText, string newText)
		{
			bool bReturn = false;

			if (!string.IsNullOrEmpty(oldText) && !string.IsNullOrEmpty(newText))
			{
				bReturn |= string.Compare(oldText, newText) != 0;
			}

			return bReturn;
		}

		public static void WaitForThenGetTextOfIndexAndCompare(this IApp app, Func<AppQuery, AppQuery> lambda, int index, string sExpected, string screenShotText)
		{
			string sActual;
			WaitFor(app, lambda);

			if (app.Query(lambda).Length >= index)
			{
				sActual = app.Query(lambda)[index].Text;
				CompareText(sActual, sExpected);
				Screenshot(app, screenShotText);
			}
		}

		private static void CompareText(string sExpected, string sActual)
		{
			if (!string.IsNullOrEmpty(sActual) && !string.IsNullOrEmpty(sExpected))
			{
				if (String.Compare(sExpected, sActual) != 0)
				{
					Assert.Fail(string.Format("TEST FAILURE. Expecting: {0} , Actual: {1}", sExpected, sActual));
				}
			}
		}

		public static void WaitForThenTap(this IApp app, Func<AppQuery, AppQuery> lambda, string screenShotText, int seconds = 30)
		{
            WaitFor(app, lambda, seconds);
			app.Tap(lambda);
			Screenshot(app, screenShotText);
		}

		public static void WaitForThenBack(this IApp app, Func<AppQuery, AppQuery> lambda, string screenShotText, int sleep = 0)
		{
			WaitFor(app, lambda);

			if (sleep > 0)
			{
				Thread.Sleep(sleep);
			}

			app.Back();
			Screenshot(app, screenShotText);
		}

		public static void WaitFor(this IApp app, Func<AppQuery, AppQuery> lambda, int seconds = 30)
		{
            app.WaitForElement(lambda, $"Timed out after {seconds} seconds", new TimeSpan(0, 0, 0, seconds, 0), null, null);
		}

		public static void WaitForThenEnterText(this IApp app, Func<AppQuery, AppQuery> lambda, string text, string screenShotText)
		{
			WaitFor(app, lambda);
			app.EnterText(lambda, text);
			Screenshot(app, screenShotText);
		}

		public static void WaitForThenClearText(this IApp app, Func<AppQuery, AppQuery> lambda)
		{
			WaitFor(app, lambda);
			app.DoubleTap(lambda);
			app.ClearText();
		}

		public static void Screenshot(this IApp app, string screenShotText)
		{
			if (screenShotText.Length > 0)
			{
				app.Screenshot(screenShotText);
			}
		}

		public static bool IsItThere(this IApp app, Func<AppQuery, AppQuery> lambda, int secondsToWait = 30)
		{
			bool bIsThere;

			try
			{
				app.WaitForElement(lambda, "Timed out", new TimeSpan(0, 0, 0, secondsToWait, 0), null, null);
				bIsThere = true;
			}
			catch (Exception)
			{
				bIsThere = false;
			}

			return bIsThere;
		}

		public static void IsItThere(this IApp app, Func<AppQuery, AppQuery> lambda, string screenShotText)
		{
			WaitFor(app, lambda);
			Screenshot(app, screenShotText);

			if (app.Query(lambda).Length <= 0)
			{
				Assert.Fail(string.Format("FAILURE"), screenShotText);
			}
		}
	}
}