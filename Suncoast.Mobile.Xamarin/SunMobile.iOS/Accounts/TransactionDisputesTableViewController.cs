using System;
using System.Collections.Generic;
using System.Linq;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.Member;
using SunBlock.DataTransferObjects.Extensions;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.OnBase;
using SunMobile.iOS.Common;
using SunMobile.Shared.Cards;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.States;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using UIKit;

namespace SunMobile.iOS.Accounts
{
	public partial class TransactionDisputesTableViewController : BaseTableViewController
	{
		public Transaction SelectedTransaction { get; set; }
		public string SelectedMemberId { get; set; }
		public string SelectedSuffix { get; set; }
		public string AccountDescription { get; set; }
		private MobileStatusResponse<TransactionDisputeInformationResponse> _transactionDisputeInfoResponse;
		private TransactionDisputeInfo _transactionDisputeInfo;
		private List<string> _reasonList;
		private List<TransactionDisputeQuestion> _questionList;
		private List<TransactionDisputeAnswer> _answerList;
		private List<FileInformation> _documentsList;
		private List<Transaction> _additionalTransactionsList;
		private List<DisputeRow> _disputeRows;
		private string _disputeType;
		private bool _isCreditCard;
		private bool _canSelectMultipleTransactions;
		private string SELECT_A_REASON = "Select";
		private string SELECT_AN_OPTION = "Select";
		private const int MAX_TRANSACTIONS = 19;

		private class DisputeRow
		{
			public TransactionDisputeQuestion question;
			public UITableViewCell cell;
			public UILabel lblQuestion;
			public UITextField txtAnswer;
			public UILabel lblAnswer;

			public DisputeRow(TransactionDisputeQuestion disputeQuestion, UITableViewCell tableViewCell, UILabel labelQuestion, UITextField textAnswer, UILabel labelAnswer)
			{
				question = disputeQuestion;
				cell = tableViewCell;
				lblQuestion = labelQuestion;
				txtAnswer = textAnswer;
				lblAnswer = labelAnswer;
			}
		}

		public TransactionDisputesTableViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var buttonText = CultureTextProvider.CONTINUE();
            var rightButton = new UIBarButtonItem(buttonText, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => ReviewDispute();

			_isCreditCard = SelectedSuffix.Length > 5;

			txtDaytimePhone.EditingChanged += (sender, e) =>
			{
				GatherAnswersAndValidate();
			};

			txtDaytimePhone.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 10;
			};

			txtAddressLine1.EditingChanged += (sender, e) =>
			{
				GatherAnswersAndValidate();
			};

			txtAddressLine1.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 40;
			};

			txtCity.EditingChanged += (sender, e) =>
			{
				GatherAnswersAndValidate();
			};

