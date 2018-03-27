using System;
using System.Collections.Generic;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts;
using SunBlock.DataTransferObjects.Mobile.Model.CreditUnion.Memberships.Accounts.ListItems.Text;
using SunBlock.DataTransferObjects.UserInterface.MVC;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.PaymentMediums;
using SunMobile.Shared.Utilities.Dates;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Utilities.General;
using System.Linq;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.OnBase;
using System.Globalization;
using SunBlock.DataTransferObjects.DocumentCenter;
using SunBlock.DataTransferObjects.BillPay.V2;

#if __ANDROID__
using SunMobile.Droid;
using Android.App;
using SunMobile.Droid.Accounts;
#endif

namespace SunMobile.Shared.Views
{
	public static class ViewUtilities
	{
		public static TextViewTableSource ConvertTextViewModelToTextViewTableSource(TransactionListTextViewModel model, MobileStatusResponse<List<TransactionDisputeHistoryItem>> disputedItems, bool isCreditCard, string searchText)
		{
			var textViewTableSource = ConvertTextViewModelToTextViewTableSource(model, isCreditCard, searchText);

			if (disputedItems != null && disputedItems.Success && disputedItems.Result.Count > 0)
			{
				var results = textViewTableSource.Items.Where(x => !disputedItems.Result.Any(d => DateTime.ParseExact(d.DateOfTransaction, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") == x.HeaderText &&
						  d.TransactionId == x.Value4Text)).ToList();
				textViewTableSource.Items = results;
			}

			return textViewTableSource;
		}

		public static TextViewTableSource ConvertTransactionsToTextViewTableSource(List<Transaction> model, bool isCreditCard, string searchText)
		{
			var textViewTableSource = new TextViewTableSource
			{
				Items = new List<ListViewItem>()
			};

			if (model != null)
			{
				foreach (var transaction in model)
				{
					var item = new ListViewItem();

					item.HeaderText = string.Format("{0:MM/dd/yyyy}", transaction.PostingDate.UtcToEastern());
					item.MoreIconVisible = ((!string.IsNullOrEmpty(transaction.CheckNumber)) || (!string.IsNullOrEmpty(transaction.TraceNumber)) && !isCreditCard);
					item.ImageName = item.MoreIconVisible ? "transaction-check.png" : string.Empty;
					item.Data = transaction;
					item.Item1Text = transaction.Description;
					item.Value1Text = StringUtilities.StringUtilities.FormatAsCurrency(transaction.TransactionAmount.ToString());
					item.Value2Text = isCreditCard && transaction.Balance == 0 ? string.Empty : StringUtilities.StringUtilities.FormatAsCurrency(transaction.Balance.ToString());
					item.Item5Text = isCreditCard ? "CreditCard" : string.Empty;
					item.Value4Text = transaction.SequenceNumber.ToString();
                    item.Bool1Value = transaction.IsPending;

					if (!string.IsNullOrEmpty(searchText))
					{
						if (item.HeaderText.ToUpper().Contains(searchText.ToUpper()) || item.Item1Text.ToUpper().Contains(searchText.ToUpper()) || item.Value1Text.ToUpper().Contains(searchText.ToUpper()))
						{
							textViewTableSource.Items.Add(item);
						}
					}
					else
					{
						textViewTableSource.Items.Add(item);
					}
				}
			}

			return textViewTableSource;
		}

		public static TextViewTableSource ConvertTextViewModelToTextViewTableSource(TransactionListTextViewModel model, bool isCreditCard, string searchText)
		{
			var textViewTableSource = new TextViewTableSource
			{
				Items = new List<ListViewItem>()
			};

			if (model != null)
			{
				foreach (var headerSection in model.TransactionSections)
				{
					foreach (var itemSection in headerSection.Items)
					{
						var item = new ListViewItem();

						var transaction = itemSection.Data;

						item.HeaderText = string.Format("{0:MM/dd/yyyy}", transaction.PostingDate.UtcToEastern());
						item.MoreIconVisible = ((!string.IsNullOrEmpty(transaction.CheckNumber)) || (!string.IsNullOrEmpty(transaction.TraceNumber)) && !isCreditCard);
						item.ImageName = item.MoreIconVisible ? "transaction-check.png" : string.Empty;
						item.Data = itemSection.Data;
						item.Item1Text = transaction.Description;
						item.Value1Text = StringUtilities.StringUtilities.FormatAsCurrency(transaction.TransactionAmount.ToString());
						item.Value2Text = isCreditCard && transaction.Balance == 0 ? string.Empty : StringUtilities.StringUtilities.FormatAsCurrency(transaction.Balance.ToString());
						item.Item5Text = isCreditCard ? "CreditCard" : string.Empty;
						item.Value4Text = transaction.SequenceNumber.ToString();

						if (!string.IsNullOrEmpty(searchText))
						{
							if (item.HeaderText.ToUpper().Contains(searchText.ToUpper()) || item.Item1Text.ToUpper().Contains(searchText.ToUpper()) || item.Value1Text.ToUpper().Contains(searchText.ToUpper()))
							{
								textViewTableSource.Items.Add(item);
							}
						}
						else
						{
							textViewTableSource.Items.Add(item);
						}
					}
				}
			}

			return textViewTableSource;
		}

		public static TextViewTableSource ConvertCardListTextViewTableSource(List<BankCard> model)
		{
			var textViewTableSource = new TextViewTableSource
			{
				Items = new List<ListViewItem>()
			};

			if (model != null)
			{
				foreach (var bankcard in model)
				{
					var item = new ListViewItem();

					item.HeaderText = bankcard.DisplayName;
					item.Header2Text = bankcard.CardAccountNumber;
					item.MoreIconVisible = false;
					item.Data = bankcard;

					textViewTableSource.Items.Add(item);
				}
			}

			return textViewTableSource;
		}

		public static List<ListViewItem> ConvertCardListViewItems(List<BankCard> model)
		{
			var items = new List<ListViewItem>();

			if (model != null)
			{
				foreach (var bankcard in model)
				{
					var item = new ListViewItem();

					switch (bankcard.CardType)
					{
						case CardTypes.CreditCard:
							item.Item1Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "17783198-589D-44DC-A3F8-40A0E32522B9", "Credit Card");
							break;
						case CardTypes.ProprietaryCard:
							item.Item1Text = CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "F02EAA6F-CB04-491A-8252-440DBAF9F856", "ATM Card");
							break;
						case CardTypes.VisaDebitCard:
							item.Item1Text = "Visa " + CultureTextProvider.GetMobileResourceText("408B726E-56B9-420D-B97A-47F3B8506420", "F74D32D8-EFCA-4798-BF46-05175CFACE02", "Check Card");
							break;
					}

					if (!string.IsNullOrWhiteSpace(bankcard.CardAccountNumber) && bankcard.CardAccountNumber.Length >= 4)
					{
						item.Item2Text = "xxxx-xxxx-xxxx-" + bankcard.CardAccountNumber.Substring(bankcard.CardAccountNumber.Length - 4, 4);
					}
					item.MoreIconVisible = false;
					item.Data = bankcard;

					items.Add(item);
				}
			}

