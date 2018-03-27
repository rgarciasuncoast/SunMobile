using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunMobile.Droid.ExternalServices;
using SunMobile.Shared.Data;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Navigation;

namespace SunMobile.Droid.LoanCenter
{
    public class LoanCenterMenuFragment : BaseFragment
    {
        private TableRow tableRowApplyForALoan;
        private TableRow tableRowBuyACar;
        private TableRow tableRowBuyAHome;

        private TextView lblApplyForALoan;
        private TextView lblBuyACar;
        private TextView lblBuyAHome;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);



            SetupView();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((MainActivity)Activity).SetActionBarTitle("Loan Center");
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.LoanCenterMenuView, null);
            RetainInstance = true;

            return view;
        }

        public override void SetupView()
        {
            base.SetupView();

            tableRowApplyForALoan = Activity.FindViewById<TableRow>(Resource.Id.rowApplyForALoan);
            tableRowApplyForALoan.Click += (sender, e) => ListItemClicked(0);
            tableRowBuyACar = Activity.FindViewById<TableRow>(Resource.Id.rowBuyACar);
            tableRowBuyACar.Click += (sender, e) => ListItemClicked(1);
            tableRowBuyAHome = Activity.FindViewById<TableRow>(Resource.Id.rowBuyAHome);
            tableRowBuyAHome.Click += (sender, e) => ListItemClicked(2);

            lblApplyForALoan = Activity.FindViewById<TextView>(Resource.Id.lblApplyForALoan);
            lblBuyACar = Activity.FindViewById<TextView>(Resource.Id.lblBuyACar);
            lblBuyAHome = Activity.FindViewById<TextView>(Resource.Id.lblBuyAHome);

        }

        public void ListItemClicked(int position)
        {
            LoanCenterFragment fragment = null;

            try
            {
                switch (position)
                {
                    case 0:
                        fragment = new LoanCenterFragment();
                        fragment.LoanType = LoanCenterTypes.ApplyForLoan;
                        break;

                    case 1:
                        fragment = new LoanCenterFragment();
                        fragment.LoanType = LoanCenterTypes.CarLoan;
                        break;

                    case 2:
                        fragment = new LoanCenterFragment();
                        fragment.LoanType = LoanCenterTypes.HomeLoan;
                        break;
                }

                if (fragment != null)
                {
                    NavigationService.NavigatePush(fragment, true, false);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "LoanCenterMenuFragment:RowSelected");
            }
        }
    }
}