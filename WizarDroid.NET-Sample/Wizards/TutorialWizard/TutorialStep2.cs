using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WizarDroid.NET;

namespace WizarDroid.NET_Sample
{
    public class TutorialStep2 : WizardStep
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.tuorial_steps_layout, container, false);
            TextView tv = (TextView)v.FindViewById(Resource.Id.textView);
            tv.Text = "This is an example of Step 2 and also the last step in this wizard. " +
                "By pressing Finish you will conclude this wizard and go back to the main activity." +
                "Hit the back button to go back to the previous step.";

            return v;
        }
    }
}