			return items;
		}

		public static TextViewTableSource ConvertPaymentListTextViewTableSource(List<Payment> model, bool isPending)
		{
			var textViewTableSource = new TextViewTableSource
			{
				Items = new List<ListViewItem>()
			};

			if (model != null)
			{
				foreach (var payment in model)
				{
					var item = new ListViewItem();

                    item.HeaderText = payment.PayeeName;

                    if (!string.IsNullOrEmpty(payment.PayeeAlias))
                    {
                        item.HeaderText += $" ({payment.PayeeAlias})";
                    }

					item.MoreIconVisible = true;
					item.Data = payment;
					item.Item1Text = string.Empty;
					item.Item2Text = string.Empty;
					item.Value1Text = StringUtilities.StringUtilities.FormatAsCurrency(payment.Amount.ToString());
                    item.Value3Text = payment.Status;
                    item.Value4Text = payment.StatusCode.ToString();

                    if (isPending)
                    {
                        item.Value2Text = payment.SendOn.UtcToEastern().GetFiendlyFullDate();
                    }
                    else
                    {
                        item.Value2Text = payment.HistoricalDate.UtcToEastern().GetFiendlyFullDate();
                    }

					textViewTableSource.Items.Add(item);
				}
			}

			return textViewTableSource;
		}
		
