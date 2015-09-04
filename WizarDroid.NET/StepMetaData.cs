
namespace WizarDroid.NET
{
    /// <summary>
    /// This class wraps WizardStep to provide additional meta data which is persisted separately as part of the wizard flow.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal  class StepMetaData {
        internal bool Completed { get; set; }

        internal bool Required { get; private set; }
        internal WizardStep StepClass { get; private set; }

        internal StepMetaData(bool isRequired, WizardStep stepClass)
        {
            Required = isRequired;
            StepClass = stepClass;
        }

    }
}