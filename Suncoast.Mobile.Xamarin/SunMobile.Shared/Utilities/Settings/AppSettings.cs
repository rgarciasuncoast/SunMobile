using SunMobile.Shared.Utilities.General;

namespace SunMobile.Shared.Utilities.Settings
{
	public static class AppSettings
	{
		#if DEBUG
		//public static string SunBlockUrl = @"https://192.168.224.128:444/";
		public static string SunBlockUrl = @"https://sunblock-dev.suncoastcreditunion.com/";		
		public static string GoogleApiProjectId = "825206996652";
		public static string InAuthAccountId = "ba22e3a3-70c9-4354-80ea-63e48e45c96e";
        public static string InAuthApplicationId = null;
		public static string VisualStudioAppCenteriOS = "ae4f9a37-9149-41f1-a341-a6ededbb922e";
        public static string VisualStudioAppCenterAndroid = "71bbbd2c-8fa4-4e4f-82e3-617fb35ff77b";
		#endif

		#if __ADHOC__
		public static string SunBlockUrl = @"https://sunblock-dev.suncoastcreditunion.com/";		
		public static string GoogleApiProjectId = "825206996652"; 
		public static string InAuthAccountId = "ba22e3a3-70c9-4354-80ea-63e48e45c96e";
        public static string InAuthApplicationId = null;
        public static string VisualStudioAppCenteriOS = "ae4f9a37-9149-41f1-a341-a6ededbb922e";
        public static string VisualStudioAppCenterAndroid = "71bbbd2c-8fa4-4e4f-82e3-617fb35ff77b";
		#endif

		#if __APPSTORE__
        /*
		public static string SunBlockUrl = @"https://sunblock.suncoastcreditunion.com/";		
		public static string GoogleApiProjectId = "66399184134"; 
		public static string InAuthAccountId = "5b0522bd-c681-4f4e-8e39-eed566a592c7";
        public static string InAuthApplicationId = null;
        public static string VisualStudioAppCenteriOS = "29e2b832-f63b-42dd-97c0-a057b0ba7ab5";
        public static string VisualStudioAppCenterAndroid = "29e2b832-f63b-42dd-97c0-a057b0ba7ab5r";
		*/

		public static string SunBlockUrl = @"https://sunblock-dev.suncoastcreditunion.com/";		
		public static string GoogleApiProjectId = "825206996652";
		public static string InAuthAccountId = "ba22e3a3-70c9-4354-80ea-63e48e45c96e";
		public static string InAuthApplicationId = null;
        public static string VisualStudioAppCenteriOS = "ae4f9a37-9149-41f1-a341-a6ededbb922e";
        public static string VisualStudioAppCenterAndroid = "71bbbd2c-8fa4-4e4f-82e3-617fb35ff77b";		

		#endif

		public static string SunBlockAnalyzeUrl 
		{ 
			get 
			{
				if (GeneralUtilities.IsPhone())
				{
					#if __IOS__
					return @"Mobile/iOS/iPhone/Analyze.svc/";
					#else
					return @"Mobile/Android/Phone/Analyze.svc/";
					#endif
				}
				else
				{
					#if __IOS__
					return @"Mobile/iOS/iPad/Analyze.svc/";
					#else
					return @"Mobile/Android/Tablet/Analyze.svc/";
					#endif
				}				
			}
		}

		public static string SunBlockServiceUrl 
		{ 
			get 
			{
				if (GeneralUtilities.IsPhone())
				{
					#if __IOS__
					return @"Mobile/iOS/iPhone/Service.svc/";
					#else
					return @"Mobile/Android/Phone/Service.svc/";
					#endif
				}
				else
				{
					#if __IOS__
					return @"Mobile/iOS/iPad/Service.svc/";
					#else
					return @"Mobile/Android/Tablet/Service.svc/";
					#endif
				}				
			}
		}        	

		public static string SunBlockRemoteDepositsServiceUrl = @"RemoteDeposits/RemoteDepositsService.svc/";
		public static string SunBlockBillPayServiceUrl = @"BillPay/V2/BillPayService.svc/";

		public static string GoogleMapsApiKey = "02_hGpmJP2ag2AVZSb-gPsAe0NVENgISg9FlJTQ";
	}
}