		public static TextViewTableSource ConvertPayeeV2ListTextViewTableSource(List<Payee> model, bool isActive)
		{
			var textViewTableSource = new TextViewTableSource
			{
				Items = new List<ListViewItem>()
			};

			if (model != null)
			{
				foreach (var payee in model)
				{
					var item = new ListViewItem();

					item.HeaderText = payee.PayeeName;
					item.Header2Text = payee.PayeeAlias;
					item.MoreIconVisible = true;
					item.Data = payee;
					item.Item1Text = string.Empty;
					item.Value1Text = payee.PayeeAccountNumber;

					if (payee.Active == isActive)
					{
						textViewTableSource.Items.Add(item);
					}
				}
			}

			return textViewTableSource;
		}

        public static List<Account> ConvertTextViewModelToAccountList(AccountListTextViewModel model, bool includeJoints = false)
        {
            var returnValue = new List<Account>();

            foreach (var headerSection in model.AccountSections)
            {
                foreach (var itemSection in headerSection.Items)
                {
                    var item = GetAccountListViewItem(headerSection, itemSection, includeJoints);

                    if (item != null)
                    {
                        returnValue.Add((Account)item.Data);
                    }
                }
            }

            return returnValue;
        }

		public static GroupedTextViewTableSource ConvertTextViewModelToGroupedTextViewTableSource(AccountListTextViewModel model, bool includeJoints, bool includeRocketAccounts = false)
		{
			var textViewTableSource = new GroupedTextViewTableSource();
			textViewTableSource.Sections = new Dictionary<string, TableSection>();
			textViewTableSource.SectionTitles = new List<string>();

			int sectionIndex = 0;
			string lastSectionName = string.Empty;

			foreach (var headerSection in model.AccountSections)
			{
				string sectionName;

				switch (headerSection.HeaderText)
				{
					case "Shares":
						sectionName = "Deposits";
						break;
					default:
						sectionName = headerSection.HeaderText;
						break;
				}

				foreach (var itemSection in headerSection.Items)
				{
					var item = GetAccountListViewItem(headerSection, itemSection, includeJoints);

					if (item != null)
					{
						if (!textViewTableSource.Sections.ContainsKey(sectionName))
						{
							var tableSection = new TableSection
							{
								Index = sectionIndex,
								SectionName = sectionName,
								ItemCount = 0,
								ListViewItems = new List<ListViewItem>()
							};

							textViewTableSource.Sections.Add(sectionName, tableSection);
							textViewTableSource.SectionTitles.Add(sectionName);
						}

						// Increment SectionTotals and add Items
						textViewTableSource.Sections[sectionName].ItemCount++;
						textViewTableSource.Sections[sectionName].ListViewItems.Add(item);

						lastSectionName = sectionName;
					}
				}

				sectionIndex++;
			}

			return textViewTableSource;
		}

		public static Dictionary<string, List<ListViewItem>> ConvertTextViewModelToGroupedListView(AccountListTextViewModel model)
		{
			var results = new Dictionary<string, List<ListViewItem>>();
			var listViewItems = new List<ListViewItem>();

			foreach (var headerSection in model.AccountSections)
			{
				foreach (var itemSection in headerSection.Items)
				{
					var item = new ListViewItem();

					item.GroupHeaderText = headerSection.HeaderText;
					item.HeaderText = itemSection.ItemDescription;
					item.MoreIconVisible = true;
					item.Data = itemSection.Data;
					item.Item1Text = string.Empty;

					for (int i = 0; i < itemSection.ListItems.DisplayFieldNames.Count; i++)
					{
						switch (i)
						{
							case 0:
								item.Item1Text = itemSection.ListItems.DisplayFieldNames[i];
								item.Value1Text = itemSection.ListItems.DisplayFieldValues[i];
								break;
							case 1:
								item.Item2Text = itemSection.ListItems.DisplayFieldNames[i];
								item.Value2Text = itemSection.ListItems.DisplayFieldValues[i];
								break;
						}
					}

					listViewItems.Add(item);
				}
			}

			var items = new List<ListViewItem>();
			string currentHeader = null;

			foreach (var item in listViewItems)
			{
				if (item.GroupHeaderText != currentHeader)
				{
					items = new List<ListViewItem>();
					currentHeader = item.GroupHeaderText;
					results.Add(currentHeader, items);
				}

				items.Add(item);
			}

			return results;
		}

