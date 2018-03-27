using System.Collections.Generic;

namespace SunMobile.Shared.Views
{
    public class TableSection
    {
        public int Index { get; set; }
        public string SectionName { get; set; }
        public int ItemCount { get; set; }
        public List<ListViewItem> ListViewItems { get; set; }
    }
}