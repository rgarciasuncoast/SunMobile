using System.Collections.Generic;

namespace SunMobile.Shared.States
{
	public static class USStates
	{
		public static Dictionary<string, string> USStateList { get; set; }

		static USStates()
		{
			USStateList = new Dictionary<string, string>();

			USStateList.Add("AL", "Alabama");
			USStateList.Add("AK", "Alaska");
			USStateList.Add("AZ", "Arizona");
			USStateList.Add("AR", "Arkansas");
			USStateList.Add("CA", "California");
			USStateList.Add("CO", "Colorado");
			USStateList.Add("CT", "Connecticut");
			USStateList.Add("DE", "Delaware");
			USStateList.Add("DC", "District Of Columbia");
			USStateList.Add("FL", "Florida");
			USStateList.Add("GA", "Georgia");
			USStateList.Add("HI", "Hawaii");
			USStateList.Add("ID", "Idaho");
			USStateList.Add("IL", "Illinois");
			USStateList.Add("IN", "Indiana");
			USStateList.Add("IA", "Iowa");
			USStateList.Add("KS", "Kansas");
			USStateList.Add("KY", "Kentucky");
			USStateList.Add("LA", "Louisiana");
			USStateList.Add("ME", "Maine");
			USStateList.Add("MD", "Maryland");
			USStateList.Add("MA", "Massachusetts");
			USStateList.Add("MI", "Michigan");
			USStateList.Add("MN", "Minnesota");
			USStateList.Add("MS", "Mississippi");
			USStateList.Add("MO", "Missouri");
			USStateList.Add("MT", "Montana");
			USStateList.Add("NE", "Nebraska");
			USStateList.Add("NV", "Nevada");
			USStateList.Add("NH", "New Hampshire");
			USStateList.Add("NJ", "New Jersey");
			USStateList.Add("NM", "New Mexico");
			USStateList.Add("NY", "New York");
			USStateList.Add("NC", "North Carolina");
			USStateList.Add("ND", "North Dakota");
			USStateList.Add("OH", "Ohio");
			USStateList.Add("OK", "Oklahoma");
			USStateList.Add("OR", "Oregon");
			USStateList.Add("PA", "Pennsylvania");
			USStateList.Add("RI", "Rhode Island");
			USStateList.Add("SC", "South Carolina");
			USStateList.Add("SD", "South Dakota");
			USStateList.Add("TN", "Tennessee");
			USStateList.Add("TX", "Texas");
			USStateList.Add("UT", "Utah");
			USStateList.Add("VT", "Vermont");
			USStateList.Add("VA", "Virginia");
			USStateList.Add("WA", "Washington");
			USStateList.Add("WV", "West Virginia");
			USStateList.Add("WI", "Wisconsin");
			USStateList.Add("WY", "Wyoming");
		}
	}
}