		public static List<ListViewItem> ConvertListImageDocumentToListViews(List<ImageDocument> model)
		{
			var items = new List<ListViewItem>();

			if (model != null)
			{
				foreach (var document in model)
				{
					var item = new ListViewItem();

                    var documentName = document.DocumentName.Trim();					
					item.Data = document;

					//If the document name contains the "-", separate the date from the name
                    if (!string.IsNullOrEmpty(documentName) && documentName.Length > 10 && documentName.Contains("-"))
					{
                        var index = documentName.LastIndexOf('-');
                        var dateTextLine = documentName.Substring(index + 1);
                        var nameTextLine = documentName.Substring(0, index).Trim();
                        item.Item1Text = dateTextLine.Trim();
						item.Item2Text = nameTextLine;
					}
                    else
                    {
                        item.Item2Text = documentName;
                    }

					items.Add(item);
				}
			}

			return items;
		}

		public static string GetImageNameFromAccountType(string accountType)
		{
			string returnValue;

			switch (accountType.ToUpper())
			{
				case "SHARES":
					returnValue = "account_savings.png";
					break;
				case "LOANS":
					returnValue = "account_loan.png";
					break;
				case "CREDITCARDS":
					returnValue = "account_creditcard.png";
					break;
				case "MORTGAGES":
					returnValue = "account_loan.png";
					break;
				case "SHARECERTIFICATES":
					returnValue = "account_certificate.png";
					break;
				case "IRAS":
					returnValue = "account_loan.png";
					break;
				case "SHAREDRAFTS":
					returnValue = "account_checking.png";
					break;
				case "CERTIFICATES":
					returnValue = "account_certificate.png";
					break;
				case "MONEYMARKETS":
					returnValue = "account_moneymarket.png";
					break;
				case "CLUBS":
					returnValue = "account_checking.png";
					break;
				default:
					returnValue = "account_general.png";
					break;
			}

			return returnValue;
		}

		public static string GetColorFromAccountType(string accountType)
		{
			string returnValue;

			switch (accountType.ToUpper())
			{
				case "SHARES":
					returnValue = "78c143";
					break;
				case "LOANS":
					returnValue = "06afce";
					break;
				case "CREDITCARDS":
					returnValue = "f78e1e";
					break;
				case "MORTGAGES":
					returnValue = "06afce";
					break;
				case "SHARECERTIFICATES":
					returnValue = "06afce";
					break;
				case "IRAS":
					returnValue = "06afce";
					break;
				case "SHAREDRAFTS":
					returnValue = "78c143";
					break;
				case "CERTIFICATES":
					returnValue = "06afce";
					break;
				case "MONEYMARKETS":
					returnValue = "78c143";
					break;
				case "CLUBS":
					returnValue = "78c143";
					break;
				default:
					returnValue = "78c143";
					break;
			}

			return returnValue;
		}

		public static List<ListViewItem> GetSubAccountsDisclosures(bool isEnrolledInEStatements)
		{
			var disclosureItems = new List<ListViewItem>();

			var item1 = new ListViewItem
			{
				Item1Text = "Checking Account Features",
				Item2Text = "http://sunblockstorage.blob.core.windows.net/documents/CheckingAccountApplication.pdf"
			};
			disclosureItems.Add(item1);

			var item2 = new ListViewItem
			{
				Item1Text = "Account Agreement and Disclosure ",
				Item2Text = "https://www.suncoastcreditunion.com/-/media/files/forms-and-applications/suncoast-account-agreement.ashx"
			};
			disclosureItems.Add(item2);

			var item3 = new ListViewItem
			{
				Item1Text = "Discretionary Overdraft Privilege Policy ",
				Item2Text = "http://sunblockstorage.blob.core.windows.net/documents/DiscretionaryOverdraftPrivilegePolicy.pdf"
			};
			disclosureItems.Add(item3);

			var item4 = new ListViewItem
			{
				Item1Text = "Share Account Rate Schedule",
				Item2Text = "http://sunblockstorage.blob.core.windows.net/documents/ShareRateSchedule%202017.pdf"
			};
			disclosureItems.Add(item4);

			var item5 = new ListViewItem
			{
				Item1Text = "Fee Schedule",
				Item2Text = "http://sunblockstorage.blob.core.windows.net/documents/FeeSchedule.pdf"
			};
			disclosureItems.Add(item5);

			if (!isEnrolledInEStatements)
			{
				var item6 = new ListViewItem
				{
					Item1Text = "eStatements Disclosure",
					Item2Text = "https://sunblockstorage.blob.core.windows.net/documents/eStatement-Disclosure.pdf"
				};
				disclosureItems.Add(item6);
			}

			var item7 = new ListViewItem
			{
				Item1Text = "Signature Card Disclosure",
				Item2Text = "https://sunblockstorage.blob.core.windows.net/documents/SignatureCardDisclosure.pdf"
			};
			disclosureItems.Add(item7);

			var item8 = new ListViewItem
			{
				Item1Text = "Tax Disclosure",
				Item2Text = "https://sunblockstorage.blob.core.windows.net/documents/InstantCheckingTaxDisclosure.pdf"
			};
			disclosureItems.Add(item8);

			return disclosureItems;
		}

