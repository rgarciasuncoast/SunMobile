using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.OnBase;
using SunMobile.Droid.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.States;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Utilities.General;

namespace SunMobile.Droid.Accounts
{
	public class TransactionDisputeFragment : BaseFragment
	{
		public Transaction SelectedTransaction { get; set; }
		public string SelectedMemberId { get; set; }
		public string SelectedAccountDescription { get; set; }
		public string SelectedSuffix { get; set; }

        private TextView txtTitle;
		private TextView lblSection1Title;
        private TextView lblSection2Title;
        private TextView lblSection3Title;
        private TextView lblSection4Title;
		private TextView lblCardHolderName;
		private EditText txtDayTimePhone;
		private EditText txtAddressLine1;
		private EditText txtAddressLine2;
		private EditText txtCity;
		private Spinner spinnerState;
		private EditText txtZipCode;
		private TextView txtTransactionDate;
		private TextView txtTransactionDescription;
		private TextView txtTransactionAmount;
		private Spinner spinnerReason;
		private ImageButton imgBack;
		private TextView txtContinue;
		private TableLayout tableDocumentsHeader;
		private TableLayout tableDocuments;
		private TableLayout tableTransactionsHeader;
		private TableLayout tableTransactions;
		private TableLayout tableQuestions;
		private TableRow rowCardHolderName;
		private TableRow rowAddress1;
		private TableRow rowAddress2;
		private TableRow rowCity;
		private TableRow rowState;
		private TableRow rowZipCode;
		private TableRow rowReason;
		private TableRow rowReasonSpinner;
		private Button btnUploadDocuments;
		private Button btnAddTransactions;
        private TextView lblCardholderLabel;
        private TextView lblPhoneLabel;
        private TextView lblAddress1Label;
        private TextView lblAddress2Label;
        private TextView lblCityLabel;
        private TextView lblStateLabel;
        private TextView lblZipCodeLabel;
        private TextView lblDescriptionLabel;
        private TextView lblDateLabel;
        private TextView lblAmountLabel;
        private TextView lblReasonLabel;

		private MobileStatusResponse<TransactionDisputeInformationResponse> _transactionDisputeInfoResponse;
		private TransactionDisputeInfo _transactionDisputeInfo;
		private MemberInformation _memberResponse;
		private StatusResponse<List<BankCard>> _cardResponse;
		private List<TransactionDisputeQuestion> _questionList;
		private List<TransactionDisputeAnswer> _answerList;
		private List<FileInformation> _documentsList;
		private List<Transaction> _additionalTransactionsList;
		private List<string> _reasonsList;
		private bool _inReview;
		private bool _isCreditCard;
		private string _disputeType;
		private bool _canSelectMultipleTransactions;
		private const int QUESTION_WIDTH = 140;
		private string SELECT_A_REASON = "Select";
		private string SELECT_AN_OPTION = "Select";
		private const int MAX_TRANSACTIONS = 19;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.TransactionDisputeView, null);

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			string json = JsonConvert.SerializeObject(SelectedTransaction);
			outState.PutString("Transaction", json);
			json = JsonConvert.SerializeObject(_transactionDisputeInfoResponse);
			outState.PutString("TransactionDisputeInfoResponse", json);
			json = JsonConvert.SerializeObject(_transactionDisputeInfo);
			outState.PutString("TransactionDisputeInfo", json);
			json = JsonConvert.SerializeObject(_questionList);
			outState.PutString("Questions", json);
			json = JsonConvert.SerializeObject(_answerList);
			outState.PutString("Answers", json);
			json = JsonConvert.SerializeObject(_reasonsList);
			outState.PutString("ReasonsList", json);
			json = JsonConvert.SerializeObject(_memberResponse);
			outState.PutString("MemberResponse", json);
			json = JsonConvert.SerializeObject(_cardResponse);
			outState.PutString("CardResponse", json);
			json = JsonConvert.SerializeObject(_additionalTransactionsList);
			outState.PutString("AdditionalTransactionsList", json);
			json = JsonConvert.SerializeObject(_documentsList);
			outState.PutString("DocumentsList", json);
			outState.PutString("MemberId", SelectedMemberId);
			outState.PutString("Suffix", SelectedSuffix);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			if (savedInstanceState != null)
			{
				string json = savedInstanceState.GetString("Transaction");
				SelectedTransaction = JsonConvert.DeserializeObject<Transaction>(json);
				json = savedInstanceState.GetString("TransactionDisputeInfoResponse");
				_transactionDisputeInfoResponse = JsonConvert.DeserializeObject<MobileStatusResponse<TransactionDisputeInformationResponse>>(json);
				json = savedInstanceState.GetString("TransactionDisputeInfo");
				_transactionDisputeInfo = JsonConvert.DeserializeObject<TransactionDisputeInfo>(json);
				json = savedInstanceState.GetString("Questions");
				_questionList = JsonConvert.DeserializeObject<List<TransactionDisputeQuestion>>(json);
				json = savedInstanceState.GetString("Answers");
				_answerList = JsonConvert.DeserializeObject<List<TransactionDisputeAnswer>>(json);
				json = savedInstanceState.GetString("ReasonsList");
				_reasonsList = JsonConvert.DeserializeObject<List<string>>(json);
				json = savedInstanceState.GetString("MemberResponse");
				_memberResponse = JsonConvert.DeserializeObject<MemberInformation>(json);
				json = savedInstanceState.GetString("CardResponse");
				_cardResponse = JsonConvert.DeserializeObject<StatusResponse<List<BankCard>>>(json);
				json = savedInstanceState.GetString("AdditionalTransactionsList");
				_additionalTransactionsList = JsonConvert.DeserializeObject<List<Transaction>>(json);
				json = savedInstanceState.GetString("DocumentsList");
				_documentsList = JsonConvert.DeserializeObject<List<FileInformation>>(json);
				SelectedMemberId = savedInstanceState.GetString("MemberId");
				SelectedSuffix = savedInstanceState.GetString("Suffix");
			}

