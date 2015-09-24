using System;
using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using WizarDroid.NET.Infrastructure;
using WizarDroid.NET.Infrastructure.Events;
using WizarDroid.NET.Persistence;

namespace WizarDroid.NET
{
    /// <summary>
    /// Defines the direction in which the user navigated away from this step.
    /// </summary>
    public enum StepExitCode
    {
        ExitNext = 0,
        ExitPrevious = 1
    }

    /// <summary>
    /// Base class for a wizard's step. Extend this class to create a step and override OnExit(int) to handle input
    /// and do tasks before the wizard changes the current step.
    ///  As with regular Fragment each inherited class must have an empty constructor.
    /// </summary>
    public class WizardStep : Fragment
    {
        public event OnExitHandler StepExited;

        //We will use the lockobject to sync the notify events -- avoid uncessary spamming if the event has already once been raised
        private object LockObject = new object();
        private bool StepCompleted = false;

        /// <summary>
        /// The wizard triggers OnExit when the user scrolls forward or backward.  Trigger the step exit event passing on the exit code to underlying
        /// custom implementations
        /// </summary>
        /// <param name="exitCode"></param>
        internal void OnExit(StepExitCode exitCode)
        {
            if (StepExited != null)
                StepExited(exitCode);
        }

        /// <summary>
        /// Notify the wizard that this step is completed
        /// </summary>
        public void NotifyCompleted()
        {
            lock (LockObject) {
                if (StepCompleted == false) { //Step was not marked complete prior to this call, so raise the step completion event
                    MessageBus.GetInstance().Publish(new StepCompletedEvent(true, this/*wizardStep*/));
                    StepCompleted = true;
                }
            }
        }

        /// <summary>
        /// Notify the wizard that this step is incomplete
        /// </summary>
        public void NotifyIncomplete()
        {
            lock (LockObject) {
                if (StepCompleted == true) { //Step was previously marked complete.. We are reversing the call, so raise the event
                    MessageBus.GetInstance().Publish(new StepCompletedEvent(false, this/*wizardStep*/));
                    StepCompleted = false;
                }
            }
        }


        public override void OnAttach(Android.App.Activity activity)
        {
            base.OnAttach(activity);
            if (this.Arguments != null)
                BindFields(this.Arguments);
        }

        private void BindFields(Bundle args)
        {
            var fields = this.GetType().GetFields();

            foreach (var field in fields) {
                if (field.CustomAttributes == null || !field.CustomAttributes.Any(a => a.AttributeType == typeof(WizardStateAttribute)))
                    continue;

                try {
                    if (field.GetType() == typeof(DateTime)) {
                        field.SetValue(this, new DateTime(args.GetLong(field.Name)));
                    }
                    else {
                        //var value = args.GetString(field.Name);
                        var value = args.GetValue(field.Name, field.FieldType); //Workaround
                        field.SetValue(this, value);
                    }
                }
                catch (FieldAccessException f) {
                    throw new ArgumentException(string.Format("Unable to access the field: {0}. Only public fields are supported", field.Name), f);
                }
            }
        }
    }

    public delegate void OnExitHandler(StepExitCode exitCode);
}