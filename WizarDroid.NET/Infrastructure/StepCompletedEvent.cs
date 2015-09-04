
namespace WizarDroid.NET.Infrastructure.Events
{
    //triggered when a wizard step is either set as completed or incomplete
    public class StepCompletedEvent
    {
        public bool IsStepCompleted { get; private set; }
        public WizardStep WizardStep { get; private set; }
        public StepCompletedEvent(bool isStepComplete, WizardStep step)
        {
            this.IsStepCompleted = isStepComplete;
            this.WizardStep = step;
        }
    }
}