		public static List<ListViewItem> GetSubAccountsNextStepsDocuments(List<ImageDocument> documents)
		{
			var returnValue  = ConvertListImageDocumentToListViews(documents);

            foreach (var item in returnValue)
            {
                item.Item1Text = item.Item2Text;
            }

			// Add Direct Deposit Forms
			var documentCenterFile1 = new DocumentCenterFile();
			documentCenterFile1.URL = "https://sunblockstorage.blob.core.windows.net/documents/DirectDepositBrochure403.pdf";
			var documentListViewItem1 = new ListViewItem();
			documentListViewItem1.Item1Text = documentListViewItem1.Item2Text = "Direct Deposit Form";
			documentListViewItem1.Data = documentCenterFile1;
			returnValue.Add(documentListViewItem1);

			var documentCenterFile2 = new DocumentCenterFile();
			documentCenterFile2.URL = "https://sunblockstorage.blob.core.windows.net/documents/StateOfFloridaEmployeeDirectDepositDFSA126S.pdf";
			var documentListViewItem2 = new ListViewItem();
			documentListViewItem2.Item1Text = documentListViewItem2.Item2Text = "Florida Employee Direct Deposit Form";
			documentListViewItem2.Data = documentCenterFile2;
			returnValue.Add(documentListViewItem2);

			var documentCenterFile3 = new DocumentCenterFile();
			documentCenterFile3.URL = "https://sunblockstorage.blob.core.windows.net/documents/StateOfFloridaRetireeDirectDepositDFSA126R.pdf";
			var documentListViewItem3 = new ListViewItem();
			documentListViewItem3.Item1Text = documentListViewItem3.Item2Text = "Florida Retiree Direct Deposit Form";
			documentListViewItem3.Data = documentCenterFile3;
			returnValue.Add(documentListViewItem3);

            return returnValue;
		}

#if __ANDROID__
		public static AccountListItemAdapter ConvertTextViewModelToGroupedListItemAdapter(Activity activity, AccountListTextViewModel model, bool includeJoints, bool includeRocketAccounts = false)
		{
			var groupedAdapter = new AccountListItemAdapter(activity);

			try
			{
				var lastSection = model.AccountSections.Last();

				foreach (var headerSection in model.AccountSections)
				{
					try
					{
						string sectionName = headerSection.HeaderText;

						if (headerSection.HeaderText == "Shares")
						{
							sectionName = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E88AAE40-7473-4810-A25B-C2C40A0928A2", "Deposits");
						}
						else if (headerSection.HeaderText == "Loans")
						{
							sectionName = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "8F7B796C-9009-49B3-BC91-9F14CADAA936", "Loans");
						}
						else if (headerSection.HeaderText == "Credit Cards")
						{
							sectionName = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "944713ED-0D8C-4997-8FAD-4B8D45BB10F0", "Credit Cards");
						}

						var items = new List<ListViewItem>();

						foreach (var itemSection in headerSection.Items)
						{
							var item = GetAccountListViewItem(headerSection, itemSection, includeJoints);

							if (item != null)
							{
								items.Add(item);
							}
						}

                        /*
                        if (sectionName == "Deposits" && includeRocketAccounts)
						{
							var item = new ListViewItem();
							item.HeaderText = string.Empty;
							item.Header2Text = "Open a New Checking Account";
							item.MoreIconVisible = true;
							item.Item1Text = string.Empty;
							item.Item2Text = string.Empty;
							item.Item3Text = string.Empty;
							item.Item4Text = string.Empty;
							item.Item5Text = string.Empty;
							item.Value1Text = string.Empty;
							item.Value2Text = string.Empty;
							item.Value3Text = string.Empty;
							item.Value4Text = string.Empty;
							item.Data = new Account
							{
								OwnershipType = "Primary",
								AccountType = "RocketAccount"
							};

							items.Add(item);
						}
                        */

						AddGroupList(activity, groupedAdapter, items, sectionName);
					}
					catch (Exception ex)
					{
						Logging.Logging.Log(ex, "ViewUtilities:ConvertTextViewModelToGroupedListItemAdapter:foreach");
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "ViewUtilities:ConvertTextViewModelToGroupedListItemAdapter");
			}

