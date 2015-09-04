using Android.Support.V4.View;
using Android.Widget;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET.Infrastructure.Layouts
{
    /// <summary>
    /// Basic Wizard UI class with built-in layout that uses two buttons 'Next' and 'Back' to control the wizard (button labels are customizable). 
    /// The 'Back' button is disabled on the first step. Extend this class if you wish to use a wizard with this built-in layout.
    /// Otherwise, extend  WizardFragment and implement a custom wizard layout.
    /// Override WizardFragment.OnSetup() to set up the wizard's flow.  Handle the WizardCompleted Event to determine
    /// what should happen when the wizard completes.
    /// </summary>
    public abstract class BasicWizardLayout : WizardFragment
    {
        private Button NextButton;
        private Button PreviousButton;

        public string NextButtonText { get; set; }
        public string FinishButtonText { get; set; }
        public string BackButtonText { get; set; }

        private ViewPager ViewPager { get; set; }

        //Empty constructor for Fragment. You must have an empty constructor according to Fragment documentation
        public BasicWizardLayout()
            : base()
        {
        }

        public BasicWizardLayout(IStateManager stateManager)
            : base(stateManager)
        {
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var wizardLayout = inflater.Inflate(Resource.Layout.wizardroid_basic_wizard, container, false);
            
            //Hookup the next button
            NextButton = (Button)wizardLayout.FindViewById(Resource.Id.wizard_next_button);
            NextButton.Text = NextButtonText ?? Resources.GetString(Resource.String.action_next);
            NextButton.Click += OnNextButtonClick;

            //Now the previous button
            PreviousButton = (Button)wizardLayout.FindViewById(Resource.Id.wizard_previous_button);
            PreviousButton.Text = BackButtonText ?? Resources.GetString(Resource.String.action_previous);
            PreviousButton.Click += OnPreviousButtonClick;
            
            ViewPager = wizardLayout.FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.step_container);
            
            return wizardLayout;
        }

        public override void OnActivityCreated(Android.OS.Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            Wizard.SetViewPager(ViewPager);
        }

        public override void OnResume()
        {
            base.OnResume();
            UpdateWizardControls();
        }


        protected override void OnWizardStepChanged()
        {
            base.OnWizardStepChanged();
            UpdateWizardControls();
        }


        private void OnPreviousButtonClick(object sender, System.EventArgs e)
        {
            Wizard.GoBack();
        }

        private void OnNextButtonClick(object sender, System.EventArgs e)
        {
            Wizard.GoNext();
        }

        private void UpdateWizardControls()
        {
            PreviousButton.Enabled = !Wizard.IsFirstStep;//Disable the back button in the first step
            PreviousButton.Text = BackButtonText ?? Resources.GetString(Resource.String.action_previous);
            
            NextButton.Enabled = Wizard.CanGoNext; //Disable the next button if the step is marked as 'required' and is incomplete
            NextButton.Text = Wizard.IsLastStep ?
                                   FinishButtonText ?? Resources.GetString(Resource.String.action_finish) :
                                   NextButtonText ?? Resources.GetString(Resource.String.action_next); //Set different next button label based on the wizard position
        }
    }
}