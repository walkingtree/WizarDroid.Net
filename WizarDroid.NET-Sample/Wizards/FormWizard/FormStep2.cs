using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using WizarDroid.NET;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET_Sample.Wizards
{
    /// <summary>
    /// Step 2 of the FormWizard
    /// </summary>
    public class FormStep2 : WizardStep
    {
        [WizardState]
        public string FirstName;

        [WizardState]
        public string LastName;

        [WizardState]
        public bool Agreed = false;

        private CheckBox checkBox;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.form_step_2, container, false);
            checkBox = view.FindViewById<CheckBox>(Resource.Id.sample_form2_checkbox);

            var firstNameText = view.FindViewById<TextView>(Resource.Id.firstname_form2);

            firstNameText.Text = String.Format("Hello {0},", FirstName);

            checkBox.CheckedChange += checkBox_CheckedChange;

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        void checkBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Agreed = e.IsChecked;

            if (e.IsChecked)
                NotifyCompleted();
            else
                NotifyIncomplete();
        }
    }
}