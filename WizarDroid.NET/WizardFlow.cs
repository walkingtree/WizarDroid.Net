using System;
using System.Collections.Generic;
using Android.OS;

namespace WizarDroid.NET
{
    /// <summary>
    /// WizardFlow holds information regarding the wizard's steps and flow. Use WizardFlow.Builder to create an instance of WizardFlow.
    /// </summary>
    public class WizardFlow
    {
        private IList<StepMetaData> Steps { get; set; }

        /// <summary>
        /// Total number of steps in this wizard, includes Required Steps
        /// </summary>
        public int TotalSteps { get { return Steps == null ? 0 : Steps.Count; } }

        /// <summary>
        /// Number of steps upto the last required and completed step.. This is used to restrict the view pager to the last required step that is yet
        /// to be completed
        /// </summary>
        public int StepsCount { get; private set; }


        private WizardFlow(IList<StepMetaData> steps)
        {
            Steps = steps;
            SetStepCount();
        }

        /// <summary>
        /// Get the list of wizard flow steps which is cut off at the last step which is required and incomplete
        /// and the first step which doesn't allow to go back and is incomplete.
        /// This method is designed to work directly with ViewPager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<WizardStep> GetSteps()
        {
            var cutOffFlow = new List<WizardStep>();

            //Calculate the cut off step by finding the last step which is required and incomplete
            foreach (var stepMetaData in this.Steps) {
                cutOffFlow.Add(stepMetaData.StepClass);
                if (!stepMetaData.Completed && stepMetaData.Required) break;
            }

            return cutOffFlow;
        }

        internal void SetStepCompleted(int position, bool stepCompleted)
        {
            if (Steps == null || position >= Steps.Count)
                throw new ArgumentOutOfRangeException("position", "Position param is out of range or no steps specified");

            Steps[position].Completed = stepCompleted;

            if (Steps[position].Required) SetStepCount(); //If this is a required step, then we need to update our counts
        }

        /// <summary>
        /// Check if the specified step is completed or incomplete
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal bool IsStepCompleted(int position)
        {
            if (Steps == null || position >= Steps.Count)
                throw new ArgumentOutOfRangeException("position", "Position param is out of range or no steps specified");

            return Steps[position].Completed;
        }

        /// <summary>
        /// Check if the specified step is required
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        internal bool IsStepRequired(int position)
        {
            if (Steps == null || position >= Steps.Count)
                throw new ArgumentOutOfRangeException("position", "Position param is out of range or no steps specified");

            return Steps[position].Required;
        }

        internal void LoadFlow(Bundle state)
        {
            foreach (var stepMetaData in Steps) {
                stepMetaData.Completed =
                        state.GetBoolean(stepMetaData.StepClass.GetType().Name + Steps.IndexOf(stepMetaData), stepMetaData.Completed /*defaultValue*/);
            }
        }

        internal void PersistFlow(Bundle state)
        {
            foreach (var stepMetaData in Steps) {
                state.PutBoolean(stepMetaData.StepClass.GetType().Name + Steps.IndexOf(stepMetaData), stepMetaData.Completed);
            }
        }

        /// <summary>
        /// Helper function to compute the step count
        /// </summary>
        private void SetStepCount()
        {
            if (Steps == null) {
                StepsCount = 0;
                return;
            }

            int count = 0;
            foreach (var stepMetaData in this.Steps) {
                ++count;
                if (!stepMetaData.Completed && stepMetaData.Required) break;
            }

            StepsCount = count;
        }

        /// <summary>
        /// Builder for WizardFlow. Use this class to build an instance of WizardFlow.
        /// You need to use this class in your wizard's WizardFragment.OnSetup() to return an instance of WizardFlow.
        /// Call AddStep(Class)} o add steps to the flow, keeping in mind that the order you the steps
        /// will be the order the wizard will display them. Eventually call WizardFlow.Builder.Create() to create the instance.
        /// </summary>
        public class Builder
        {

            private IList<StepMetaData> WizardSteps { get; set; }

            /// <summary>
            /// Ctor...
            /// </summary>
            public Builder()
            {
                WizardSteps = new List<StepMetaData>();
            }

            /// <summary>
            /// Add a step to the WizardFlow. Note that the wizard flow is determined by the order of added steps.
            /// </summary>
            /// <param name="stepClass"></param>
            /// <returns></returns>
            public Builder AddStep(WizardStep stepClass)
            {
                return AddStep(stepClass, false);
            }


            /// <summary>
            /// Add a step to the WizardFlow. Note that the wizard flow is determined by the order of added steps.
            /// </summary>
            /// <param name="stepClass"></param>
            /// /// <param name="isRequired">Determine if the step is required before advancing to the next step</param>
            /// <returns></returns>
            public Builder AddStep(WizardStep stepClass, bool isRequired)
            {
                WizardSteps.Add(new StepMetaData(isRequired, stepClass));
                return this;
            }

            /// <summary>
            /// Create a wizard flow object.. Must be called after the wizard steps have been added via a call to the AddStep method
            /// </summary>
            /// <returns></returns>
            public WizardFlow Create()
            {
                if (WizardSteps.Count > 0)
                    return new WizardFlow(WizardSteps);

                throw new InvalidOperationException("Cannot create WizardFlow. No step has been added! Call Builder#addStep(stepClass) to add steps to the wizard flow.");
            }
        }
    }
}