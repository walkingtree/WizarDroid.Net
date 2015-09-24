using WizarDroid.NET;
using WizarDroid.NET.Infrastructure.Layouts;
using WizarDroid.NET.Persistence;
using WizarDroid.NET_Sample.Wizards;

namespace WizarDroid.NET_Sample
{
    /// <summary>
    /// This wizard demonstates passing context between various fragments. The wizard also demonstates how one might setup a required step.
    /// </summary>
    public class FormWizard : BasicWizardLayout
    {

        // Variables tagged with WizardStateAttribute are used to perist context collected from the Wizard Steps/
        // NOTE: Wizard State Variable names are unique and therefore must have the same name and type wherever you wish to use them.
        [WizardState]
        public string FirstName = "Walking";

        [WizardState]
        public string LastName = "Tree";

        [WizardState]
        public bool Agreed = false;


        public FormWizard()
        {
            WizardCompleted += OnWizardComplete;
        }

        /// <summary>
        /// The custom wizard must implement the OnSetup method and instantiate a new Wizard Flow using the WizardFlow builder
        /// </summary>
        /// <returns></returns>
        public override WizardFlow OnSetup()
        {
            NextButtonText = "Advance";  //Customize the text on the wizard buttons
            BackButtonText = "Return";
            FinishButtonText = "Finalize";

            //Add your steps in the order you want them to appear and eventually call create() to create the wizard flow.
            return new NET.WizardFlow.Builder()
                                     .AddStep(new FormStep1())
                                     .AddStep(new FormStep2(), true /*isRequired*/) //Mark this step as 'required', preventing the user from advancing to the next step until a certain action is taken to mark this step as completed by calling WizardStep#notifyCompleted() from the step.
                                     .AddStep(new FormStep3())
                                     .Create();
        }

        /// <summary>
        /// Sets up the wizard completion handler..  This is where we process the final user input
        /// </summary>
        void OnWizardComplete()
        {
            Android.Util.Log.Info("FormWizard", string.Format("The full name is {0} {1}", FirstName, LastName)); //... Access context variables here before terminating the wizard

            Activity.Finish(); //Store the data in the DB or pass it back to the calling activity
        }
    }
}