using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.OnBase;
using SunMobile.Droid.Documents;
using SunMobile.Shared;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Views;

namespace SunMobile.Droid
{
	public class EStatementsFragment : BaseListFragment
	{
		public string Header { get; set; }
		public EDocumentTypes DocumentType { get; set; }
		private SwipeRefreshLayout refresher;
		private EDocumentListAdapter _documentsListAdapter;		
		private List<ImageDocument> _viewModel;
		private bool _enrolled;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.DocumentsListView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			base.SetupView();

			if (!string.IsNullOrEmpty(Header))
			{
				((MainActivity)Activity).SetActionBarTitle(Header);
			}
			else
			{
				((MainActivity)Activity).SetActionBarTitle("EStatements");
			}

			refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.documentsRefresher);
			refresher.SetColorSchemeResources(Resource.Color.material_blue_grey_800);
			refresher.Refresh += (sender, e) => { LoadDocuments(); };

			ListViewMain.ItemClick += (sender, e) =>
			{
                var document = new ImageDocument();

				switch (DocumentType)
				{					
					default:
						{
							document = _documentsListAdapter.GetListViewItem(e.Position).Data as ImageDocument;

							//load the pdf viewer for the document
							if (null != document && !string.IsNullOrEmpty(document.DocumentName))
							{
								var documentViewerViewPagerFragment = new DocumentViewerViewPagerFragment();
                                var documentCenterFile = new DocumentCenterFile { FileId = document.DocumentId.ToString(), OnBaseImageDocumentType = DocumentType.ToString(), MimeType = "application/pdf" };

                                if (document.Images != null && document.Images.Count > 0 && document.Images[0].ImageStream != null)
                                {
                                    documentCenterFile.FileBytes = document.Images[0].ImageStream;
                                }

                                documentViewerViewPagerFragment.Files = new List<DocumentCenterFile> { documentCenterFile };
								NavigationService.NavigatePush(documentViewerViewPagerFragment, true, false, true);
							}
							break;
						}
				}
			};

			LoadDocuments();
		}

		private async void LoadDocuments()
		{
			try
			{
				if (refresher.Refreshing)
				{
					_viewModel = null;
				}

				if (_viewModel == null)
				{
					if (!refresher.Refreshing)
					{
						if (!string.IsNullOrEmpty(Header))
						{
							ShowActivityIndicator("Loading " + Header + "...");
						}
						else
						{
							ShowActivityIndicator("Loading EStatements...");
						}
					}

					var methods = new DocumentMethods();
					var request = new EDocumentRequest();
					request.DocumentType = DocumentType.ToString();
					request.BeginTimeQuery = DateTime.UtcNow.AddYears(-2);
					request.EndTimeQuery = DateTime.UtcNow;

					switch (DocumentType)
					{
						case EDocumentTypes.AccountEStatements:
							_enrolled = await GetEnrollment(EDocumentTypes.AccountEStatements);
							break;
						case EDocumentTypes.ENotices:
							_enrolled = await GetEnrollment(EDocumentTypes.ENotices);
							break;
						default:
							_enrolled = true;
							break;
					}

					if (!_enrolled)
					{
						var documentTypeMessage = (DocumentType == EDocumentTypes.AccountEStatements) ? "eStatements" : "eNotices";
						var alertMessage = "You are not currently enrolled in " + documentTypeMessage + ". Would you like to enroll?";
						var alertResponseOptions = new string[] { "Accept", "Decline" };
						var alertResponse = await AlertMethods.Alert(Activity, documentTypeMessage + " Enrollment", alertMessage, alertResponseOptions);

						if (alertResponse == "Accept")
						{
							switch (DocumentType)
							{
								case EDocumentTypes.AccountEStatements:
									SetEnrollment(EDocumentTypes.AccountEStatements);
									_enrolled = true;
									break;
								case EDocumentTypes.ENotices:
									SetEnrollment(EDocumentTypes.ENotices);
									_enrolled = true;
									break;
							}
						}

						else if (alertResponse == "Decline")
						{
							NavigationService.NavigatePop();
						}
					}

					if (_enrolled)
					{
						var response = await methods.GetEDocuments(request, Activity);

						if (response != null && response.Success)
						{
							_viewModel = response.Result;
						}
					}

					if (!refresher.Refreshing)
					{
						HideActivityIndicator();
					}
					else
					{
						refresher.Refreshing = false;
					}
				}

				if (_viewModel == null)
				{
					_viewModel = new List<ImageDocument>();
				}

				if (_viewModel != null)
				{
					var listViews = ViewUtilities.ConvertListImageDocumentToListViews(_viewModel);
					_documentsListAdapter = new EDocumentListAdapter(Activity, listViews, _enrolled);
					ListAdapter = _documentsListAdapter;					

					switch (DocumentType)
					{
						case EDocumentTypes.AccountEStatements:
							Logging.Track("Starting EStatements.");
							break;
						case EDocumentTypes.ENotices:
							Logging.Track("Starting ENotices.");
							break;
						case EDocumentTypes.TaxDocuments:
							Logging.Track("Starting Tax Documents.");
							break;
                        case EDocumentTypes.CreditCardAnnualEStatements:
                            Logging.Track("Starting Credit Card EStatements");
                            break;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "EStatementsFragment:LoadDocuments");
			}
		}

		private async Task<bool> GetEnrollment(EDocumentTypes docType)
		{
			var request = new EDocumentIsEnrolledRequest();

			switch (docType)
			{
				case EDocumentTypes.ENotices:
					request.DocumentType = EDocumentTypes.ENotices.ToString();
					break;
				default:
					request.DocumentType = EDocumentTypes.AccountEStatements.ToString();
					break;
			}

			var methods = new DocumentMethods();
			var response = await methods.IsEDocumentEnrolled(request, Activity);

			return response.Result;
		}

		private async void SetEnrollment(EDocumentTypes docType)
		{
			var request = new EDocumentEnrollmentRequest();

			switch (docType)
			{
				case EDocumentTypes.ENotices:
					request.DocumentType = EDocumentTypes.ENotices.ToString();
					request.EnrollmentFlag = true;
					break;
				default:
					request.DocumentType = EDocumentTypes.AccountEStatements.ToString();
					request.EnrollmentFlag = true;
					break;
			}

			var methods = new DocumentMethods();
			var response = await methods.SetEDocumentEnrollment(request, Activity);

			if (response == null || !response.Success)
			{
				await AlertMethods.Alert(Activity, "SunMobile", "Unable to update enrollment.", "OK");
			}
		}
	}
}