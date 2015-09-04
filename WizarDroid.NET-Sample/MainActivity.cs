using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace WizarDroid.NET_Sample
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        private Sample[] Samples { get; set; }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.main);

            Samples = new Sample[] {
                new Sample("Tutorial Wizard", typeof(TutorialActivity)),
                new Sample("Forms Wizard", typeof(FormsActivity))
            };

            this.ListAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, Android.Resource.Id.Text1, Samples);
        }

        protected override void OnListItemClick(ListView l, Android.Views.View v, int position, long id)
        {
            StartActivity(new Intent(this, Samples[position].ActivityType));
        }

        private class Sample
        {
            private string Title { get; set; }
            public Type ActivityType { get; private set; }

            public Sample(string title, Type activityType)
            {
                Title = title;
                ActivityType = activityType;
            }

            public override string ToString()
            {
                return Title;
            }
        }
    }
}

