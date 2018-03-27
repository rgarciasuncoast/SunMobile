using System.Collections.Generic;

namespace SunMobile.Shared.Views
{
	public class GroupedTextViewTableSource
	{
		public Dictionary<string, TableSection> Sections { get; set; }
		public List<string> SectionTitles { get; set; }
	}
}