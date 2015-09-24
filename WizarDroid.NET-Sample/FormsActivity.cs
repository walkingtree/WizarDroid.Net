using Android.App;
using Android.OS;

namespace WizarDroid.NET_Sample
{
    [Activity(Label = "Simple Form Wizard")]
    public class FormsActivity : Android.Support.V4.App.FragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.formtest);
        }
    }
}