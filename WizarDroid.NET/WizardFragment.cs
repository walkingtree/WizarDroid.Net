using System;
using Android.OS;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET
{
    /// <summary>
    /// Base class for Fragments that want to implement step-by-step wizard functionality... The Wizard Steps are loaded as child Fragments within this Fargment.
    /// Extend this class to implement your own custom wizard layout use Wizard API to control the wizard. Typically, you'd call Wizard.GoNext() and Wizard.GoBack()
    /// from your controls onClick event to control the flow of the wizard.
    /// The implementation takes care of persisting the instance of the fragment and therefore the wizard context.
    /// Keep in mind that if for some reason you are not able to extend this class and have to implement your
    ///  own, then wizard context persistence is totally up to you by implementing IStateManager and passing
    ///  an instance of it when you construct the Wizard.
    /// </summary>
    public abstract class WizardFragment : Android.Support.V4.App.Fragment // Must derive from v4 fragment to support viewpager
    {
        protected Wizard Wizard { get; set; }

        private IStateManager StateManager { get; set; }
        private WizardFlow WizardFlow { get; set; }
        private const string STATE_WIZARD_CONTEXT = "ContextVariable";

        public event WizardCompleteHandler WizardCompleted;

        public WizardFragment()
            : this(new StateManager())
        {
        }

        public WizardFragment(IStateManager stateManager)
        {
            StateManager = stateManager;
        }

        /// <summary>
        /// Called once the fragment is associated with its activity.
        /// </summary>
        /// <param name="activity"></param>
        public override void OnAttach(Android.App.Activity activity)
        {
            base.OnAttach(activity);

            WizardFlow = OnSetup(); //Require the user to Setup the Wizard flow when the fragment is attached 

            if (WizardFlow == null) 
                throw new ArgumentException("Error setting up the Wizard's flow. You must override WizardFragment.OnSetup and create a WizardFlow using the WizardFlow.Builder.");
        }

        /// <summary>
        ///  Tells the fragment that its activity has completed its own Activity.onCreate().
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            if (savedInstanceState != null) { //Load pre-saved wizard context.. Coming in from a orientation change perhaps
                WizardFlow.LoadFlow(savedInstanceState);
                
                StateManager.ContextBundle = savedInstanceState.GetBundle(STATE_WIZARD_CONTEXT);
                StateManager.LoadStepContext(this);
            }
            else {
                StateManager.ContextBundle = new Bundle();    //Initialize wizard context
                //Persist hosting activity/fragment fields to wizard context enabling easy data transfer between wizard host and the steps
                StateManager.PersistStepContext(this);
            }
            
            Wizard = new Wizard(WizardFlow, StateManager, ChildFragmentManager);

            Wizard.StepChanged += OnWizardStepChanged;
            Wizard.WizardComplete += OnWizardComplete;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            Wizard.Close();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            WizardFlow.PersistFlow(outState);
            outState.PutBundle(STATE_WIZARD_CONTEXT, StateManager.ContextBundle);
        }

        /// <summary>
        /// Triggered when the wizard is completed. Overriding this method is optional.
        ///  Typically overriden to persist the final wizard context and finish the activity
        /// </summary>
        internal void OnWizardComplete()
        {
            StateManager.LoadStepContext(this);
            
            if (this.WizardCompleted != null) //Trigger the event within the fragment.. The underlying user defined wizard can add an handler to close out the activity if so desired
                this.WizardCompleted();
        }

        protected virtual void OnWizardStepChanged()
        {
            // In order to hide software input method we need to authorize with window token from focused window
            // this code relies on (somewhat fragile) assumption, that the only window, that can hold
            // software keyboard focus during fragment switch, one with fragment itself.
            var inputMethodManager = Android.Views.InputMethods.InputMethodManager.FromContext(Activity);

            var focusedWindowChild = Wizard.CurrentStep == null ? null : Wizard.CurrentStep.View;

            if (focusedWindowChild == null)
                focusedWindowChild = Activity.CurrentFocus;

            if (focusedWindowChild == null)
                focusedWindowChild = new Android.Views.View(Activity);

            inputMethodManager.HideSoftInputFromWindow(focusedWindowChild.WindowToken, 0);
        }



        /// <summary>
        /// Set up the Wizard's flow. Use WizardFlow.Builder to create the Wizard's flow.
        /// </summary>
        /// <returns></returns>
        public abstract WizardFlow OnSetup();

    }
}