			SetupView();
		}

		public override void SetupView()
		{
			try
			{
				base.SetupView();

				((MainActivity)Activity).SetActionBarTitle("Transaction Dispute");

				if (_documentsList == null)
				{
					_documentsList = new List<FileInformation>();
				}

				if (_additionalTransactionsList == null)
				{
					_additionalTransactionsList = new List<Transaction>();
				}

				_isCreditCard = SelectedSuffix.Length > 5;

                txtTitle = Activity.FindViewById<TextView>(Resource.Id.txtTitle);
				lblSection1Title = Activity.FindViewById<TextView>(Resource.Id.lblSection1Title);
                lblSection2Title = Activity.FindViewById<TextView>(Resource.Id.lblSection2Title);
                lblSection3Title = Activity.FindViewById<TextView>(Resource.Id.lblSection3Title);
                lblSection4Title = Activity.FindViewById<TextView>(Resource.Id.lblSection4Title);
				lblCardHolderName = Activity.FindViewById<TextView>(Resource.Id.lblCardholdersName);
				txtDayTimePhone = Activity.FindViewById<EditText>(Resource.Id.txtDaytimePhone);
				txtAddressLine1 = Activity.FindViewById<EditText>(Resource.Id.txtAddressLine1);
				txtAddressLine2 = Activity.FindViewById<EditText>(Resource.Id.txtAddressLine2);
				txtCity = Activity.FindViewById<EditText>(Resource.Id.txtCity);
				spinnerState = Activity.FindViewById<Spinner>(Resource.Id.spinnerState);
				txtZipCode = Activity.FindViewById<EditText>(Resource.Id.txtZipCode);
				txtTransactionDate = Activity.FindViewById<TextView>(Resource.Id.txtTransactionDate);
				txtTransactionDescription = Activity.FindViewById<TextView>(Resource.Id.txtTransactionDescription);
				txtTransactionAmount = Activity.FindViewById<TextView>(Resource.Id.txtTransactionAmount);
				spinnerReason = Activity.FindViewById<Spinner>(Resource.Id.spinnerReason);
				txtContinue = Activity.FindViewById<TextView>(Resource.Id.txtContinueDispute);
				imgBack = Activity.FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
				tableDocumentsHeader = Activity.FindViewById<TableLayout>(Resource.Id.tableDocumentsHeader);
				tableDocuments = Activity.FindViewById<TableLayout>(Resource.Id.tableDocuments);
				tableTransactionsHeader = Activity.FindViewById<TableLayout>(Resource.Id.tableAdditionalTransactionsHeader);
				tableTransactions = Activity.FindViewById<TableLayout>(Resource.Id.tableAdditionalTransactions);

				tableQuestions = Activity.FindViewById<TableLayout>(Resource.Id.tableReasonQuestions);
				rowCardHolderName = Activity.FindViewById<TableRow>(Resource.Id.cardHoldersNameRow);
				rowAddress1 = Activity.FindViewById<TableRow>(Resource.Id.addressRow1);
				rowAddress2 = Activity.FindViewById<TableRow>(Resource.Id.addressRow2);
				rowCity = Activity.FindViewById<TableRow>(Resource.Id.cityRow);
				rowState = Activity.FindViewById<TableRow>(Resource.Id.stateRow);
				rowZipCode = Activity.FindViewById<TableRow>(Resource.Id.zipCodeRow);
				rowReason = Activity.FindViewById<TableRow>(Resource.Id.reasonRow);
				rowReasonSpinner = Activity.FindViewById<TableRow>(Resource.Id.reasonSpinnerRow);
				btnUploadDocuments = Activity.FindViewById<Button>(Resource.Id.btnUploadDocuments);
				btnAddTransactions = Activity.FindViewById<Button>(Resource.Id.btnAddTransactions);

                lblCardholderLabel = Activity.FindViewById<TextView>(Resource.Id.lblCardholderLabel);
                lblPhoneLabel = Activity.FindViewById<TextView>(Resource.Id.lblPhoneLabel);
                lblAddress1Label = Activity.FindViewById<TextView>(Resource.Id.lblAddress1Label);
                lblAddress2Label = Activity.FindViewById<TextView>(Resource.Id.lblAddress2Label);
                lblCityLabel = Activity.FindViewById<TextView>(Resource.Id.lblCityLabel);
                lblStateLabel = Activity.FindViewById<TextView>(Resource.Id.lblStateLabel);
                lblZipCodeLabel = Activity.FindViewById<TextView>(Resource.Id.lblZipCodeLabel);
                lblDescriptionLabel = Activity.FindViewById<TextView>(Resource.Id.lblDescriptionLabel);
                lblDateLabel = Activity.FindViewById<TextView>(Resource.Id.lblDateLabel);
                lblAmountLabel = Activity.FindViewById<TextView>(Resource.Id.lblAmountLabel);
                lblReasonLabel = Activity.FindViewById<TextView>(Resource.Id.lblReasonLabel);

				var stateAdapter = new ArrayAdapter(Activity, Resource.Layout.support_simple_spinner_dropdown_item, USStates.USStateList.Keys.ToList());
				spinnerState.Adapter = stateAdapter;

				txtTransactionDate.Text = string.Format("{0:MM/dd/yyyy}", SelectedTransaction.TransactionDate.UtcToEastern());
				txtTransactionDescription.Text = SelectedTransaction.Description;
				txtTransactionAmount.Text = StringUtilities.FormatAsCurrency(SelectedTransaction.TransactionAmount.ToString());

				if (SelectedTransaction.TransactionAmount < 0)
				{
					txtTransactionAmount.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.TextViewTextColorRed)));
				}

				imgBack.Click += (sender, e) =>
				{
					NavigationService.NavigatePop(false);
				};

				var methods = new AccountMethods();
				_disputeType = methods.GetDisputeInfo(SelectedTransaction, SelectedMemberId, SelectedSuffix.Length > 5).DisputeType;

				spinnerReason.ItemSelected += (sender, e) =>
				{
					if (_disputeType == "ach")
					{
                        lblSection1Title.Text = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "FC6FB253-3453-4721-8A2A-E1F355F936B6", "Contact Information");
						_transactionDisputeInfo = _transactionDisputeInfoResponse.Result.DisputeInfo.Find(x => x.DisputeReason == "ACH");
						rowCardHolderName.Visibility = ViewStates.Gone;
						rowAddress1.Visibility = ViewStates.Gone;
						rowAddress2.Visibility = ViewStates.Gone;
						rowCity.Visibility = ViewStates.Gone;
						rowState.Visibility = ViewStates.Gone;
						rowZipCode.Visibility = ViewStates.Gone;
						rowReason.Visibility = ViewStates.Gone;
						rowReasonSpinner.Visibility = ViewStates.Gone;
					}
					else if (_disputeType == "atm")
					{
						_transactionDisputeInfo = _transactionDisputeInfoResponse.Result.DisputeInfo.Find(x => x.DisputeReason == "ATM");
						rowReason.Visibility = ViewStates.Gone;
						rowReasonSpinner.Visibility = ViewStates.Gone;
					}
					else
					{
						if (spinnerReason.SelectedItemPosition == 0)
						{
							_questionList = null;
							_transactionDisputeInfo = null;
							tableDocumentsHeader.Visibility = ViewStates.Gone;
							tableDocuments.Visibility = ViewStates.Gone;
							tableTransactionsHeader.Visibility = ViewStates.Gone;
							tableTransactions.Visibility = ViewStates.Gone;
						}
						else
						{
							var reasonPositionAdjusted = -1;
							for (int i = 0; i < _transactionDisputeInfoResponse.Result.DisputeInfo.Count; i++)
							{
								if (_reasonsList[spinnerReason.SelectedItemPosition] == _transactionDisputeInfoResponse.Result.DisputeInfo[i].DisputeReason)
								{
									reasonPositionAdjusted = i;
								}
							}

							_transactionDisputeInfo = (reasonPositionAdjusted != -1) ? _transactionDisputeInfoResponse.Result.DisputeInfo[reasonPositionAdjusted] : _transactionDisputeInfoResponse.Result.DisputeInfo[spinnerReason.SelectedItemPosition - 1];
						}
					}

					if (_transactionDisputeInfo != null)
					{
						_questionList = _transactionDisputeInfo.DisputeQuestions;

						if (_transactionDisputeInfo.CanSubmitDocuments)
						{
							tableDocumentsHeader.Visibility = ViewStates.Visible;
							tableDocuments.Visibility = ViewStates.Visible;
						}
						else
						{
							tableDocumentsHeader.Visibility = ViewStates.Gone;
							tableDocuments.Visibility = ViewStates.Gone;
						}

						if (_canSelectMultipleTransactions)
						{
							tableTransactionsHeader.Visibility = ViewStates.Visible;
							tableTransactions.Visibility = ViewStates.Visible;
						}
						else
						{
							tableTransactionsHeader.Visibility = ViewStates.Gone;
							tableTransactions.Visibility = ViewStates.Gone;
						}

						if (!_transactionDisputeInfo.CanSubmitDocuments)
						{
							_documentsList = new List<FileInformation>();
						}

						if (!_canSelectMultipleTransactions)
						{
							_additionalTransactionsList = new List<Transaction>();
						}
					}
					else
					{
						tableDocumentsHeader.Visibility = ViewStates.Gone;
						tableDocuments.Visibility = ViewStates.Gone;
					}

					ShowQuestions();
					ShowHideQuestions();
				};

				btnUploadDocuments.Click += (sender, e) =>
				{
					SelectDocuments();
				};

				btnAddTransactions.Click += (sender, e) =>
				{
					AddTransactions();
				};

				txtContinue.Click += (sender, e) =>
				{
					ReviewDispute();
				};

				GetReasonsAndCardHolderInformation();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionDisputeFragment:SetupView");
			}
		}

		public override void SetCultureConfiguration()
		{
            var title = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "8BD25D4A-6B7B-4545-B288-B6C67586F137", "Transaction Dispute");
            ((MainActivity)Activity).SetActionBarTitle(title);
            txtTitle.Text = title;
            CultureTextProvider.SetMobileResourceText(lblCardholderLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "C12FBC08-F395-433B-9E02-92B131E06707");
			CultureTextProvider.SetMobileResourceText(lblPhoneLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0D4022CE-894B-483E-B024-641CD5E44861");
			CultureTextProvider.SetMobileResourceText(lblAddress1Label, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "A4DE4858-AF9F-4171-88DE-E627E366EBCA");
			CultureTextProvider.SetMobileResourceText(lblAddress2Label, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "38D74819-CF0B-4859-8B4A-B73AB98339B5");
			CultureTextProvider.SetMobileResourceText(lblCityLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "6F3D40B5-FCEB-49E0-864F-F61D9C2F7DC6");
			CultureTextProvider.SetMobileResourceText(lblStateLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "3EB5BA39-31D3-45FD-98ED-221F9D4A5F36");
			CultureTextProvider.SetMobileResourceText(lblZipCodeLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "68B60EDA-C284-4EFA-84A4-42923A147735");
            CultureTextProvider.SetMobileResourceText(lblDescriptionLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "E7804EBE-3832-4679-8F94-4E93D5DFF6B4");
            CultureTextProvider.SetMobileResourceText(lblDateLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "4A3232E6-4B53-4756-94C3-AF58BAD475CC");
			CultureTextProvider.SetMobileResourceText(lblAmountLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "F683B4B2-07E4-431F-AD8A-2590F8A70937");
            CultureTextProvider.SetMobileResourceText(lblReasonLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0B1A176A-53A3-4516-BDD9-55FADCF29BB8");
			CultureTextProvider.SetMobileResourceText(btnUploadDocuments, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "C1E99194-9061-4F57-8D02-BE1D6CACF341");
			CultureTextProvider.SetMobileResourceText(btnAddTransactions, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "704F5538-1FEE-494D-A9FF-E6A709D1AB61");
            CultureTextProvider.SetMobileResourceText(lblSection1Title, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "E3BF7B71-37DD-41CD-AA73-26ADADE8FB52", "Cardholder Information");
            CultureTextProvider.SetMobileResourceText(lblSection2Title, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "66F253B5-5E4A-414D-8D5D-F141EDF6E3AE", "Transaction Information");
            CultureTextProvider.SetMobileResourceText(lblSection3Title, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "D8FF7869-BDAF-489B-82ED-33C5A5DAE917", "Supporting Documents");
            CultureTextProvider.SetMobileResourceText(lblSection4Title, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "66F253B5-5E4A-414D-8D5D-F141EDF6E3AE", "Additional Transactions");
			SELECT_A_REASON = CultureTextProvider.SELECT();
			SELECT_AN_OPTION = CultureTextProvider.SELECT();
            txtAddressLine2.Hint = CultureTextProvider.OPTIONAL();
            txtContinue.Text = CultureTextProvider.CONTINUE();
		}

		private async void GetReasonsAndCardHolderInformation()
		{
			try
			{
				var accountMethods = new AccountMethods();
				var memberRequest = new MemberInformationRequest { MemberId = int.Parse(SelectedMemberId) };
				var cardMethods = new CardMethods();
				var cardListRequest = new CardListRequest { ExcludeAtmCards = false };

				if (_transactionDisputeInfoResponse == null)
				{
					ShowActivityIndicator();

					_transactionDisputeInfoResponse = await accountMethods.GetTransactionDisputeInformation(null, Activity);
					_memberResponse = await accountMethods.GetMemberInformation(memberRequest, Activity);
					_cardResponse = await cardMethods.CardList(cardListRequest, Activity);

					HideActivityIndicator();

					_reasonsList = new List<string>();

					// Add all posible dispute reasons.
					foreach (TransactionDisputeInfo reason in _transactionDisputeInfoResponse.Result.DisputeInfo)
					{
						_reasonsList.Add(reason.DisputeReason);
					}

					// If we have a transaction type with restricted reasons, use those instead.
					if (_transactionDisputeInfoResponse?.Result.DisputeRules != null)
					{
						foreach (var rule in _transactionDisputeInfoResponse.Result.DisputeRules)
						{
							if (rule.TransactionCode == (SelectedTransaction.ActionCode + SelectedTransaction.SourceCode) || _isCreditCard && rule.TransactionCode == "CREDITCARD")
							{
								_reasonsList.Clear();
								_reasonsList = rule.DisputeReasons;
								break;
							}
						}
					}

					_reasonsList.Insert(0, SELECT_A_REASON);

					if (_disputeType != "ach" && _disputeType != "atm")
					{
						_reasonsList.Remove("ATM");
						_reasonsList.Remove("ACH");
					}
				}
				else
				{
					if (_questionList != null)
					{
						ShowQuestions();
						ShowHideQuestions();
					}

					if (_answerList != null)
					{
						FillAnswers();
					}
				}

				var reasonsAdapter = new ArrayAdapter(Activity, Resource.Layout.support_simple_spinner_dropdown_item, _reasonsList);
				spinnerReason.Adapter = reasonsAdapter;

				var cardHolderName = string.Empty;
				var memberName = string.Empty;

				if (SelectedTransaction.CardNumber != null)
				{
					foreach (var card in _cardResponse.Result)
					{
						if (card?.CardAccountNumber != null && card.CardAccountNumber.Substring(Math.Max(0, card.CardAccountNumber.Length - 4)) == SelectedTransaction.CardNumber.Substring(Math.Max(0, SelectedTransaction.CardNumber.Length - 4)))
						{
							cardHolderName = card.CardHolderName;
						}
					}
				}

				if (_memberResponse != null && _memberResponse.MemberId > 0)
				{
					memberName = _memberResponse.FirstName + " " + _memberResponse.LastName;
				}

				if (cardHolderName != string.Empty)
				{
					lblCardHolderName.Text = cardHolderName;
				}
				else
				{
					lblCardHolderName.Text = memberName;
				}

				if (int.Parse(SelectedMemberId) == GeneralUtilities.GetMemberIdAsInt() && (cardHolderName == string.Empty || cardHolderName == memberName))
				{
					txtAddressLine1.Text = _memberResponse.Address1;
					txtAddressLine2.Text = _memberResponse.Address2;
					txtCity.Text = _memberResponse.City;
					spinnerState.SetSelection(USStates.USStateList.Keys.ToList().FindIndex(x => x == _memberResponse.State));
					txtZipCode.Text = _memberResponse.Zip.Replace("-", string.Empty);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionDisputeFragment:GetReasonsAndCardHolderInformation");
			}
		}

		private void ShowHideQuestions()
		{
			try
			{
				GatherAnswers(false);

				for (int i = 0; i < tableQuestions.ChildCount; i++)
				{
					var view = tableQuestions.GetChildAt(i);

					if (view is TableRow)
					{
						if (view.Tag != null && (string)view.Tag != string.Empty)
						{
							var json = (string)view.Tag;
							var disputeQuestion = JsonConvert.DeserializeObject<TransactionDisputeQuestion>(json);
							view.Visibility = ShouldHideDependencyRow(disputeQuestion) ? ViewStates.Gone : ViewStates.Visible;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionDisputeFragment:ShowHideQuestions");
			}
		}

		private void FormatCurrencyLabel(decimal amount, TextView label)
		{
			var amountString = amount.ToString();
			amountString = StringUtilities.SafeEmptyNumber(StringUtilities.StripInvalidCurrencyChars(amountString));
			label.Text = amountString;
			label.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, amount < 0 ? Resource.Color.Red : Resource.Color.black)));
		}

		private void ShowQuestions()
		{
			try
			{
				// Get dispute questions from transaction dispute reason
				tableQuestions.RemoveAllViews();
				tableDocuments.RemoveAllViews();

				var questionRowCount = 0;
				var margin = (int)GeneralUtilities.ConvertDpToPixel(Activity, 20);

				if (_questionList != null)
				{
					foreach (TransactionDisputeQuestion disputeQuestion in _questionList)
					{
						// Create Divider
						var questionDivider = new View(Activity);
						questionDivider.SetBackgroundResource(Resource.Drawable.default_separatorcolor);

						// Add Divider
						var tableLayoutParams = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1);
						tableQuestions.AddView(questionDivider, tableLayoutParams);
						questionRowCount++;

						// Create Question Row
						var questionRow = new TableRow(Activity);
						questionRow.SetPadding(0, 10, 0, 10);
						questionRow.Tag = JsonConvert.SerializeObject(disputeQuestion);

						// Add Question Row
						tableLayoutParams = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
						tableLayoutParams.Gravity = GravityFlags.Center;
						tableQuestions.AddView(questionRow, tableLayoutParams);
						questionRowCount++;

						// Create Question
						var question = new TextView(Activity);
						question.Text = disputeQuestion.Question;
						question.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));
						question.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
						question.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.TextViewTextColor)));
						question.SetPadding(0, 10, 0, 10);

						// Add Question
						var width = (int)GeneralUtilities.ConvertDpToPixel(Activity, QUESTION_WIDTH);
						var questionLayoutParams = new TableRow.LayoutParams(width, ViewGroup.LayoutParams.WrapContent);
						questionLayoutParams.LeftMargin = margin;
						questionLayoutParams.Gravity = GravityFlags.CenterVertical;
						questionRow.AddView(question, questionLayoutParams);

						if (disputeQuestion.AnswerType == "string")
						{
							// Create Answer
							var answer = new EditText(Activity);
							answer.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));

							if (!string.IsNullOrEmpty(disputeQuestion.Hint))
							{
								answer.Hint = disputeQuestion.Hint;
							}

							if (disputeQuestion.MaxLength <= 0)
							{
								disputeQuestion.MaxLength = 250;
							}

							answer.SetFilters(new Android.Text.IInputFilter[]
							{
								new Android.Text.InputFilterLengthFilter(disputeQuestion.MaxLength)
							});

							// Add Answer
							var answerLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.7f);
							answerLayoutParams.RightMargin = margin;
							answerLayoutParams.Gravity = GravityFlags.Bottom;
							questionRow.AddView(answer, answerLayoutParams);
						}

						if (disputeQuestion.AnswerType == "decimal")
						{
							// Create Answer
							var answer = new EditText(Activity);
							answer.InputType = Android.Text.InputTypes.ClassNumber;
							answer.AfterTextChanged += OnTextChanged;
							answer.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));

							// Add Answer
							var answerLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.7f);
							answerLayoutParams.RightMargin = margin;
							answerLayoutParams.Gravity = GravityFlags.Bottom;
							questionRow.AddView(answer, answerLayoutParams);
						}

						if (disputeQuestion.AnswerType == "int")
						{
							// Create Answer
							var answer = new EditText(Activity);
							answer.InputType = Android.Text.InputTypes.ClassNumber;
							answer.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));

							// Add Answer
							var answerLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.7f);
							answerLayoutParams.RightMargin = margin;
							questionRow.AddView(answer, answerLayoutParams);
						}

						if (disputeQuestion.AnswerType == "bool")
						{
							// Create Answer
							var answer = new Spinner(Activity);
                            var yesOrNo = new List<string> { SELECT_AN_OPTION, CultureTextProvider.YES(), CultureTextProvider.NO() };
							var answerAdapter = new ArrayAdapter(Activity, Resource.Layout.support_simple_spinner_dropdown_item, yesOrNo);
							answer.Adapter = answerAdapter;

							answer.ItemSelected += (sender, e) =>
							{
								if (disputeQuestion.ShowAdditionalTransactionsIfAnswerTrue)
								{
                                    if (((Spinner)sender).SelectedItem.ToString() == CultureTextProvider.YES())
									{
										_canSelectMultipleTransactions = true;
										tableTransactionsHeader.Visibility = ViewStates.Visible;
										tableTransactions.Visibility = ViewStates.Visible;
									}
									else
									{
										_canSelectMultipleTransactions = false;
										tableTransactionsHeader.Visibility = ViewStates.Gone;
										tableTransactions.Visibility = ViewStates.Gone;
									}
								}

								ShowHideQuestions();
							};

							// Add Answer
							var answerLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.7f);
							answerLayoutParams.RightMargin = margin;
							answerLayoutParams.Height = 80;
							answerLayoutParams.Gravity = GravityFlags.CenterVertical;
							questionRow.AddView(answer, answerLayoutParams);
						}

						if (disputeQuestion.AnswerType == "list")
						{
							// Create Answer
							var answer = new Spinner(Activity);
							List<string> list = disputeQuestion.DefaultValues;

							// We shouldn't need this if statement.  Looks like a mono bug.
							if (!list.Contains(SELECT_AN_OPTION))
							{
								list.Insert(0, SELECT_AN_OPTION);
							}

							var answerAdapter = new ArrayAdapter(Activity, Resource.Layout.support_simple_spinner_dropdown_item, list);
							answer.Adapter = answerAdapter;

							answer.ItemSelected += (sender, e) =>
							{
								ShowHideQuestions();
							};

							// Add Answer
							var answerLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 0.7f);
							answerLayoutParams.RightMargin = margin;
							answerLayoutParams.Gravity = GravityFlags.CenterHorizontal;
							questionRow.AddView(answer, answerLayoutParams);
						}

						if (disputeQuestion.AnswerType == "datetime")
						{
							// Create Answer
							var answer = new TextView(Activity);
							answer.Tag = questionRowCount;
							answer.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));
							answer.SetSingleLine(true);
							answer.SetMaxLines(1);

							// Add Answer
							var answerLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1.0f);
							answerLayoutParams.RightMargin = margin;
							answerLayoutParams.LeftMargin = margin;
							answerLayoutParams.Gravity = GravityFlags.CenterVertical;
							questionRow.AddView(answer, answerLayoutParams);

							// Add Arrow
							var image = new ImageView(Activity);
							image.SetBackgroundResource(Resource.Drawable.listitemselect);

							var imageLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
							imageLayoutParams.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
							imageLayoutParams.RightMargin = margin;
							questionRow.AddView(image, imageLayoutParams);

							questionRow.Click += (sender, e) =>
							{
								var requestCode = 100 + (int)((TableRow)sender).GetChildAt(1).Tag;
								var intent = new Intent(Activity, typeof(SelectDateActivity));
								intent.PutExtra("DisableHolidays", false);
								intent.PutExtra("DisableWeekends", false);
								intent.PutExtra("SelectDate", DateTime.Today.ToString());

								switch (disputeQuestion.MaxLength)
								{
									case -1: // Can only select previous days and current day.
										intent.PutExtra("StartDate", DateTime.Today.AddDays(-90).ToString());
										intent.PutExtra("EndDate", DateTime.Today.ToString());
										break;
									case 0:  // Can select any day.
										intent.PutExtra("StartDate", DateTime.Today.AddDays(-90).ToString());
										intent.PutExtra("EndDate", DateTime.Today.AddDays(90).ToString());
										break;
									case 1:  // Can only select current day and future days.
										intent.PutExtra("StartDate", DateTime.Today.ToString());
										intent.PutExtra("EndDate", DateTime.Today.AddDays(90).ToString());
										break;
								}

								StartActivityForResult(intent, requestCode);
							};
						}
					}
				}

				if (_documentsList != null)
				{
					tableDocuments.RemoveAllViews();

					foreach (var document in _documentsList)
					{
						// Create Docuemnt Row
						var documentRow = new TableRow(Activity);
						documentRow.SetPadding(0, 10, 0, 10);
						documentRow.Tag = JsonConvert.SerializeObject(document);

						// Add Document Row
						var documentLayoutRowParams = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
						documentLayoutRowParams.Gravity = GravityFlags.Center;
						tableDocuments.AddView(documentRow, documentLayoutRowParams);

						// Create Document
						var txtDocument = new TextView(Activity);
						txtDocument.Text = document.FileName;
						txtDocument.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));
						txtDocument.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
						txtDocument.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.TextViewTextColor)));
						txtDocument.SetPadding(0, 10, 0, 10);

						// Add Document
						var documentLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
						documentLayoutParams.LeftMargin = margin;
						documentLayoutParams.Gravity = GravityFlags.CenterVertical;
						documentRow.AddView(txtDocument, documentLayoutParams);
					}
				}

				if (_additionalTransactionsList != null)
				{
					tableTransactions.RemoveAllViews();

					foreach (var transaction in _additionalTransactionsList)
					{
						// Create Transaction Date Row
						var transactionDateRow = new TableRow(Activity);
						transactionDateRow.SetPadding(0, 10, 0, 0);

						// Add Transaction Date Row
						var transactionDateLayoutRowParams = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
						transactionDateLayoutRowParams.Gravity = GravityFlags.Center;
						tableTransactions.AddView(transactionDateRow, transactionDateLayoutRowParams);

						// Create Transaction Date
						var txtTransactionPDate = new TextView(Activity);
						txtTransactionPDate.Text = string.Format("{0:MM/dd/yyyy}", transaction.PostingDate);
						txtTransactionPDate.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));
						txtTransactionPDate.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.TextViewTextColor)));
						txtTransactionPDate.SetPadding(0, 0, 0, 0);

						// Add Transaction Date
						var transactionDateLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
						transactionDateLayoutParams.LeftMargin = margin;
						transactionDateLayoutParams.Gravity = GravityFlags.CenterVertical;
						transactionDateRow.AddView(txtTransactionPDate, transactionDateLayoutParams);

						// Create Transaction Row
						var transactionRow = new TableRow(Activity);
						transactionRow.SetPadding(0, 0, 0, 0);

						// Add Transaction Row
						var transactionLayoutRowParams = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
						transactionLayoutRowParams.Gravity = GravityFlags.Center;
						tableTransactions.AddView(transactionRow, transactionLayoutRowParams);

						// Create Transaction
						var txtTransaction = new TextView(Activity);
						txtTransaction.Text = transaction.Description;
						txtTransaction.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));
						txtTransaction.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.TextViewTextColor)));
						txtTransaction.SetPadding(0, 0, 0, 0);

						// Add Transaction
						var transactionLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
						transactionLayoutParams.LeftMargin = margin;
						transactionLayoutParams.Gravity = GravityFlags.CenterVertical;
						transactionRow.AddView(txtTransaction, transactionLayoutParams);

						// Create Transaction Amount Row
						var transactionAmountRow = new TableRow(Activity);
						transactionAmountRow.SetPadding(0, 0, 0, 10);

						// Add Transaction Amount Row
						var transactionAmountLayoutRowParams = new TableLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
						transactionAmountLayoutRowParams.Gravity = GravityFlags.Center;
						tableTransactions.AddView(transactionAmountRow, transactionAmountLayoutRowParams);

						// Create Transaction Amount
						var txtTransactionAmt = new TextView(Activity);
						FormatCurrencyLabel(transaction.TransactionAmount, txtTransactionAmt);
						txtTransactionAmt.SetTextSize(Android.Util.ComplexUnitType.Px, Resources.GetDimension(Resource.Dimension.TextSizeMedium));
						txtTransactionAmt.SetPadding(0, 0, 0, 10);

						// Add Transaction Amount
						var transactionAmountLayoutParams = new TableRow.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
						transactionAmountLayoutParams.LeftMargin = margin;
						transactionAmountLayoutParams.Gravity = GravityFlags.CenterVertical;
						transactionAmountRow.AddView(txtTransactionAmt, transactionAmountLayoutParams);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionDisputeFragment:ShowQuestions");
			}
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			((EditText)sender).AfterTextChanged -= OnTextChanged;
			((EditText)sender).Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((EditText)sender).Text);
			((EditText)sender).SetSelection(((EditText)sender).Text.Length);
			((EditText)sender).AfterTextChanged += OnTextChanged;
			return;
		}

		private void SelectDocuments()
		{
			var intent = new Intent(Activity, typeof(UploadDisputeDocumentsActivity));
			StartActivityForResult(intent, (int)ActivityResults.UploadDocuments);
		}

		private void AddTransactions()
		{
			var intent = new Intent(Activity, typeof(TransactionsSelectionActivity));
			var dTJson = JsonConvert.SerializeObject(SelectedTransaction);
			intent.PutExtra("DisputedTransaction", dTJson);
			intent.PutExtra("AccountDescription", SelectedAccountDescription);
			intent.PutExtra("Suffix", SelectedSuffix);
			intent.PutExtra("MemberId", SelectedMemberId);
			intent.PutExtra("MaxSelectedTransactions", MAX_TRANSACTIONS);
            intent.PutExtra("LimitToDisputedMerchant", spinnerReason.SelectedItem.ToString().ToUpper() == CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "2DC07692-B453-42E5-BB78-D1E72D06A3FE", "CANCELLED SERVICE"));

			if (_additionalTransactionsList == null)
			{
				_additionalTransactionsList = new List<Transaction>();
			}

			var json = JsonConvert.SerializeObject(_additionalTransactionsList);
			intent.PutExtra("AdditionalTransactions", json);

			StartActivityForResult(intent, (int)ActivityResults.AddTransactions);
		}

		public bool ShouldHideDependencyRow(TransactionDisputeQuestion item)
		{
			bool returnValue = false;

			returnValue |= (item.VisibleDependencyQuestion > 0 && _answerList != null && _answerList.Count >= item.VisibleDependencyQuestion && item.VisibleDependencyAnswer != _answerList[item.VisibleDependencyQuestion - 1].Answer);
			returnValue |= (item.HideIfNegativeTransactionAmount && SelectedTransaction.TransactionAmount < 0);

			return returnValue;
		}

		private async void GatherAnswers(bool gatherForReview)
		{
			_inReview = true;
			_answerList = new List<TransactionDisputeAnswer>();

			for (int i = 0; i < tableQuestions.ChildCount; i++)
			{
				var view = tableQuestions.GetChildAt(i);

				if (view is TableRow)
				{
					if (view.Tag != null && (string)view.Tag != string.Empty)
					{
						var json = (string)view.Tag;

						var disputeQuestion = JsonConvert.DeserializeObject<TransactionDisputeQuestion>(json);

						var answer = string.Empty;
						var answerSelection = 0;

						if (disputeQuestion.AnswerType == "string" || disputeQuestion.AnswerType == "decimal" || disputeQuestion.AnswerType == "int")
						{
							answer = ((EditText)((TableRow)view).GetChildAt(1)).Text;
						}

						if (disputeQuestion.AnswerType == "bool" || disputeQuestion.AnswerType == "list")
						{
							answer = ((Spinner)((TableRow)view).GetChildAt(1)).SelectedItem.ToString();
							answerSelection = ((Spinner)((TableRow)view).GetChildAt(1)).SelectedItemPosition;
						}

						if (disputeQuestion.AnswerType == "datetime")
						{
							answer = ((TextView)((TableRow)view).GetChildAt(1)).Text;
						}

						if (gatherForReview)
						{
							if (((string.IsNullOrEmpty(answer) || answer == SELECT_AN_OPTION || answer.Length < disputeQuestion.MinLength)) && disputeQuestion.IsAnswerRequired
								&& !ShouldHideDependencyRow(disputeQuestion))
							{
								_inReview = false;
                                await AlertMethods.Alert(Activity, "SunMobile",
		                             CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "5F154679-DCAA-4A9C-922B-14D99D7181FA", "All answers must be filled in."),
		                             CultureTextProvider.OK());

								gatherForReview = false;
								break;
							}
							else
							{
								if (ShouldHideDependencyRow(disputeQuestion))
								{
									answer = string.Empty;
								}

								_answerList.Add(new TransactionDisputeAnswer { AnswerType = disputeQuestion.AnswerType, Answer = answer, AnswerSelection = answerSelection });
							}
						}
						else
						{
							_answerList.Add(new TransactionDisputeAnswer { AnswerType = disputeQuestion.AnswerType, Answer = answer, AnswerSelection = answerSelection });
						}
					}
				}
			}

			if (gatherForReview)
			{
				var methods = new AccountMethods();
				var request = PopulateSubmitTransactionDisputeRequest();
				var validationMessage = methods.ValidateSubmitTransactionDisputeRequest(request, _canSelectMultipleTransactions);

				if (validationMessage != string.Empty)
				{
					_inReview = false;
                    await AlertMethods.Alert(Activity, "SunMobile", validationMessage, CultureTextProvider.OK());
				}
			}
		}

		private void FillAnswers()
		{
			var answerPosition = 0;

			if (_answerList != null)
			{
				for (int i = 0; i < tableQuestions.ChildCount; i++)
				{
					var view = tableQuestions.GetChildAt(i);

					if (view is TableRow)
					{
						if (view.Tag != null && (string)view.Tag != string.Empty)
						{
							var json = (string)view.Tag;

							TransactionDisputeQuestion disputeQuestion = JsonConvert.DeserializeObject<TransactionDisputeQuestion>(json);

							var answer = _answerList[answerPosition].Answer;
							var answerSelection = _answerList[answerPosition].AnswerSelection;

							if (disputeQuestion.AnswerType == "string" || disputeQuestion.AnswerType == "decimal" || disputeQuestion.AnswerType == "int")
							{
								if (answer != null)
								{
									((EditText)((TableRow)view).GetChildAt(1)).Text = answer;
								}
							}

							if (disputeQuestion.AnswerType == "bool" || disputeQuestion.AnswerType == "list")
							{
								((Spinner)((TableRow)view).GetChildAt(1)).SetSelection(answerSelection);
							}

							if (disputeQuestion.AnswerType == "datetime")
							{
								if (answer != null)
								{
									((TextView)((TableRow)view).GetChildAt(1)).Text = answer;
								}
							}

							answerPosition++;
						}
					}
				}
			}
		}

		private async void ReviewDispute()
		{
			GatherAnswers(true);

			if (_inReview)
			{
				var agreement = _transactionDisputeInfo.DisputeAgreement.Replace("\\n", "\n");
                var response = string.Empty;
				var request = PopulateSubmitTransactionDisputeRequest();

				if (request.Reason == 11 || request.Reason == 12)
				{
                    var alertResponse = await AlertMethods.AlertWithCheckBox(Activity, 
                        CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "D94BBBF4-6399-418B-942D-73272BA5B9CB", "Confirm Dispute"),
                        agreement,
		                CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "4AD5D983-969D-4899-8BC1-C1E26F3DCCC5", "I understand that selecting Submit will also close my card and a replacement card will be ordered and sent to my mailing address."),
                        true,
                        false,
		                CultureTextProvider.SUBMIT(), CultureTextProvider.NOREVIEW());

                    if (alertResponse != null)
                    {
                        response = alertResponse.Item1;
                    }
				}
				else
				{
                    response = await AlertMethods.Alert(Activity, 
	                    CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "D94BBBF4-6399-418B-942D-73272BA5B9CB", "Confirm Dispute"),
	                    agreement, CultureTextProvider.SUBMIT(), CultureTextProvider.NOREVIEW());
				}

                if (response == CultureTextProvider.SUBMIT())
				{
					SubmitDispute();
				}
			}
		}

		private SubmitTransactionDisputeRequest PopulateSubmitTransactionDisputeRequest()
		{
			var request = new SubmitTransactionDisputeRequest();

			try
			{
				if (_documentsList == null)
				{
					_documentsList = new List<FileInformation>();
				}

				if (_additionalTransactionsList == null)
				{
					_additionalTransactionsList = new List<Transaction>();
				}

				if (_disputeType.ToUpper() == "ACH")
				{
					request.Reason = _transactionDisputeInfo.DisputeQuestions[0].DefaultValues.FindIndex(x => x == _answerList[0].Answer);
				}
				else
				{
					request.Reason = _transactionDisputeInfoResponse.Result.DisputeInfo.Select((f, i) => new { Field = f, Index = i })
						.Where(x => x.Field.DisputeReason == spinnerReason.SelectedItem.ToString())
						.Select(x => x.Index)
						.DefaultIfEmpty(-1)
						.FirstOrDefault() + 1;
				}

				request.DisputeType = _disputeType;
				request.Answers = _answerList.Select(x => x.Answer).ToList();
				request.DocumentIds = _documentsList.Select(x => x.FileId).ToList();
				request.CardNumber = SelectedTransaction.CardNumber != null ? SelectedTransaction.CardNumber.Substring(SelectedTransaction.CardNumber.Length - 4) : string.Empty;
				request.Suffix = SelectedSuffix.Substring(1);
				request.CardholderName = lblCardHolderName.Text;
				request.AddressLine1 = txtAddressLine1.Text;
				request.AddressLine2 = txtAddressLine2.Text;
				request.City = txtCity.Text;
				request.State = spinnerState.SelectedItem.ToString();
				request.UnmaskedZipCode = txtZipCode.Text.Replace("-", string.Empty);
				request.DaytimePhone = txtDayTimePhone.Text.Replace("-", string.Empty);

				request.AdditionalTransactions = new List<AdditionalTransactionInfo>();
				var currentTransactionInfo = new AdditionalTransactionInfo();
				currentTransactionInfo.Description = SelectedTransaction.Description;
				currentTransactionInfo.TransactionDate = SelectedTransaction.TransactionDate;
				currentTransactionInfo.TransactionAmount = SelectedTransaction.TransactionAmount.ToString();
				currentTransactionInfo.TransactionType = SelectedTransaction.ActionCode == "D" ? "1" : "2";
				currentTransactionInfo.TransactionId = SelectedTransaction.SequenceNumber;
				request.AdditionalTransactions.Add(currentTransactionInfo);

				foreach (var transaction in _additionalTransactionsList)
				{
					var transactionInfo = new AdditionalTransactionInfo();
					transactionInfo.Description = transaction.Description;
					transactionInfo.TransactionDate = transaction.TransactionDate;
					transactionInfo.TransactionAmount = transaction.TransactionAmount.ToString();
					transactionInfo.TransactionType = transaction.ActionCode == "D" ? "1" : "2";
					transactionInfo.TransactionId = transaction.SequenceNumber;
					request.AdditionalTransactions.Add(transactionInfo);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionDisputeFragment:PopulateSubmitTransactionDisputeRequest");
			}

			return request;
		}

		private async void SubmitDispute()
		{
			var methods = new AccountMethods();
			var request = PopulateSubmitTransactionDisputeRequest();

			var successText = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "6B921DDC-79A5-48AF-A998-B1167A4937C2", "Dispute submitted successfully.");
			var failedText = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "7EB12290-690A-404F-B8D3-C60D89DB5B04", "Dispute submission failed.");

            ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "2D5F3E48-3752-4F51-95FD-FFE17BEF1F38", "Submitting Dispute..."));

			var response = await methods.SubmitTransactionDispute(request, Activity);

			HideActivityIndicator();

			if (response?.Success == true)
			{
                await AlertMethods.Alert(Activity, "SunMobile", successText, CultureTextProvider.OK());
				NavigationService.NavigatePop();
                Logging.Track("Transaction dispute successful.");
			}
			else
			{
                await AlertMethods.Alert(Activity, "SunMobile", failedText, CultureTextProvider.OK());
                Logging.Track("Transaction dispute failed.");
			}
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (requestCode == (int)ActivityResults.UploadDocuments && resultCode == (int)Result.Ok)
			{
				var json = data.GetStringExtra("FileList");
				var documents = JsonConvert.DeserializeObject<List<FileInformation>>(json);

				if (_documentsList.Count != 0)
				{
					_documentsList.RemoveRange(0, _documentsList.Count);
				}
				_documentsList.AddRange(documents);
				GatherAnswers(false);
				ShowQuestions();
				FillAnswers();
			}
			else if (requestCode == (int)ActivityResults.AddTransactions && resultCode == (int)Result.Ok)
			{
				var json = data.GetStringExtra("SelectedTransations");
				var transactions = JsonConvert.DeserializeObject<List<Transaction>>(json);
				if (_additionalTransactionsList.Count != 0)
				{
					_additionalTransactionsList.RemoveRange(0, _additionalTransactionsList.Count);
				}
				_additionalTransactionsList.AddRange(transactions);
				GatherAnswers(false);
				ShowQuestions();
				FillAnswers();
			}
			else
			{
				for (int i = 0; i < tableQuestions.ChildCount; i++)
				{
					var view = tableQuestions.GetChildAt(i);

					if (view is TableRow)
					{
						if (((TableRow)view).ChildCount == 3 && ((TableRow)view).GetChildAt(1) is TextView)
						{
							var textView = (TextView)((TableRow)view).GetChildAt(1);

							if ((int)textView.Tag == requestCode - 100)
							{
								if (resultCode == (int)Result.Ok && data != null)
								{
									var json = data.GetStringExtra("SelectedDate");
									var _inDate = JsonConvert.DeserializeObject<DateTime>(json);
									textView.Text = string.Format("{0:MM/dd/yyyy}", _inDate);
									break;
								}
							}
						}
					}
				}
			}
		}
	}
}