			txtCity.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 33;
			};

			txtState.EditingChanged += (sender, e) =>
			{
				GatherAnswersAndValidate();
			};

			txtZipCode.EditingChanged += (sender, e) =>
			{
				GatherAnswersAndValidate();
			};

			txtZipCode.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 9;
			};

			btnUploadDocuments.TouchUpInside += (sender, e) => SelectDocuments();
			btnAddTransactions.TouchUpInside += (sender, e) => SelectTransactions();

			lblTransactionDate.Text = string.Format("{0:MM/dd/yyyy}", SelectedTransaction.TransactionDate.Date);
			lblTransactionDescription.Text = SelectedTransaction.Description;
			lblTransactionAmount.Text = StringUtilities.FormatAsCurrency(SelectedTransaction.TransactionAmount.ToString());

			if (SelectedTransaction.TransactionAmount < 0)
			{
				lblTransactionAmount.TextColor = UIColor.Red;
			}

			GetReasonsAndCardHolderInformation();

			ShowDocuments();
		}

        public override void SetCultureConfiguration()
        {
            Title = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "8BD25D4A-6B7B-4545-B288-B6C67586F137", "Transaction Dispute");
            CultureTextProvider.SetMobileResourceText(lblCardHolderLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "C12FBC08-F395-433B-9E02-92B131E06707");
            CultureTextProvider.SetMobileResourceText(lblPhoneLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0D4022CE-894B-483E-B024-641CD5E44861");
            CultureTextProvider.SetMobileResourceText(lblAddress1Label, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "A4DE4858-AF9F-4171-88DE-E627E366EBCA");
            CultureTextProvider.SetMobileResourceText(lblAddress2Label, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "38D74819-CF0B-4859-8B4A-B73AB98339B5");
            CultureTextProvider.SetMobileResourceText(lblCityLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "6F3D40B5-FCEB-49E0-864F-F61D9C2F7DC6");
            CultureTextProvider.SetMobileResourceText(lblStateLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "3EB5BA39-31D3-45FD-98ED-221F9D4A5F36");
            CultureTextProvider.SetMobileResourceText(lblZipLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "68B60EDA-C284-4EFA-84A4-42923A147735");
            CultureTextProvider.SetMobileResourceText(lblTransactionDescriptionLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "E7804EBE-3832-4679-8F94-4E93D5DFF6B4");
            CultureTextProvider.SetMobileResourceText(lblTransactionDateLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "4A3232E6-4B53-4756-94C3-AF58BAD475CC");
            CultureTextProvider.SetMobileResourceText(lblAmountLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "F683B4B2-07E4-431F-AD8A-2590F8A70937");
            CultureTextProvider.SetMobileResourceText(lblDisputeReasonLabel, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0B1A176A-53A3-4516-BDD9-55FADCF29BB8");
            CultureTextProvider.SetMobileResourceText(btnUploadDocuments, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "C1E99194-9061-4F57-8D02-BE1D6CACF341");
            CultureTextProvider.SetMobileResourceText(btnAddTransactions, "EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "704F5538-1FEE-494D-A9FF-E6A709D1AB61");
            SELECT_A_REASON = CultureTextProvider.SELECT();
            SELECT_AN_OPTION = CultureTextProvider.SELECT();
            txtAddressLine2.Placeholder = CultureTextProvider.OPTIONAL();
        }

		private async void GetReasonsAndCardHolderInformation()
		{
			try
			{
				var accountMethods = new AccountMethods();
				var memberRequest = new MemberInformationRequest { MemberId = int.Parse(SelectedMemberId) };
				var cardMethods = new CardMethods();
				var cardListRequest = new CardListRequest { ExcludeAtmCards = false };

				ShowActivityIndicator();

				_transactionDisputeInfoResponse = await accountMethods.GetTransactionDisputeInformation(null, View);
				var memberResponse = await accountMethods.GetMemberInformation(memberRequest, View);
				var cardResponse = await cardMethods.CardList(cardListRequest, View);

				HideActivityIndicator();

				_reasonList = new List<string>();
				_disputeType = accountMethods.GetDisputeInfo(SelectedTransaction, SelectedMemberId, SelectedSuffix.Length > 5).DisputeType;

				// Add all posible dispute reasons.
				foreach (TransactionDisputeInfo reason in _transactionDisputeInfoResponse.Result.DisputeInfo)
				{
					_reasonList.Add(reason.DisputeReason);
				}

				// If we have a transaction type with restricted reasons, use those instead.
				if (_transactionDisputeInfoResponse?.Result.DisputeRules != null)
				{
					foreach (var rule in _transactionDisputeInfoResponse.Result.DisputeRules)
					{
						if (rule.TransactionCode == (SelectedTransaction.ActionCode + SelectedTransaction.SourceCode) || _isCreditCard && rule.TransactionCode == "CREDITCARD")
						{
							_reasonList.Clear();
							_reasonList = rule.DisputeReasons;
							break;
						}
					}
				}

				_reasonList.Remove("ATM");
				_reasonList.Remove("ACH");

				_reasonList.Insert(0, SELECT_A_REASON);

				CommonMethods.CreateDropDownFromTextFieldWithDelegate(txtReason, _reasonList, (text) =>
				{
					ReasonChanged(text);
				}, 14f);

				txtReason.Text = _reasonList[0];

				CommonMethods.CreateDropDownFromTextField(txtState, USStates.USStateList.Keys.ToList());

				var cardHolderName = string.Empty;
				var memberName = string.Empty;

				if (SelectedTransaction.CardNumber != null)
				{
					foreach (var card in cardResponse.Result)
					{
						if (card?.CardAccountNumber != null && card.CardAccountNumber.Substring(Math.Max(0, card.CardAccountNumber.Length - 4)) == SelectedTransaction.CardNumber.Substring(Math.Max(0, SelectedTransaction.CardNumber.Length - 4)))
						{
							cardHolderName = card.CardHolderName;
						}
					}
				}

				if (memberResponse != null && memberResponse.MemberId > 0)
				{
					memberName = memberResponse.FirstName + " " + memberResponse.LastName;
				}

				if (cardHolderName != string.Empty)
				{
					lblCardholderName.Text = cardHolderName;
				}
				else
				{
					lblCardholderName.Text = memberName;
				}

				if (int.Parse(SelectedMemberId) == GeneralUtilities.GetMemberIdAsInt() && (cardHolderName == string.Empty || cardHolderName == memberName))
				{
					txtAddressLine1.Text = memberResponse.Address1;
					txtAddressLine2.Text = memberResponse.Address2;
					txtCity.Text = memberResponse.City;
					txtState.Text = memberResponse.State;
					txtZipCode.Text = memberResponse.Zip.Replace("-", string.Empty);
				}

				ReasonChanged(_reasonList[0]);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "TransactionDisputesTableViewController:GetReasonsAndCardHolderInformation");
			}
		}

		private void ReasonChanged(string text)
		{
			if (_disputeType == "ach")
			{
				_transactionDisputeInfo = _transactionDisputeInfoResponse.Result.DisputeInfo.Find(x => x.DisputeReason == "ACH");
				lblCardHolderLabel.Text = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "CACEE294-ED8F-40F1-9415-3954E31A4F8A", "Contact Name");
			}
			else if (_disputeType == "atm")
			{
				_transactionDisputeInfo = _transactionDisputeInfoResponse.Result.DisputeInfo.Find(x => x.DisputeReason == "ATM");
			}
			else if (text == SELECT_A_REASON)
			{
				_questionList = null;
				btnUploadDocuments.Enabled = false;
				_transactionDisputeInfo = new TransactionDisputeInfo { DisputeQuestions = new List<TransactionDisputeQuestion>() };
			}
			else
			{
				var reasonPositionAdjusted = -1;

				for (int i = 0; i < _transactionDisputeInfoResponse.Result.DisputeInfo.Count; i++)
				{
					if (text == _transactionDisputeInfoResponse.Result.DisputeInfo[i].DisputeReason)
					{
						reasonPositionAdjusted = i;
					}
				}

				_transactionDisputeInfo = (reasonPositionAdjusted != -1) ? _transactionDisputeInfoResponse.Result.DisputeInfo[reasonPositionAdjusted] : _transactionDisputeInfoResponse.Result.DisputeInfo[_reasonList.IndexOf(text) - 1];
			}

			_questionList = _transactionDisputeInfo.DisputeQuestions;

			if (_transactionDisputeInfo.CanSubmitDocuments)
			{
				btnUploadDocuments.Enabled = true;
			}
			else
			{
				btnUploadDocuments.Enabled = false;
			}

			if (!_transactionDisputeInfo.CanSubmitDocuments)
			{
				_documentsList = null;
			}

			_canSelectMultipleTransactions = false;
			_additionalTransactionsList = null;

			// Add the controls to a list to make things easier.
			_disputeRows = new List<DisputeRow>();
			DisputeRow row = null;

			for (int i = 1; i <= _questionList.Count; i++)
			{
				switch (i)
				{
					case 1:
						row = new DisputeRow(_questionList[0], cellQuestion1, lblQuestion1, txtAnswer1, lblAnswer1);
						break;
					case 2:
						row = new DisputeRow(_questionList[1], cellQuestion2, lblQuestion2, txtAnswer2, lblAnswer2);
						break;
					case 3:
						row = new DisputeRow(_questionList[2], cellQuestion3, lblQuestion3, txtAnswer3, lblAnswer3);
						break;
					case 4:
						row = new DisputeRow(_questionList[3], cellQuestion4, lblQuestion4, txtAnswer4, lblAnswer4);
						break;
					case 5:
						row = new DisputeRow(_questionList[4], cellQuestion5, lblQuestion5, txtAnswer5, lblAnswer5);
						break;
					case 6:
						row = new DisputeRow(_questionList[5], cellQuestion6, lblQuestion6, txtAnswer6, lblAnswer6);
						break;
					case 7:
						row = new DisputeRow(_questionList[6], cellQuestion7, lblQuestion7, txtAnswer7, lblAnswer7);
						break;
					case 8:
						row = new DisputeRow(_questionList[7], cellQuestion8, lblQuestion8, txtAnswer8, lblAnswer8);
						break;
					case 9:
						row = new DisputeRow(_questionList[8], cellQuestion9, lblQuestion9, txtAnswer9, lblAnswer9);
						break;
					case 10:
						row = new DisputeRow(_questionList[9], cellQuestion10, lblQuestion10, txtAnswer10, lblAnswer10);
						break;
					case 11:
						row = new DisputeRow(_questionList[10], cellQuestion11, lblQuestion11, txtAnswer11, lblAnswer11);
						break;
					case 12:
						row = new DisputeRow(_questionList[11], cellQuestion12, lblQuestion12, txtAnswer12, lblAnswer12);
						break;
					case 13:
						row = new DisputeRow(_questionList[12], cellQuestion13, lblQuestion13, txtAnswer13, lblAnswer13);
						break;
					case 14:
						row = new DisputeRow(_questionList[13], cellQuestion14, lblQuestion14, txtAnswer14, lblAnswer14);
						break;
					case 15:
						row = new DisputeRow(_questionList[14], cellQuestion15, lblQuestion15, txtAnswer15, lblAnswer15);
						break;
					case 16:
						row = new DisputeRow(_questionList[15], cellQuestion16, lblQuestion16, txtAnswer16, lblAnswer16);
						break;
					case 17:
						row = new DisputeRow(_questionList[16], cellQuestion17, lblQuestion17, txtAnswer17, lblAnswer17);
						break;
				}

				_disputeRows.Add(row);
			}

			ShowQuestions();
		}

		private void ShowQuestions()
		{
			if (_disputeRows != null)
			{
				foreach (var row in _disputeRows)
				{
					DisplayCell(row.question, row.cell, row.lblQuestion, row.txtAnswer, row.lblAnswer);
				}

				tableMain.ReloadData();
			}
		}

		private void ShowDocuments()
		{
			DisplayDocuments();
			tableMain.ReloadData();
		}

		private void ShowTransactions()
		{
			DisplayTransactions();
			tableMain.ReloadData();
		}

		private void EditingChangedString(object sender, EventArgs e)
		{
			GatherAnswersAndValidate();
		}

		private void EditingChangedDecimal(object sender, EventArgs e)
		{
			((UITextField)sender).Text = StringUtilities.StripInvalidCurrencCharsAndFormat(((UITextField)sender).Text);
			GatherAnswersAndValidate();
		}

		private void DisplayCell(TransactionDisputeQuestion item, UITableViewCell cell, UILabel lblQuestion, UITextField txtAnswer, UILabel lblAnswer)
		{
			lblQuestion.Text = item.Question;
			txtAnswer.Text = string.Empty;
			txtAnswer.Hidden = false;
			CommonMethods.RemoveDropDownFromTextField(txtAnswer);
			lblAnswer.Text = string.Empty;
			lblAnswer.Hidden = true;
			cell.Accessory = UITableViewCellAccessory.None;
			txtAnswer.EditingChanged -= EditingChangedString;
			txtAnswer.EditingChanged -= EditingChangedDecimal;

			switch (item.AnswerType)
			{
				case "string":
					if (item.DefaultValues != null && item.DefaultValues.Count >= 1)
					{
						txtAnswer.Text = item.DefaultValues[0];
					}

					txtAnswer.EditingChanged += EditingChangedString;

					if (!string.IsNullOrEmpty(item.Hint))
					{
						txtAnswer.Placeholder = item.Hint;
					}

					if (item.MaxLength <= 0)
					{
						item.MaxLength = 250;
					}

					txtAnswer.ShouldChangeCharacters = (textField, range, replacementString) =>
					{
						var newLength = textField.Text.Length + replacementString.Length - range.Length;
						return newLength <= item.MaxLength;
					};
					break;
				case "int":
					if (item.DefaultValues != null && item.DefaultValues.Count >= 1)
					{
						txtAnswer.Text = item.DefaultValues[0];
					}

					txtAnswer.EditingChanged += EditingChangedString;

					if (!string.IsNullOrEmpty(item.Hint))
					{
						txtAnswer.Placeholder = item.Hint;
					}

					if (item.MaxLength <= 0)
					{
						item.MaxLength = 250;
					}

					txtAnswer.ShouldChangeCharacters = (textField, range, replacementString) =>
					{
						var newLength = textField.Text.Length + replacementString.Length - range.Length;
						return replacementString.IsNumericOrEmpty() && newLength <= item.MaxLength;
					};
					break;
				case "decimal":
					if (item.DefaultValues != null && item.DefaultValues.Count >= 1)
					{
						txtAnswer.Text = item.DefaultValues[0];
					}

					txtAnswer.EditingChanged += EditingChangedDecimal;

					if (!string.IsNullOrEmpty(item.Hint))
					{
						txtAnswer.Placeholder = item.Hint;
					}

					txtAnswer.ShouldChangeCharacters = (textField, range, replacementString) =>
					{
						var newLength = textField.Text.Length + replacementString.Length - range.Length;
						return replacementString.IsNumericOrEmpty() && newLength <= 15;
					};
					break;
				case "bool":
                    var yesOrNo = new List<string> { SELECT_AN_OPTION, CultureTextProvider.YES(), CultureTextProvider.NO() };

					CommonMethods.CreateDropDownFromTextFieldWithDelegate(txtAnswer, yesOrNo, (obj) =>
					{
						GatherAnswersAndValidate();

						if (item.ShowAdditionalTransactionsIfAnswerTrue)
						{
                            _canSelectMultipleTransactions = (obj == CultureTextProvider.YES());
							_additionalTransactionsList = null;
							ShowTransactions();
						}

						// The answer could be used to hide / show questions so update the table
						tableMain.BeginUpdates();
						tableMain.EndUpdates();
					});

					txtAnswer.Text = SELECT_AN_OPTION;
					break;
				case "datetime":
					txtAnswer.Hidden = true;
					lblAnswer.Hidden = false;

					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					break;
				case "list":
					if (item.DefaultValues != null && item.DefaultValues.Count > 1)
					{
						var values = item.DefaultValues;
						values.Insert(0, SELECT_AN_OPTION);
						CommonMethods.CreateDropDownFromTextFieldWithDelegate(txtAnswer, values, (obj) =>
						{
							GatherAnswersAndValidate();

							// The answer could be used to hide / show questions so update the table.
							tableMain.BeginUpdates();
							tableMain.EndUpdates();
						}, 12f);

						txtAnswer.Text = SELECT_AN_OPTION;
					}
					break;
			}
		}

		public void GatherAnswersAndValidate()
		{
			var valid = true;

			_answerList = new List<TransactionDisputeAnswer>();

			foreach (var row in _disputeRows)
			{
				var answer = new TransactionDisputeAnswer
				{
					AnswerType = row.question.AnswerType
				};

				if (!string.IsNullOrEmpty(row.lblAnswer.Text))
				{
					answer.Answer = row.lblAnswer.Text;
				}

				if (!string.IsNullOrEmpty(row.txtAnswer.Text))
				{
					answer.Answer = row.txtAnswer.Text;
				}

				if (((string.IsNullOrEmpty(answer.Answer) || answer.Answer == SELECT_AN_OPTION || answer.Answer.Length < row.question.MinLength)) && row.question.IsAnswerRequired)
				{
					if (!ShouldHideDependencyRow(row.question))
					{
						valid = false;
					}
					else
					{
						answer.Answer = string.Empty;
					}
				}

				if (string.IsNullOrEmpty(answer.Answer))
				{
					answer.Answer = string.Empty;
				}

				_answerList.Add(answer);
			}

			var methods = new AccountMethods();
			var request = PopulateSubmitTransactionDisputeRequest();
			var validationMessage = methods.ValidateSubmitTransactionDisputeRequest(request, false);

			if (validationMessage != string.Empty)
			{
				valid = false;
			}

			NavigationItem.RightBarButtonItem.Enabled = valid;
		}

		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			if (indexPath.Section == 1 && indexPath.Row > 3)
			{
				var question = _questionList[indexPath.Row - 4];

				if (question.AnswerType == "datetime")
				{
					GeneralUtilities.CloseKeyboard(View);

					var datePickerViewController = AppDelegate.StoryBoard.InstantiateViewController("DatePickerViewController") as DatePickerViewController;
					datePickerViewController.DisableHolidays = false;
					datePickerViewController.DisableWeekends = false;
					datePickerViewController.SelectDate = DateTime.Today;

					switch (question.MaxLength)
					{
						case -1: // Can only select previous days and current day.
							datePickerViewController.StartDate = DateTime.Today.AddDays(-90);
							datePickerViewController.EndDate = DateTime.Today;
							break;
						case 0:  // Can select any day.
							datePickerViewController.StartDate = DateTime.Today.AddDays(-90);
							datePickerViewController.EndDate = DateTime.Today.AddDays(90);
							break;
						case 1:  // Can only select current day and future days.
							datePickerViewController.StartDate = DateTime.Today;
							datePickerViewController.EndDate = DateTime.Today.AddDays(90);
							break;
					}

					datePickerViewController.ItemSelected += date =>
					{
						_disputeRows[indexPath.Row - 4].lblAnswer.Text = string.Format("{0:MM/dd/yyyy}", date);
					};

					NavigationController.PushViewController(datePickerViewController, true);
				}
			}
		}

		public bool ShouldHideDependencyRow(TransactionDisputeQuestion item)
		{
			bool returnValue = false;

			returnValue |= (item.VisibleDependencyQuestion > 0 && _answerList != null && _answerList.Count >= item.VisibleDependencyQuestion && item.VisibleDependencyAnswer != _answerList[item.VisibleDependencyQuestion - 1].Answer);
			returnValue |= (item.HideIfNegativeTransactionAmount && SelectedTransaction.TransactionAmount < 0);

			return returnValue;
		}

		/*
		public override string TitleForHeader(UITableView tableView, nint section)
		{
			if (section == 2 && (_transactionDisputeInfo == null || (_transactionDisputeInfo != null && !_transactionDisputeInfo.CanSubmitDocuments)))
			{
				return null;
			}

			if (section == 3 && !_canSelectMultipleTransactions)
			{
				return null;
			}

			if (section == 0 && _disputeType != null && _disputeType.ToUpper() == "ACH")
			{
				return "Contact Information";
			}

			return base.TitleForHeader(tableView, section);
		}
		*/

		public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			// Cardholder Information
			if (indexPath.Section == 0 && !string.IsNullOrEmpty(_disputeType) && _disputeType == "ach" && indexPath.Row != 1)
			{
				return 0;
			}

			// Reason Type
			if (indexPath.Section == 1 && indexPath.Row == 3)
			{
				return (!string.IsNullOrEmpty(_disputeType) && (_disputeType == "ach" || _disputeType == "atm")) ? 0 : 74;
			}

			// Documents
			if (indexPath.Section == 2 && indexPath.Row == 0 && (_transactionDisputeInfo == null || (_transactionDisputeInfo != null && !_transactionDisputeInfo.CanSubmitDocuments)))
			{
				return 0;
			}

			if (indexPath.Section == 2 && indexPath.Row > 0)
			{
				return _documentsList == null || indexPath.Row > _documentsList.Count ? 0 : 64;
			}

			// Additional Transactions
			if (indexPath.Section == 3 && indexPath.Row == 0 && !_canSelectMultipleTransactions)
			{
				return 0;
			}

			if (indexPath.Section == 3 && indexPath.Row > 0)
			{
				return _additionalTransactionsList == null || indexPath.Row > _additionalTransactionsList.Count ? 0 : 64;
			}

			// Transaction Information
			if (indexPath.Section == 1 && indexPath.Row > 3)
			{
				if (_questionList == null || indexPath.Row > _questionList.Count + 3)
				{
					return 0;
				}

				GatherAnswersAndValidate();  // Used to fill the answer array;

				// Dynamic questions
				if (_questionList != null)
				{
					var item = _questionList[indexPath.Row - 4];

					if (ShouldHideDependencyRow(item))
					{
						return 0;
					}
				}

				return 64;
			}

			return base.GetHeightForRow(tableView, indexPath);
		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			var view = new UIView(new CoreGraphics.CGRect(0, 0, 320, 28));
			view.BackgroundColor = AppStyles.TableHeaderBackgroundColor;
			var label = new UILabel();
			label.BackgroundColor = UIColor.Clear;
			label.Opaque = false;
			label.TextColor = AppStyles.TableHeaderTextColor;
			label.Font = UIFont.FromName("Helvetica", 16f);
			label.Frame = new CoreGraphics.CGRect(15, 2, 290, 24);
			label.Text = string.Empty;

			if (section == 0 && _disputeType != null && _disputeType.ToUpper() == "ACH")
			{
				label.Text = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "FC6FB253-3453-4721-8A2A-E1F355F936B6", "Contact Information");
			}
			else if (section == 0)
			{
                label.Text = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "E3BF7B71-37DD-41CD-AA73-26ADADE8FB52", "Cardholder Information");				
			}

			if (section == 1)
			{				
                label.Text = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "66F253B5-5E4A-414D-8D5D-F141EDF6E3AE", "Transaction Information");
			}

			if (section == 2 && !(_transactionDisputeInfo == null || (_transactionDisputeInfo != null && !_transactionDisputeInfo.CanSubmitDocuments)))
			{
                label.Text = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "D8FF7869-BDAF-489B-82ED-33C5A5DAE917", "Supporting Documents");				
			}

			if (section == 3 && _canSelectMultipleTransactions)
			{				
                label.Text = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "66F253B5-5E4A-414D-8D5D-F141EDF6E3AE", "Additional Transactions");
			}

			if (label.Text == string.Empty)
			{
				view = new UIView(new CoreGraphics.CGRect(0, 0, 320, 0));
				view.BackgroundColor = UIColor.Clear;
			}
			else
			{
				view.AddSubview(label);
			}

			return view;
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			nfloat returnValue = 28;

			if (section == 2 && (_transactionDisputeInfo == null || (_transactionDisputeInfo != null && !_transactionDisputeInfo.CanSubmitDocuments)))
			{
				returnValue = 0;
			}

			if (section == 3 && !_canSelectMultipleTransactions)
			{
				returnValue = 0;
			}

			return returnValue;
		}

		public override nfloat GetHeightForFooter(UITableView tableView, nint section)
		{
			return 0.1f;
		}

		private async void ReviewDispute()
		{
			if (_canSelectMultipleTransactions && _additionalTransactionsList.Count < 1)
			{
                var message = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "0D5CC903-B90E-448B-B200-7DB6D813BB3B", "At least one additional transaction must be selected.");
				await AlertMethods.Alert(View, "SunMobile", message, "OK");
			}
			else
			{
				var agreement = _transactionDisputeInfo.DisputeAgreement.Replace("\\n", "\n");
                var confirmDisputeText = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "D94BBBF4-6399-418B-942D-73272BA5B9CB", "Confirm Dispute");

				var request = PopulateSubmitTransactionDisputeRequest();

				if (request.Reason == 11 || request.Reason == 12)
				{
					var alertViewController = AppDelegate.StoryBoard.InstantiateViewController("CustomAlertViewController") as CustomAlertViewController;
                    alertViewController.Header = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "D94BBBF4-6399-418B-942D-73272BA5B9CB", "Confirm Dispute");
					alertViewController.Message = agreement;					
                    alertViewController.CheckBoxText = CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "4AD5D983-969D-4899-8BC1-C1E26F3DCCC5", 
                        "I understand that selecting Submit will also close my card and a replacement card will be ordered and sent to my mailing address.");
                    alertViewController.PositiveButtonText = CultureTextProvider.SUBMIT();
                    alertViewController.NegativeButtonText = CultureTextProvider.NOREVIEW();
					alertViewController.Completed += (shouldcontinue) =>
					{
						DismissModalViewController(true);

						if (shouldcontinue)
						{
							SubmitDispute();
						}
					};

					PresentModalViewController(alertViewController, true);
				}
				else
				{
                    var response = await AlertMethods.Alert(View, confirmDisputeText, agreement, CultureTextProvider.SUBMIT(), CultureTextProvider.NOREVIEW());

                    if (response == CultureTextProvider.SUBMIT())
					{
						SubmitDispute();
					}
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
						.Where(x => x.Field.DisputeReason == txtReason.Text)
						.Select(x => x.Index)
						.DefaultIfEmpty(-1)
						.FirstOrDefault() + 1;
				}

				request.DisputeType = _disputeType;
				request.Answers = _answerList.Select(x => x.Answer).ToList();
				request.DocumentIds = _documentsList.Select(x => x.FileId).ToList();
				request.CardNumber = SelectedTransaction.CardNumber != null ? SelectedTransaction.CardNumber.Substring(SelectedTransaction.CardNumber.Length - 4) : string.Empty;
				request.Suffix = SelectedSuffix.Substring(1);
				request.CardholderName = lblCardholderName.Text;
				request.AddressLine1 = txtAddressLine1.Text;
				request.AddressLine2 = txtAddressLine2.Text;
				request.City = txtCity.Text;
				request.State = txtState.Text;
				request.UnmaskedZipCode = txtZipCode.Text.Replace("-", string.Empty);
				request.DaytimePhone = txtDaytimePhone.Text.Replace("-", string.Empty);

				request.AdditionalTransactions = new List<AdditionalTransactionInfo>();
				var currentTransactionInfo = new AdditionalTransactionInfo();
				currentTransactionInfo.Description = SelectedTransaction.Description;
				currentTransactionInfo.TransactionAmount = SelectedTransaction.TransactionAmount.ToString();
				currentTransactionInfo.TransactionDate = SelectedTransaction.TransactionDate;
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

			ShowActivityIndicator();

			var response = await methods.SubmitTransactionDispute(request, View);

			HideActivityIndicator();

			if (response?.Success == true)
			{
                await AlertMethods.Alert(View, "SunMobile", successText, "OK");
				NavigationController.PopViewController(true);
                Logging.Track("Transaction dispute successful.");
			}
			else
			{
                await AlertMethods.Alert(View, "SunMobile", failedText, "OK");
                Logging.Track("Transaction dispute failed.");
			}
		}

		private void FormatCurrencyLabel(decimal amount, UILabel label)
		{
			var amountString = StringUtilities.FormatAsCurrency(amount.ToString());
			label.Text = amountString;
			label.TextColor = amount < 0 ? UIColor.Red : UIColor.Black;
		}

		private void DisplayTransactions()
		{
			if (_additionalTransactionsList != null)
			{
				for (int i = 1; i <= _additionalTransactionsList.Count; i++)
				{
					switch (i)
					{
						case 1:
							lblATransactionDate1.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription1.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt1);
							break;
						case 2:
							lblATransactionDate2.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription2.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt2);
							break;
						case 3:
							lblATransactionDate3.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription3.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt3);
							break;
						case 4:
							lblATransactionDate4.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription4.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt4);
							break;
						case 5:
							lblATransactionDate5.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription5.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt5);
							break;
						case 6:
							lblATransactionDate6.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription6.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt6);
							break;
						case 7:
							lblATransactionDate7.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription7.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt7);
							break;
						case 8:
							lblATransactionDate8.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription8.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt8);
							break;
						case 9:
							lblATransactionDate9.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription9.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt9);
							break;
						case 10:
							lblATransactionDate10.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription10.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt10);
							break;
						case 11:
							lblATransactionDate11.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription11.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt11);
							break;
						case 12:
							lblATransactionDate12.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription12.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt12);
							break;
						case 13:
							lblATransactionDate13.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription13.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt13);
							break;
						case 14:
							lblATransactionDate14.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription14.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt14);
							break;
						case 15:
							lblATransactionDate15.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription15.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt15);
							break;
						case 16:
							lblATransactionDate16.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription16.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt16);
							break;
						case 17:
							lblATransactionDate17.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription17.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt17);
							break;
						case 18:
							lblATransactionDate18.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription18.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt18);
							break;
						case 19:
							lblATransactionDate19.Text = string.Format("{0:MM/dd/yyyy}", _additionalTransactionsList[i - 1].PostingDate.UtcToEastern());
							lblATransactionDescription19.Text = _additionalTransactionsList[i - 1].Description;
							FormatCurrencyLabel(_additionalTransactionsList[i - 1].TransactionAmount, lblATransactionAmnt19);
							break;
					}
				}
			}
		}

		private void DisplayDocuments()
		{
			if (_documentsList != null)
			{
				for (int i = 1; i <= _documentsList.Count; i++)
				{
					switch (i)
					{
					case 1:
						lblDocument1.Text = _documentsList[0].FileName;
						break;
					case 2:
						lblDocument2.Text = _documentsList[1].FileName;
						break;
					case 3:
						lblDocument3.Text = _documentsList[2].FileName;
						break;
					case 4:
						lblDocument4.Text = _documentsList[3].FileName;
						break;
					case 5:
						lblDocument5.Text = _documentsList[4].FileName;
						break;
					case 6:
						lblDocument6.Text = _documentsList[5].FileName;
						break;
					}
				}
			}

			tableMain.ReloadData();
		}

		private void SelectTransactions()
		{
			var selectTransactionsViewController = AppDelegate.StoryBoard.InstantiateViewController("TransactionSelectionViewController") as TransactionSelectionViewController;
			selectTransactionsViewController.MemberId = SelectedMemberId;
			selectTransactionsViewController.Suffix = SelectedSuffix;
			selectTransactionsViewController.AccountDescription = AccountDescription;
			selectTransactionsViewController.SelectedTransactions = _additionalTransactionsList;
			selectTransactionsViewController.MaxSelectedTransactions = MAX_TRANSACTIONS;
			selectTransactionsViewController.DisputedTransaction = SelectedTransaction;
			selectTransactionsViewController.LimitToDisputedMerchant = txtReason.Text.ToUpper() == CultureTextProvider.GetMobileResourceText("EAC13A45-7834-4BC4-90CC-A73C57DA05BC", "2DC07692-B453-42E5-BB78-D1E72D06A3FE", "CANCELLED SERVICE");

			if (_additionalTransactionsList != null)
			{
				selectTransactionsViewController.SelectedTransactions = _additionalTransactionsList;
			}

			selectTransactionsViewController.TransactionsSelected += (selectedTransactions) =>
			{
				_additionalTransactionsList = new List<Transaction>();

				_additionalTransactionsList.AddRange(selectedTransactions);
				ShowTransactions();

				NavigationController.PopViewController(true);
			};

			NavigationController.PushViewController(selectTransactionsViewController, true);
		}

		private void SelectDocuments()
		{
			var uploadDisputeDocumentsViewController = AppDelegate.StoryBoard.InstantiateViewController("UploadDisputeDocumentsTableViewController") as UploadDisputeDocumentsTableViewController;
			uploadDisputeDocumentsViewController.Completed += (fileList) =>
			{
				_documentsList = fileList;
				ShowDocuments();
			};

			NavigationController.PushViewController(uploadDisputeDocumentsViewController, true);
		}
	}
}