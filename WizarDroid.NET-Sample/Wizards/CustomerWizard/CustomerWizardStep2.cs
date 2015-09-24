using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using WizarDroid.NET;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET_Sample.Wizards
{
    public class Step2 : WizardStep
    {
        [WizardState]
        public Customer Cust;

        public object LockObj = new object();
        public bool StepCompleted = false;


        public Step2()
        {
            StepExited += OnStepExited;
        }

        EditText AddrLine1; EditText AddrLine2; EditText AddrCity; EditText AddrState; EditText AddrZipCode;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.CustomerWizardStep2, container, false);

            AddrLine1 = view.FindViewById<EditText>(Resource.Id.billLine1);
            AddrLine2 = view.FindViewById<EditText>(Resource.Id.billLine2);
            AddrCity = view.FindViewById<EditText>(Resource.Id.billCity);
            AddrState = view.FindViewById<EditText>(Resource.Id.billState);
            AddrZipCode = view.FindViewById<EditText>(Resource.Id.billZip);

            AddrLine1.AfterTextChanged += delegate { Validate(); };
            AddrCity.AfterTextChanged += delegate { Validate(); };
            AddrState.AfterTextChanged += delegate { Validate(); };
            AddrZipCode.AfterTextChanged += delegate { Validate(); };

            return view;
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

            lock (LockObj) {

                if (valid && StepCompleted == false) {
                    NotifyCompleted(); // All the input is valid.. Set the step as completed
                    StepCompleted = true;
                }
                else if (!valid && StepCompleted == true) {
                    NotifyIncomplete();
                    StepCompleted = false;
                }
            }

        }

        void OnStepExited(StepExitCode exitCode)
        {
            if (exitCode == StepExitCode.ExitPrevious) return;

            if (Cust == null) throw new ArgumentNullException("Customer object is required");

            Cust.BillingAddress = new Address
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