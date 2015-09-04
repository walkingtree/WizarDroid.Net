using Android.OS;
using Android.Support.V4.App;

namespace WizarDroid.NET.Persistence
{
    /// <summary>
    /// This interface defines the wizard state manager API used to pass data between steps.
    /// </summary>
    public interface IStateManager
    {
        /// <summary>
        /// Populates the Fields in the Fragment with values stored within the BundleContext
        /// </summary>
        /// <param name="step"></param>
        void LoadStepContext(Fragment step);

        /// <summary>
        /// Extracts the data from the Fragment and stores it into the Bundle
        /// </summary>
        /// <param name="step"></param>
        void PersistStepContext(Fragment step);
        
        /// <summary>
        /// The android bundle used to store the context
        /// </summary>
        Bundle ContextBundle { get; set; }
    }
}