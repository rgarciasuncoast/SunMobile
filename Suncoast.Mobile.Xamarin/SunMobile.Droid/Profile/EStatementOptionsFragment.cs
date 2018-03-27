using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Droid.Documents;
using SunMobile.Shared;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.Profile
{
	public class DocumentOptionsFragment : BaseFragment
	{
		private Switch switchAccountEStatementEnrollment;
		private Switch switchENoticeEnrollment;
        private Switch switchENoticeAndEStatementEnrollment;
        private TableRow rowAccountEStatementEnrollment;
        private TableRow rowENoticeEnrollment;
        private TableRow rowENoticeAndEStatementEnrollment;
        private View seperatorENoticeEnrollment;
        private View seperatorENoticeAndEStatementEnrollment;
		private TableRow rowStatementDisclosure;
		private bool _didGetEnrollment;
        private StatusResponse<bool> _ENoticeResponse;
        private StatusResponse<bool> _AccountEStatementResponse;
        private bool seperateRowsCollapsed;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.EDocumentsSettingsView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			outState.PutBoolean("DidGetEnrollment", _didGetEnrollment);
			outState.PutBoolean("switchAccountEStatementEnrollment", switchENoticeEnrollment.Checked);
			outState.PutBoolean("switchENoticeEnrollment", switchENoticeEnrollment.Checked);
            outState.PutBoolean("switchENoticeAndEStatementEnrollment", switchENoticeAndEStatementEnrollment.Checked);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			if (savedInstanceState != null)
			{
				_didGetEnrollment = savedInstanceState.GetBoolean("DidGetEnrollment");
				switchAccountEStatementEnrollment.Checked = savedInstanceState.GetBoolean("switchAccountEStatementEnrollment");
				switchENoticeEnrollment.Checked = savedInstanceState.GetBoolean("switchENoticeEnrollment");
                switchENoticeAndEStatementEnrollment.Checked = savedInstanceState.GetBoolean("switchENoticeAndEStatementEnrollment");
			}
            else
            {
                _didGetEnrollment = false;
                seperateRowsCollapsed = false;
            }

			SetupView();
		}

		public override void SetupView()
		{
			base.SetupView();

			((MainActivity)Activity).SetActionBarTitle("Electronic Document Options");

			switchAccountEStatementEnrollment = Activity.FindViewById<Switch>(Resource.Id.switchAccountEStatements);
            switchAccountEStatementEnrollment.CheckedChange += SwitchAccountEStatementEnrollment_CheckedChange;
			switchENoticeEnrollment = Activity.FindViewById<Switch>(Resource.Id.switchReceiveENoticesOnline);
            switchENoticeEnrollment.CheckedChange += SwitchENoticeEnrollment_CheckedChange;
            switchENoticeAndEStatementEnrollment = Activity.FindViewById<Switch>(Resource.Id.switchReceiveENoticesAndEStatementsOnline);
            switchENoticeAndEStatementEnrollment.CheckedChange += SwitchENoticeAndEStatementEnrollment_CheckedChange;
            rowStatementDisclosure = Activity.FindViewById<TableRow>(Resource.Id.rowStatementDisclosure);
			rowStatementDisclosure.Click += (sender, e) => { ViewDisclosure(); };

            rowAccountEStatementEnrollment = Activity.FindViewById<TableRow>(Resource.Id.rowAccountEStatementsToggle);
            rowENoticeEnrollment = Activity.FindViewById<TableRow>(Resource.Id.rowReceiveENoticesOnline);
            rowENoticeAndEStatementEnrollment = Activity.FindViewById<TableRow>(Resource.Id.rowReceiveENoticesAndEStatementsOnline);
            seperatorENoticeEnrollment = Activity.FindViewById<View>(Resource.Id.seperatorENoticesAndEStatements);
            seperatorENoticeAndEStatementEnrollment = Activity.FindViewById<View>(Resource.Id.seperatorENoticesAndEStatements);

            CollapseAllRows();

			GetEnrollment();
		}

		private async void GetEnrollment()
		{
			if (!_didGetEnrollment)
			{
				var methods = new DocumentMethods();
				var request = new EDocumentIsEnrolledRequest { DocumentType = EDocumentTypes.AccountEStatements.ToString() };

				ShowActivityIndicator();

                _AccountEStatementResponse = await methods.IsEDocumentEnrolled(request, View);
                request = new EDocumentIsEnrolledRequest { DocumentType = EDocumentTypes.ENotices.ToString() };
                _ENoticeResponse = await methods.IsEDocumentEnrolled(request, View);

                if (_AccountEStatementResponse != null && _ENoticeResponse != null && _AccountEStatementResponse.Success && _ENoticeResponse.Success)
                {
                    if (_AccountEStatementResponse.Result == _ENoticeResponse.Result)
                    {
                        CollapseSeperateRows(null);
                        switchENoticeAndEStatementEnrollment.CheckedChange -= SwitchENoticeAndEStatementEnrollment_CheckedChange;
                        switchENoticeAndEStatementEnrollment.Checked = _AccountEStatementResponse.Result;
                        switchENoticeAndEStatementEnrollment.CheckedChange += SwitchENoticeAndEStatementEnrollment_CheckedChange;
                    }
                    else
                    {
                        CollapseCombinedRow();

                        switchAccountEStatementEnrollment.CheckedChange -= SwitchAccountEStatementEnrollment_CheckedChange;
                        switchAccountEStatementEnrollment.Checked = _AccountEStatementResponse.Result;
                        switchAccountEStatementEnrollment.CheckedChange += SwitchAccountEStatementEnrollment_CheckedChange;

                        switchENoticeEnrollment.CheckedChange -= SwitchENoticeEnrollment_CheckedChange;
                        switchENoticeEnrollment.Checked = _ENoticeResponse.Result;
                        switchENoticeEnrollment.CheckedChange += SwitchENoticeEnrollment_CheckedChange;
                    }
                }

				HideActivityIndicator();

				_didGetEnrollment = true;
			}
		}

        private void CollapseAllRows()
        {
            rowAccountEStatementEnrollment.Visibility = ViewStates.Gone;
            rowENoticeEnrollment.Visibility = ViewStates.Gone;
            rowENoticeAndEStatementEnrollment.Visibility = ViewStates.Gone;
            seperatorENoticeEnrollment.Visibility = ViewStates.Gone;
            seperatorENoticeAndEStatementEnrollment.Visibility = ViewStates.Gone;
            rowStatementDisclosure.Visibility = ViewStates.Gone;
        }

        private void CollapseSeperateRows(object sender)
        {
            if (!seperateRowsCollapsed)
            {
                rowAccountEStatementEnrollment.Visibility = ViewStates.Gone;
                rowENoticeEnrollment.Visibility = ViewStates.Gone;
                rowENoticeAndEStatementEnrollment.Visibility = ViewStates.Visible;
                seperatorENoticeEnrollment.Visibility = ViewStates.Gone;
                seperatorENoticeAndEStatementEnrollment.Visibility = ViewStates.Gone;
                seperateRowsCollapsed = true;
                rowStatementDisclosure.Visibility = ViewStates.Visible;

                if(sender != null)
                {
                    switchENoticeAndEStatementEnrollment.CheckedChange -= SwitchENoticeAndEStatementEnrollment_CheckedChange;
                    switchENoticeAndEStatementEnrollment.Checked = ((Switch)sender).Checked;
                    switchENoticeAndEStatementEnrollment.CheckedChange += SwitchENoticeAndEStatementEnrollment_CheckedChange;
                }
            }
        }

        private void CollapseCombinedRow()
        {
            rowAccountEStatementEnrollment.Visibility = ViewStates.Visible;
            rowENoticeEnrollment.Visibility = ViewStates.Visible;
            rowENoticeAndEStatementEnrollment.Visibility = ViewStates.Gone;
            seperatorENoticeEnrollment.Visibility = ViewStates.Visible;
            seperatorENoticeAndEStatementEnrollment.Visibility = ViewStates.Gone;
            rowStatementDisclosure.Visibility = ViewStates.Visible;
        }

        void SwitchAccountEStatementEnrollment_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SetEnrollment(EDocumentTypes.AccountEStatements, sender);
            CollapseSeperateRows(sender);
        }

        void SwitchENoticeEnrollment_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SetEnrollment(EDocumentTypes.ENotices, sender);
            CollapseSeperateRows(sender);
        }

        void SwitchENoticeAndEStatementEnrollment_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SetEnrollment(EDocumentTypes.AccountEStatements, sender);
            SetEnrollment(EDocumentTypes.ENotices, sender);
        }

        private async void SetEnrollment(EDocumentTypes docType, object sender)
		{
			var request = new EDocumentEnrollmentRequest();

			switch (docType)
			{
				case EDocumentTypes.ENotices:
					request.DocumentType = EDocumentTypes.ENotices.ToString();
                    request.EnrollmentFlag = ((Switch)sender).Checked;
					break;
				default:
					request.DocumentType = EDocumentTypes.AccountEStatements.ToString();
                    request.EnrollmentFlag = ((Switch)sender).Checked;
					break;
			}

			var methods = new DocumentMethods();
			var response = await methods.SetEDocumentEnrollment(request, Activity);

			if (response == null || !response.Success)
			{
				await AlertMethods.Alert(Activity, "SunMobile", "Unable to update enrollment.", "OK");
			}
		}

		private void ViewDisclosure()
		{
			var documentViewerViewPagerFragment = new DocumentViewerViewPagerFragment();
			documentViewerViewPagerFragment.Files = new List<DocumentCenterFile> { new DocumentCenterFile { URL = "https://sunblockstorage.blob.core.windows.net/documents/eStatement-Disclosure.pdf" } };
			NavigationService.NavigatePush(documentViewerViewPagerFragment, true, false, true);
		}
	}
}