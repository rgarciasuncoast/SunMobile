using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using System.Threading;

namespace AndroidTests
{
	[TestFixture]
	public class Tests
	{
		AndroidApp app;

		[SetUp]
		public void BeforeEachTest()
		{
			app = ConfigureApp
				.Android
				.ApkFile("/Projects/Mobile/Suncoast.Mobile.Xamarin/SunMobile.Droid/bin/Release/org.suncoast.mobile.apk")
				.StartApp();
		}

		private void Login()
		{
			if (app.IsItThere(x => x.Id("btnSkip"), 10))
			{
				app.WaitForThenTap(x => x.Id("btnSkip"), "Then I tap the Skip button.");
			}

			app.WaitForThenEnterText(x => x.Id("txtMemberId"), "4056097", "Then I enter the Member Number.");
			app.EnterText(x => x.Id("txtPin"), "Q@111222", "Then I enter the Password.");
			app.Tap(x => x.Id("btnSubmit"), "Then I click the Login button.");
			app.WaitForThenTap(x => x.Id("btnPrimary"), "Then I tap the Primary button.", 60);
		}

		private void Logout()
		{
            app.WaitForThenTap(x => x.Marked("Navigate up"), "Then I tap the menu icon.");			
            app.WaitForThenTap(x => x.Text("Sign Out"), "Then I tap Sign Out.");
		}

		[Test]
		public void Login_Valid()
		{
			Login();
		}

		[Test]
		public void Login_Three_Times_Valid()
		{
			Login();
			Logout();
			Login();
			Logout();
			Login();
			Logout();
		}

		[Test]
		public void Transfer_Valid()
		{
			Login();
			app.WaitForThenTap(x => x.Marked("Navigate up"), "Then I tap the menu icon.");
			app.WaitForThenTap(x => x.Text("Transfer Funds"), "Then I tap Transfer Funds.");
			app.WaitForThenTap(x => x.Id("rowFrom"), "Then I tap the Transfer Funds From row.");
			app.WaitForThenTap(x => x.Id("lblHeaderText"), "Then I tap the first account row.");
			app.WaitForThenTap(x => x.Id("rowTo"), "Then I tap the Transfer Funds To row.");
			app.WaitForThenTap(x => x.Id("lblHeaderText"), "Then I tap the first account row.");
			app.WaitForThenEnterText(x => x.Id("txtAmount"), "100", "Then I enter $1.00 in the Amount.");
			app.WaitForThenTap(x => x.Id("btnTransfer"), "Then I tap the Transfer button.");
			app.WaitForThenTap(x => x.Id("button1"), "Then I tap the Transfer Funds button.");
			app.WaitForThenTap(x => x.Id("button1"), "Then I tap the OK button.");
			Logout();			
		}

		[Test]
		public void Deposits_Valid()
		{
			Login();
			app.WaitForThenTap(x => x.Id("rowDeposits"), "Then I tap the Deposits row.");

			// Checks for Camera permissions
			if (app.IsItThere(x => x.Id("button1"), 5))
			{
				app.WaitForThenTap(x => x.Id("button1"), "Then I tap the Allow button.");
			}

			app.WaitForThenTap(x => x.Id("accountRow"), "Then I tap the Transfer Funds From row.");
			app.WaitForThenTap(x => x.Id("lblHeaderText"), "Then I tap the first account row.");
			app.WaitForThenEnterText(x => x.Id("txtAmount"), "100", "Then I enter $1.00 in the Amount.");
			app.WaitForThenTap(x => x.Id("imageFrontRow"), "Then I tap the Front of Check row.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");
			app.WaitForThenTap(x => x.Id("imageBackRow"), "Then I tap the Back of Check row.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");
			app.WaitForThenTap(x => x.Id("btnSubmit"), "Then I tap the Deposit button.");
			app.WaitForThenTap(x => x.Id("button1"), "Then I tap the Deposit Funds button.");
			app.WaitForThenTap(x => x.Id("button1"), "Then I tap the OK button.");
			Logout();
		}

		[Test]
		public void Deposits_TakePictures_Valid()
		{
			Login();

			app.WaitForThenTap(x => x.Marked("Navigate up"), "Then I tap the menu icon.");
			app.WaitForThenTap(x => x.Text("Deposit Funds"), "Then I tap Deposit Funds.");

			// Checks for Camera permissions
			if (app.IsItThere(x => x.Id("button1"), 5))
			{
				app.WaitForThenTap(x => x.Id("button1"), "Then I tap the Allow button.");
			}

			app.WaitForThenTap(x => x.Id("imageFrontRow"), "Then I tap the Front of Check row.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");
			app.WaitForThenTap(x => x.Id("imageBackRow"), "Then I tap the Back of Check row.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");

			Logout();
		}

