
using Android.OS;
using Android.Views;
using Android.Widget;
using WizarDroid.NET;

namespace WizarDroid.NET_Sample
{
    public class TutorialStep1 : WizardStep
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.tuorial_steps_layout, container, false);
            TextView tv = (TextView)v.FindViewById(Resource.Id.textView);
            tv.Text = "This is an example of Step 1 in the wizard. Press the Next " +
                       "button to proceed to the next step. Hit the back button to go back to the calling activity.";

            return v;
        }
    }
}