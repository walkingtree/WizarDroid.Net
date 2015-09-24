using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using WizarDroid.NET;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET_Sample.Wizards
{
    public class CustomerWizardStep3 : WizardStep
    {
        [WizardState]
        public Customer Cust;

        public CustomerWizardStep3()
        {
            StepExited += OnStepExited;
        }

        EditText AddrLine1; EditText AddrLine2; EditText AddrCity; EditText AddrState; EditText AddrZipCode; CheckBox UseBillCheck;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.CustomerWizardStep3, container, false);

            AddrLine1 = view.FindViewById<EditText>(Resource.Id.shipLine1);
            AddrLine2 = view.FindViewById<EditText>(Resource.Id.shipLine2);
            AddrCity = view.FindViewById<EditText>(Resource.Id.shipCity);
            AddrState = view.FindViewById<EditText>(Resource.Id.shipState);
            AddrZipCode = view.FindViewById<EditText>(Resource.Id.shipZip);
            UseBillCheck = view.FindViewById<CheckBox>(Resource.Id.chkUseBill);

            AddrLine1.AfterTextChanged += delegate { Validate(); };
            AddrCity.AfterTextChanged += delegate { Validate(); };
            AddrState.AfterTextChanged += delegate { Validate(); };
            AddrZipCode.AfterTextChanged += delegate { Validate(); };

            UseBillCheck.CheckedChange += OnUseBillingAddress;
            return view;
        }

        void OnUseBillingAddress(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            AddrLine1.Text = e.IsChecked ? Cust.BillingAddress.Line1 : "";
            AddrLine2.Text = e.IsChecked ? Cust.BillingAddress.Line2 : "";
            AddrCity.Text = e.IsChecked ? Cust.BillingAddress.City: "";
            AddrState.Text = e.IsChecked ? Cust.BillingAddress.State: "";
            AddrZipCode.Text = e.IsChecked ? Cust.BillingAddress.ZipCode: "";

            AddrLine1.Enabled = !e.IsChecked;
            AddrLine2.Enabled = !e.IsChecked;
            AddrCity.Enabled = !e.IsChecked;
            AddrState.Enabled = !e.IsChecked;
            AddrZipCode.Enabled = !e.IsChecked;

            if (e.IsChecked)
                NotifyCompleted();
            else
                NotifyIncomplete();
        }


        private void Validate()
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(AddrLine1.Text) || AddrLine1.Text.Length < 3) {
                AddrLine1.Error = "Line 1 is required";
                valid = false;
            }
            else {
                AddrLine1.Error = null;
            }

            if (string.IsNullOrWhiteSpace(AddrCity.Text) || AddrCity.Text.Length < 3) {
                AddrCity.Error = "City is required";
                valid = false;
            }
            else {
                AddrCity.Error = null;
            }

            if (string.IsNullOrWhiteSpace(AddrZipCode.Text) || AddrZipCode.Text.Length < 5) {
                AddrZipCode.Error = "Invalid zip code";
                valid = false;
            }
            else {
                AddrZipCode.Error = null;
            }

            if (valid)
                NotifyCompleted(); // All the input is valid.. Set the step as completed
            else
                NotifyIncomplete();
        }

        void OnStepExited(StepExitCode exitCode)
        {
            if (exitCode == StepExitCode.ExitPrevious) return;

            if (Cust == null) throw new ArgumentNullException("Customer object is required");

            Cust.ShippingAddress = new Address
            {
                Line1 = AddrLine1.Text,
                Line2 = AddrLine2.Text,
                City = AddrCity.Text,
                State = AddrState.Text,
                ZipCode = AddrZipCode.Text
            };
        }
    }
}