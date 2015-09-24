using System.Globalization;
using Android.Widget;
using WizarDroid.NET;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET_Sample.Wizards
{
    public class CustomerWizardStep1 : WizardStep
    {
        [WizardState]
        public Customer Cust;

        EditText FirstNameTxt; EditText LastNameTxt; EditText DobTxt;

        public CustomerWizardStep1()
        {
            StepExited += OnStepExited;
        }


        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container,
            Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.CustomerWizardStep1, container, false);

            FirstNameTxt = view.FindViewById<EditText>(Resource.Id.firstNameTxt);
            LastNameTxt = view.FindViewById<EditText>(Resource.Id.lastNameTxt);
            DobTxt = view.FindViewById<EditText>(Resource.Id.dobDt);

            FirstNameTxt.AfterTextChanged += delegate { Validate(); };
            LastNameTxt.AfterTextChanged += delegate { Validate(); };
            DobTxt.AfterTextChanged += delegate { Validate(); };

            return view;
        }

        void OnStepExited(StepExitCode exitCode)
        {
            if (exitCode == StepExitCode.ExitPrevious) return;

            if (Cust == null) Cust = new Customer();

            Cust.Firstname = FirstNameTxt.Text;
            Cust.LastName = FirstNameTxt.Text;
            Cust.Dob = System.DateTime.Parse(DobTxt.Text);
        }

        private void Validate()
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(FirstNameTxt.Text) || FirstNameTxt.Text.Length < 3) {
                FirstNameTxt.Error = "Invalid first name, must be at least 3 characters";
                valid = false;
            }
            else {
                FirstNameTxt.Error = null;
            }

            if (string.IsNullOrWhiteSpace(LastNameTxt.Text) || LastNameTxt.Text.Length < 3) {
                LastNameTxt.Error = "Invalid last name, must be at least 3 characters";
                valid = false;
            }
            else {
                LastNameTxt.Error = null;
            }

            System.DateTime result;
            if (string.IsNullOrWhiteSpace(DobTxt.Text) || 
                !System.DateTime.TryParseExact(DobTxt.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)) {
                DobTxt.Error = "Invalid date of birth";
                valid = false;
            }
            else {
                DobTxt.Error = null;
            }

            if (valid)
                NotifyCompleted(); // All the input is valid.. Set the step as completed
            else
                NotifyIncomplete();
        }
    }
}
