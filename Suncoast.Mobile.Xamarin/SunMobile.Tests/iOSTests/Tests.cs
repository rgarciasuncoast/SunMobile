using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;

namespace SunMobile.Tests.iOSTests
{
	[TestFixture]
	public class Tests
	{
		iOSApp app;

		[SetUp]
		public void BeforeEachTest()
		{
			// If the iOS app being tested is included in the solution then open
			// the Unit Tests window, right click Test Apps, select Add App Project
			// and select the app projects that should be tested.
			app = ConfigureApp
				.iOS
				// Update this path to point to your iOS app and uncomment the
				// code if the app is not included in the solution.
				//.AppBundle ("../../../iOS/bin/iPhoneSimulator/Debug/SunMobile.iOS.Tests.iOS.app")
				.AppBundle ("/Projects/Mobile/Suncoast.Mobile.Xamarin/SunMobile.iOS/bin/iPhoneSimulator/Debug/SunMobileiOS.app")
				.StartApp();
		}

		[Test]
		public void AppLaunches()
		{
			app.Repl();
		}

		[Test]
		public void Repl_Test()
		{
			app.Repl();
		}
	}
}