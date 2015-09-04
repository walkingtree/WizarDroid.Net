using WizarDroid.NET.Infrastructure.Layouts;

namespace WizarDroid.NET_Sample
{
    /// <summary>
    /// This is a simple two step wizard with no context to preserve.  
    /// The layout file for the parent activity defines a fragment (see tutorial.axml).  This wizard is tied to the fragment using this
    ///  android:name="WizarDroid.NET_Sample.TutorialWizard
    /// </summary>
    public class TutorialWizard : BasicWizardLayout
    {
        public TutorialWizard()
        {
            this.WizardCompleted += OnWizardComplete;
        }

        public override NET.WizardFlow OnSetup()
        {
            NextButtonText = "Advance";
            BackButtonText = "Return";
            FinishButtonText = "Finalize";

            return new NET.WizardFlow.Builder()
                                     .AddStep(new TutorialStep1())
                                     .AddStep(new TutorialStep2())
                                     .Create();
        }

        /// <summary>
        /// Defines what should happen once the wizard is complete
        /// </summary>
        public void OnWizardComplete()
        {
            Activity.Finish();
        }
    }
}