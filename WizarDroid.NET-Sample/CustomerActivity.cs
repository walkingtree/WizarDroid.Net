using Android.App;
using Android.OS;

namespace WizarDroid.NET_Sample
{
    [Activity(Label = "Customer Wizard")]
    public class CustomerActivity : Android.Support.V4.App.FragmentActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.CustomerWizardActivity);
        }
    }
}