		[Test]
		public void Deposits_TakePictures_WithInfo_Valid()
		{
            app.WaitForThenTap(x => x.Id("dummybutton"), "Then I tap the OK button.");

            Login();

			app.WaitForThenTap(x => x.Marked("Navigate up"), "Then I tap the menu icon.");
			app.WaitForThenTap(x => x.Text("Deposit Funds"), "Then I tap Deposit Funds.");			

			// Checks for Camera permissions
			if (app.IsItThere(x => x.Id("button1"), 5))
			{
				app.WaitForThenTap(x => x.Id("button1"), "Then I tap the Allow button.");
			}

			app.WaitForThenTap(x => x.Id("imageFrontRow"), "Then I tap the Front of Check row.");			
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");
			app.WaitForThenTap(x => x.Id("button1"), "Then I tap the OK button.");

			app.WaitForThenTap(x => x.Id("imageBackRow"), "Then I tap the Back of Check row.");			
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");
			app.WaitForThenTap(x => x.Id("button1"), "Then I tap the OK button.");

			Logout();
		}

		[Test]
		public void Deposits_TakeMultiplePictures_Valid()
		{
			Login();
			app.WaitForThenTap(x => x.Id("rowDeposits"), "Then I tap the Deposits row.");

			// Checks for Camera permissions
			if (app.IsItThere(x => x.Id("button1"), 5))
			{
				app.WaitForThenTap(x => x.Id("button1"), "Then I tap the Allow button.");
			}

			// Test with flash off
			app.WaitForThenTap(x => x.Id("imageFrontRow"), "Then I tap the Front of Check row.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");
			app.WaitForThenTap(x => x.Id("imageBackRow"), "Then I tap the Back of Check row.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");

			// Test again with flash on
			app.WaitForThenTap(x => x.Id("imageFrontRow"), "Then I tap the Front of Check row.");
			app.WaitForThenTap(x => x.Id("imageFlash"), "Then I tap the Flash button.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");
			app.WaitForThenTap(x => x.Id("imageBackRow"), "Then I tap the Back of Check row.");
			app.WaitForThenTap(x => x.Id("imageFlash"), "Then I tap the Flash button.");
			app.WaitForThenTap(x => x.Id("imageShutter"), "Then I tap the Shutter button.");
			app.WaitForThenTap(x => x.Id("btnUse"), "Then I tap the Use button.");

			Logout();
		}

		[Test]
		public void BillPay_SchedulePayment_Valid()
		{
			Login();
			app.WaitForThenTap(x => x.Id("rowBillPay"), "Then I tap the Bill Pay row.");
			app.WaitForThenTap(x => x.Id("rowSchedule"), "Then I tap the Schedule Payment row.");
			app.WaitForThenTap(x => x.Id("accountRow"), "Then I tap the Funding Account row.");
			app.WaitForThenTap(x => x.Id("lblHeaderText"), "Then I tap the first Account row.");
			app.WaitForThenTap(x => x.Id("bpPayeeRow"), "Then I tap the Payee row.");
			app.WaitForThenTap(x => x.Id("lblHeaderText"), "Then I tap the first Payeetr row.");
			app.WaitForThenEnterText(x => x.Id("txtAmount"), "100", "Then I enter $1.00 in the Amount.");
			app.WaitForThenTap(x => x.Id("bpWithdrawDateRow"), "Then I tap the Send On Date row.");
			app.WaitForThenBack(x => x.Id("btnNextMonth"), "Then I tap the Back button.");
			Logout();
		}

		[Test]
		public void Locations_Valid()
		{
			Login();
			app.WaitForThenTap(x => x.Id("rowLocations"), "Then I tap the Locations row.");

			// Checks for Location permissions
			if (app.IsItThere(x => x.Id("button1"), 10))
			{
				app.WaitForThenTap(x => x.Id("button1"), "Then I tap the Allow button.");
			}

			Thread.Sleep(15000);
			app.WaitForThenEnterText(x => x.Id("searchBar"), "Tampa", "Then I enter Tampa in the Search Bar.");
			app.PressEnter();
			Thread.Sleep(15000);
			Logout();
		}


		[Test]
		public void Repl_Test()
		{
			app.Repl();
		}
	}
}