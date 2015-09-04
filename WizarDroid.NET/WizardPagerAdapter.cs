using Android.Support.V4.App;
using Android.Views;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET
{
    /// <summary>
    /// Feeds the WizardSteps into the ViewPager (encapsulated by the Wizard class).
    /// </summary>
    public class WizardPagerAdapter : FragmentStatePagerAdapter
    {
        public WizardStep PrimaryItem { get; private set; }
        private WizardFlow WizardFlow { get; set; }
        private IStateManager StateManager { get; set; }
        private Wizard Wizard { get; set; }

        public WizardPagerAdapter(FragmentManager fragmentManager, WizardFlow wizardFlow, IStateManager stateManager, Wizard wizard)
            : base(fragmentManager)
        {
            WizardFlow = wizardFlow;
            StateManager = stateManager;
            Wizard = wizard;
        }

        public override Fragment GetItem(int position)
        {
            var wizardStep = WizardFlow.GetSteps()[position];
            StateManager.LoadStepContext(wizardStep);
            return wizardStep;
        }

        /// <summary>
        /// Called to inform the adapter of which item is currently considered to be the "primary", that is the one show to the user as the current page.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="position"></param>
        /// <param name="objectValue"></param>
        public override void SetPrimaryItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            base.SetPrimaryItem(container, position, objectValue);
            PrimaryItem = objectValue as WizardStep;
        }

        public override int Count
        {
            get { return WizardFlow.StepsCount; }
        }

        /// <summary>
        /// Called when the host view is attempting to determine if an item's position has changed. We 
        /// </summary>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            if (objectValue.Equals(Wizard.PreviousStep))
                return PositionUnchanged;

            return PositionNone; //Indicates that the item is no longer present in the adapter.. Forces a screen reload.. causing the updated context data to appear
        }
    }
}