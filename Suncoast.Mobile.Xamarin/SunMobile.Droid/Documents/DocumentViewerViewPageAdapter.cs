using System.Collections.Generic;
using Android.Support.V4.App;
using SunBlock.DataTransferObjects.DocumentCenter;

namespace SunMobile.Droid.Documents
{
	public class DocumentViewerViewPageAdapter : FragmentStatePagerAdapter
	{
		private List<Fragment> _fragments { get; set; }
		private List<DocumentCenterFile> _files;

		public DocumentViewerViewPageAdapter(FragmentManager fragmentManager, List<DocumentCenterFile> files) : base(fragmentManager)
		{
			_files = files;

			if (_files == null)
			{
				_files = new List<DocumentCenterFile>();
			}

			_fragments = new List<Fragment>();

			if (_files.Count > 0)
			{
				foreach (var file in _files)
				{
					var documentViewerContentFragment = new DocumentViewerContentFragment();
					documentViewerContentFragment.File = file;
					_fragments.Add(documentViewerContentFragment);
				}
			}
		}

		public override int Count 
		{
			get 
			{
				return _files.Count;
			}
		}

		public override Fragment GetItem(int position)
		{
			return _fragments[position];
		}
	}
}