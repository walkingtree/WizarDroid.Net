using WizarDroid.NET.Infrastructure.Layouts;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET_Sample.Wizards
{
    public class CustomerWizard : BasicWizardLayout
    {
        [WizardState]
        public Customer Cust;

        public CustomerWizard()
        {
            WizardCompleted += OnWizardComplete;
        }

        public override WizarDroid.NET.WizardFlow OnSetup()
        {
            return new WizarDroid.NET.WizardFlow.Builder()
                                                .AddStep(new Step1(), true/*isRequired*/)
                                                .AddStep(new Step2(), true/*isRequired*/)
                                                .AddStep(new Step3(), true/*isRequired*/)
                                                .Create();
        }


        void OnWizardComplete()
        {
            var jsonObj = Newtonsoft.Json.JsonConvert.SerializeObject(Cust);
            Android.Util.Log.Info("CustomerWizard", jsonObj);
            Activity.Finish();
        }
    }
}