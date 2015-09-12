using System;
using Android.Support.V4.App;
using Android.Support.V4.View;
using WizarDroid.NET.Infrastructure;
using WizarDroid.NET.Infrastructure.Events;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET
{
    /// <summary>
    ///  The engine of the Wizard, wraps ViewPager under the hood. You would normally want to
    ///  extend WizardFragment instead of using this class directly and make calls to the wizard API
    ///  via WizarDroid.NET.WizardFragment.Wizard field. 
    ///  Use this class only if you wish to create a custom WizardFragment to control the wizard.
    /// </summary>
    public partial class Wizard : IDisposable, ISubscriber
    {
        /// <summary>
        /// Event called when the wizard is completed
        /// </summary>
        public event WizardCompleteHandler WizardComplete;

        /// <summary>
        /// Event called after a step was changed
        /// </summary>
        public event StepChangedHandler StepChanged;

        internal WizardStep PreviousStep { get; private set; }
        
        private WizardFlow WizardFlow { get; set; }
        private IStateManager StateManager { get; set; }
        private ViewPager ViewPager { get; set; }
        private int PreviousPosition { get; set; }
        private int PreviousState = ViewPager.ScrollStateIdle;
        private FragmentManager FragmentManager { get; set; }

        /// <summary>
        /// Constructs the Wizard
        /// </summary>
        /// <param name="wizardFlow">WizardFlow instance. See WizardFlow.Builder for more information on creating WizardFlow objects</param>
        /// <param name="contextManager"></param>
        /// <param name="callbacks"></param>
        /// <param name="fragmentManager"></param>
        public Wizard(WizardFlow wizardFlow,
                      IStateManager stateManager,
                      FragmentManager fragmentManager)
        {
            this.WizardFlow = wizardFlow;
            this.StateManager = stateManager;
            this.FragmentManager = fragmentManager;


            FragmentManager.BackStackChanged += (o, sender) =>
            {
                if (FragmentManager.BackStackEntryCount < ViewPager.CurrentItem) //Backbutton has been pressed
                    ViewPager.SetCurrentItem(ViewPager.CurrentItem - 1, true /*smoothScroll*/); // Go to the previous item on the stack
            };

            MessageBus.GetInstance().Register(this, typeof(StepCompletedEvent)); //Listen for the StepCompeltedEvent on the buss
        }


        /// <summary>
        /// Set the view pager into which this wizard should load the steps
        /// </summary>
        /// <param name="viewPager"></param>
        public void SetViewPager(ViewPager viewPager)
        {
            ViewPager = viewPager;
            ViewPager.Adapter = new WizardPagerAdapter(FragmentManager, WizardFlow, StateManager, this);

            //ViewPager.PageScrolled += (sender, e) => { };

            ViewPager.PageSelected += (sender, e) =>
            {
                if (FragmentManager.BackStackEntryCount < e.Position) {
                    FragmentManager.BeginTransaction().AddToBackStack(null).Commit();
                }
                else if (FragmentManager.BackStackEntryCount > e.Position) {
                    FragmentManager.PopBackStack();
                }
            };

            ViewPager.PageScrollStateChanged += (sender, e) =>
            {
                switch (e.State) {
                    case ViewPager.ScrollStateDragging: //Indicates that the pager is currently being dragged by the user.
                        PreviousPosition = ViewPager.CurrentItem;
                        PreviousStep = CurrentStep;
                        break;
                    case ViewPager.ScrollStateSettling: //Indicates that the pager is in the process of settling to a final position.
                        if (StepChanged != null)
                            StepChanged();
                        break;
                    case ViewPager.ScrollStateIdle: //Indicates that the pager is in an idle, settled state.
                        if (PreviousState == ViewPager.ScrollStateSettling) {
                            if (ViewPager.CurrentItem > PreviousPosition) {
                                ProcessStepBeforeChange(PreviousStep, PreviousPosition);
                                ViewPager.Adapter.NotifyDataSetChanged(); //Notifies the attached observers that the underlying data has been changed and any View reflecting the data set should refresh itself.
                            }
                            else {
                                PreviousStep.OnExit(StepExitCode.ExitPrevious);
                            }
                        }
                        break;
                }

                PreviousState = e.State;
            };
        }

        public WizardStep CurrentStep
        {
            get { return ((WizardPagerAdapter)ViewPager.Adapter).PrimaryItem; }
        }

        public void Close()
        {
            MessageBus.GetInstance().UnRegister(this);
            ViewPager = null;
            WizardFlow = null;
            FragmentManager = null;
            StateManager = null;
        }

        private void ProcessStepBeforeChange(WizardStep wizardStep, int position)
        {
            wizardStep.OnExit(StepExitCode.ExitNext);
            WizardFlow.SetStepCompleted(position, true);
            StateManager.PersistStepContext(wizardStep);
        }

        #region ISubscriber Members

        public void Receive<T>(T eventArgs)
        {
            var stepCompletedEvent = eventArgs as StepCompletedEvent;

            if (stepCompletedEvent == null || stepCompletedEvent.WizardStep != CurrentStep)
                return;

            // Check that the step is not already in this state to avoid spamming the viewpager
            if (WizardFlow.IsStepCompleted(ViewPager.CurrentItem) == stepCompletedEvent.IsStepCompleted)
                return;

            WizardFlow.SetStepCompleted(ViewPager.CurrentItem, stepCompletedEvent.IsStepCompleted);

            ViewPager.Adapter.NotifyDataSetChanged(); //Notifies the attached observers that the underlying data has been changed and any View reflecting the data set should refresh itself.

            if (StepChanged != null)  //Trigger the Event handler.. may be used to refresh the UI
                StepChanged();
        }

        #endregion

        /// <summary>
        /// Takes the wizard forward
        /// </summary>
        public void GoNext()
        {
            if (!CanGoNext) return;

            if (IsLastStep) {
                ProcessStepBeforeChange(CurrentStep, ViewPager.CurrentItem);

                if (WizardComplete != null)  //Since we were at last step, finish the wizard
                    WizardComplete();
                return;
            }
            //Else we are not at the last step.. Go further
            PreviousPosition = ViewPager.CurrentItem;
            PreviousStep = CurrentStep;
            ViewPager.SetCurrentItem(PreviousPosition + 1, true /*smoothscroll*/);
        }

        /// <summary>
        /// Takes the wizard one step back
        /// </summary>
        public void GoBack()
        {
            if (IsFirstStep) return; //Cannot go back further

            PreviousPosition = ViewPager.CurrentItem;
            PreviousStep = CurrentStep;

            ViewPager.SetCurrentItem(PreviousPosition - 1, true /*smoothscroll*/);
        }

        public bool IsFirstStep
        {
            get { return ViewPager.CurrentItem == 0; }
        }

        public bool IsLastStep
        {
            get { return ViewPager.CurrentItem == WizardFlow.TotalSteps - 1; }

        }

        public bool CanGoNext
        {
            get
            {
                return WizardFlow.IsStepRequired(ViewPager.CurrentItem) ? WizardFlow.IsStepCompleted(ViewPager.CurrentItem) : true;
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            this.Close();
        }

        #endregion

    }

    public delegate void WizardCompleteHandler();
    public delegate void StepChangedHandler();
}