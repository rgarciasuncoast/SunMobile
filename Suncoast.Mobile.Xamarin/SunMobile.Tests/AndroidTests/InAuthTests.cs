using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;

namespace AndroidTests
{
	[TestFixture]
	public class InAuthTests
	{
		AndroidApp app;

		[SetUp]
		public void BeforeEachTest()
		{
			app = ConfigureApp
				.Android
				.ApkFile("/Projects/Mobile/InAuth/SunTest7821.apk")
				.StartApp();
		}

		[Test]
		public void InAuth_Valid()
		{
			app.WaitForThenTap(x => x.Id("textView4"), "Wait for the Time label to appear.");
			Thread.Sleep(20000);
			app.WaitForThenTap(x => x.Id("textView4"), "Wait twenty seconds.");
		}
	}
}	