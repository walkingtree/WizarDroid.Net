using Android.OS;
using Android.Views;
using Android.Widget;
using WizarDroid.NET;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET_Sample.Wizards
{
    /// <summary>
    /// Step3 of the form wizard
    /// </summary>
    public class FormStep3 : WizardStep
    {
        [WizardState]
        public string FirstName;

        [WizardState]
        public string LastName;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.form_step_summary, container, false);

            var firstNameText = view.FindViewById<TextView>(Resource.Id.firstname);
            var lastNameText = view.FindViewById<TextView>(Resource.Id.lastname);

            firstNameText.Text = FirstName;
            lastNameText.Text = LastName;

            return view;
        }
    }
}