			return groupedAdapter;
		}

		public static List<ListViewItem> ConvertTransactionsToListViewItems(List<Transaction> model, bool isCreditCard, string searchText)
		{
			var items = new List<ListViewItem>();

			try
			{
				foreach (var transaction in model)
				{
					try
					{
						var item = new ListViewItem();

						item.HeaderText = string.Format("{0:MM/dd/yyyy}", transaction.PostingDate.UtcToEastern());
						item.MoreIconVisible = ((!string.IsNullOrEmpty(transaction.CheckNumber)) || (!string.IsNullOrEmpty(transaction.TraceNumber)) && !isCreditCard);
						item.Data = transaction;
						item.Item1Text = transaction.Description;
						item.Value1Text = StringUtilities.StringUtilities.FormatAsCurrency(transaction.TransactionAmount.ToString());
						item.Value2Text = isCreditCard && transaction.Balance == 0 ? string.Empty : StringUtilities.StringUtilities.FormatAsCurrency(transaction.Balance.ToString());
						item.Item5Text = isCreditCard ? "CreditCard" : string.Empty;
						item.Value4Text = transaction.SequenceNumber.ToString();
                        item.Bool1Value = transaction.IsPending;

						if (!string.IsNullOrEmpty(searchText))
						{
							if (item.HeaderText.ToUpper().Contains(searchText.ToUpper()) || item.Item1Text.ToUpper().Contains(searchText.ToUpper()) || item.Value1Text.ToUpper().Contains(searchText.ToUpper()))
							{
								items.Add(item);
							}
						}
						else
						{
							items.Add(item);
						}
					}
					catch (Exception ex)
					{
						Logging.Logging.Log(ex, "ViewUtilities:ConvertTransactionsToListViewItems:foreach");
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "ViewUtilities:ConvertTransactionsToListViewItems");
			}

			return items;
		}

		public static List<ListViewItem> ConvertTextViewModelToListViewItems(TransactionListTextViewModel model, MobileStatusResponse<List<TransactionDisputeHistoryItem>> disputedItems, bool isCreditCard, string searchText)
		{
			var items = new List<ListViewItem>();

			try
			{
				items = ConvertTextViewModelToListViewItems(model, isCreditCard, searchText);

				if (disputedItems != null && disputedItems.Success && disputedItems.Result.Count > 0)
				{
					var results = items.Where(x => !disputedItems.Result.Any(d => DateTime.ParseExact(d.DateOfTransaction, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") == x.HeaderText &&
						  d.TransactionId == x.Value4Text)).ToList();
					items = results;
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "ViewUtilities:ConvertTextViewModelToListViewItems");
			}

			return items;
		}

		public static List<ListViewItem> ConvertTextViewModelToListViewItems(TransactionListTextViewModel model, bool isCreditCard, string searchText)
		{
			var items = new List<ListViewItem>();

			try
			{
				foreach (var headerSection in model.TransactionSections)
				{
					foreach (var itemSection in headerSection.Items)
					{
						try
						{
							var item = new ListViewItem();

							var transaction = itemSection.Data;

							item.HeaderText = string.Format("{0:MM/dd/yyyy}", transaction.PostingDate.UtcToEastern());
							item.MoreIconVisible = ((!string.IsNullOrEmpty(transaction.CheckNumber)) || (!string.IsNullOrEmpty(transaction.TraceNumber)) && !isCreditCard);
							item.Data = itemSection.Data;
							item.Item1Text = transaction.Description;
							item.Value1Text = StringUtilities.StringUtilities.FormatAsCurrency(transaction.TransactionAmount.ToString());
							item.Value2Text = isCreditCard && transaction.Balance == 0 ? string.Empty : StringUtilities.StringUtilities.FormatAsCurrency(transaction.Balance.ToString());
							item.Item5Text = isCreditCard ? "CreditCard" : string.Empty;
							item.Value4Text = transaction.SequenceNumber.ToString();

							if (!string.IsNullOrEmpty(searchText))
							{
								if (item.HeaderText.ToUpper().Contains(searchText.ToUpper()) || item.Item1Text.ToUpper().Contains(searchText.ToUpper()) || item.Value1Text.ToUpper().Contains(searchText.ToUpper()))
								{
									items.Add(item);
								}
							}
							else
							{
								items.Add(item);
							}
						}
						catch (Exception ex)
						{
							Logging.Logging.Log(ex, "ViewUtilities:ConvertTextViewModelToListViewItems:foreach");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "ViewUtilities:ConvertTextViewModelToListViewItems");
			}

			return items;
		}

		public static List<ListViewItem> ConvertPaymentsToListViewItems(List<Payment> model, bool isPending)
		{
			var items = new List<ListViewItem>();

			if (model != null)
			{
				foreach (var payment in model)
				{
					var item = new ListViewItem();

                    if (isPending)
                    {
                        item.HeaderText = payment.SendOn.UtcToEastern().GetFiendlyFullDate();
                    }
                    else
                    {
                        item.HeaderText = payment.HistoricalDate.UtcToEastern().GetFiendlyFullDate();
                    }

					item.Item1Text = payment.PayeeName;

                    if (!string.IsNullOrEmpty(payment.PayeeAlias))
                    {
                        item.Item1Text += $" ({payment.PayeeAlias})";
                    }

					item.Value1Text = string.Empty;
                    item.Item2Text = payment.Status;
					item.Value2Text = StringUtilities.StringUtilities.FormatAsCurrency(payment.Amount.ToString());
                    item.Value3Text = payment.Status;
                    item.Value4Text = payment.StatusCode.ToString();

					item.MoreIconVisible = true;
					item.Data = payment;

					items.Add(item);
				}
			}

			return items;
		}

		public static List<ListViewItem> ConvertDocumentUploadsToListViewItems(List<List<DocumentUpload>> model, List<string> sectionHeaders)
		{
			var headerCount = 0;
			var items = new List<ListViewItem>();

			if (model != null)
			{
				foreach (List<DocumentUpload> documentList in model)
				{
					if (documentList.Count > 0)
					{
						var item = new ListViewItem();

						item.HeaderText = sectionHeaders[headerCount];
						item.Bool1Value = true;

						items.Add(item);
						headerCount++;
					}

					foreach (var document in documentList)
					{
						var item = new ListViewItem();

						item.HeaderText = string.Format("{0:MM/dd/yyyy}", document.DocumentTimeUtc);
						item.Item1Text = document.Title.Trim();
						item.Item2Text = document.StatusType;
						item.Item3Text = document.StatusDescription;

						item.MoreIconVisible |= (document.StatusType == DocumentUploadStatusTypes.Rejected.ToString() || document.StatusType == DocumentUploadStatusTypes.Requested.ToString());
						item.Data = document;

						items.Add(item);
					}
				}
			}

			return items;
		}

		public static List<ListViewItem> ConvertDocumentDownloadsToListViewItems(List<DocumentDownload> model, string headerText)
		{
			var items = new List<ListViewItem>();

			if (model != null)
			{
				var header = new ListViewItem();

				header.HeaderText = headerText;
				header.Bool1Value = true;

				items.Add(header);

				foreach (var document in model)
				{
					var item = new ListViewItem();

					item.HeaderText = string.Format("{0:MM/dd/yyyy}", document.DocumentTimeUtc);
					item.Item1Text = document.Title.Trim();
					item.Item2Text = document.ViewStatus;

					item.MoreIconVisible = true;
					item.Data = document;

					items.Add(item);
				}
			}

			return items;
		}

		private static void AddGroupList(Activity activity, AccountListItemAdapter groupedAdapter, List<ListViewItem> items, string groupText)
		{
			int[] resourceIds = { Resource.Id.lblHeaderText, Resource.Id.lblHeader2Text, Resource.Id.lblItem1Text, Resource.Id.lblItem2Text, Resource.Id.lblItem3Text, Resource.Id.lblItem4Text, Resource.Id.lblItem5Text,
				Resource.Id.lblValue1Text, Resource.Id.lblValue2Text, Resource.Id.lblValue3Text, Resource.Id.lblValue4Text };
			string[] fields = { "HeaderText", "Header2Text", "Item1Text", "Item2Text", "Item3Text", "Item4Text", "Item5Text", "Value1Text", "Value2Text", "Value3Text", "Value4Text" };
			var listAdapter = new AccountListAdapter(activity, items, resourceIds, fields);
			groupedAdapter.AddSection(groupText, listAdapter);
		}

#endif

		public static ListViewItem GetAccountListViewItem(HeaderSectionTextView<Account> headerSection, ItemSectionTextView<Account> itemSection, bool includeJoints)
		{
			var item = new ListViewItem();

			try
			{
				var account = itemSection.Data;

				item.MoreIconVisible = true;
				item.Data = account;
				item.ImageName = GetImageNameFromAccountType(account.AccountType);


				if ((account.MemberId == GeneralUtilities.GetMemberIdAsInt() || account.OwnershipType == "Secondary") || (account.MemberId != GeneralUtilities.GetMemberIdAsInt() && includeJoints))
				{
					item.HeaderText = account.Description;

					if (account.OwnershipType == "Joint")
					{
						item.HeaderText += " (" + account.OwnerName + ")";
					}

					if (headerSection.HeaderText == "Credit Cards" && account.Suffix.Length > 12)
					{
						item.Header2Text = "XXXX-" + account.Suffix.Substring(12);
					}
					else
					{
						item.Header2Text = account.MemberId + "-" + account.Suffix;
					}

					item.Item1Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "805D4ECB-244C-4E98-B063-CC32706086DA", "Balance") + ":";
					item.Item2Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "30EF1F3D-3F26-4B82-83C6-1A278F302D5A", "Available Balance") + ":";
					item.Value1Text = string.Format(new CultureInfo("en-US"), "{0:C}", account.Balance);
					item.Value2Text = string.Format(new CultureInfo("en-US"), "{0:C}", account.AvailableBalance);

					if (headerSection.HeaderText == "Credit Cards" || headerSection.HeaderText == "Loans")
					{
						item.Item3Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "E2A598E2-E43C-4A7A-A4C9-A9A40D25DA64", "Minimum Payment") + ":";
						item.Item4Text = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "DD5DE072-7605-44C5-9C0B-4DD5389FD16A", "Next Due Date") + ":";
						item.Item5Text = string.Empty;
						item.Value3Text = string.Format(new CultureInfo("en-US"), "{0:C}", account.PaymentAmount);
						item.Value4Text = string.Format("{0:MM/dd/yyyy}", account.PaymentDueDate.UtcToEastern());
					}
					else
					{
						item.Item3Text = string.Empty;
						item.Item4Text = string.Empty;
						item.Item5Text = string.Empty;
						item.Value3Text = string.Empty;
						item.Value4Text = string.Empty;
					}

					if (!account.IsAllowedForTransferSource)
					{
						// Hide available Balance in 2 and move everything up
						item.Item2Text = item.Item3Text;
						item.Value2Text = item.Value3Text;
						item.Item3Text = item.Item4Text;
						item.Value3Text = item.Value4Text;
						item.Item4Text = string.Empty;
						item.Value4Text = string.Empty;
					}
				}
				else
				{
					item = null;
				}

			}
			catch (Exception ex)
			{
				item = null;
				Logging.Logging.Log(ex, "ViewUtilities:GetAccountListViewItem");
			}

			return item;
		}
	}
}