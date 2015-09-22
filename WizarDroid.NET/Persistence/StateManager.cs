using System;
using System.Linq;
using Android.OS;

namespace WizarDroid.NET.Persistence
{
    public class StateManager : IStateManager
    {
        public Bundle ContextBundle { get; set; }

        #region IContextManager Members

        /// <summary>
        /// Populates the Fields in the Fragment with values stored within the BundleContext
        /// </summary>
        /// <param name="step"></param>
        public void LoadStepContext(Android.Support.V4.App.Fragment step)
        {
            var fields = step.GetType().GetFields();

            var args = step.Arguments;

            if (args == null)
                args = new Bundle();

            foreach (var field in fields) {
                if (field.CustomAttributes == null || !field.CustomAttributes.Any(a => a.AttributeType == typeof(WizardStateAttribute)))
                    continue; // Only process those fields that are decorated with WizardStateAttribute

                try {

                    if (field.FieldType == typeof(string)) {
                        args.PutString(field.Name, ContextBundle.GetString(field.Name));
                    }
                    else if (field.FieldType == typeof(int)) {
                        args.PutInt(field.Name, ContextBundle.GetInt(field.Name));
                    }
                    else if (field.FieldType == typeof(bool)) {
                        args.PutBoolean(field.Name, ContextBundle.GetBoolean(field.Name));
                    }
                    else if (field.FieldType == typeof(double)) {
                        args.PutDouble(field.Name, ContextBundle.GetDouble(field.Name));
                    }
                    else if (field.FieldType == typeof(float)) {
                        args.PutFloat(field.Name, ContextBundle.GetFloat(field.Name));
                    }
                    else if (field.FieldType == typeof(short)) {
                        args.PutShort(field.Name, ContextBundle.GetShort(field.Name));
                    }
                    else if (field.FieldType == typeof(DateTime)) {
                        args.PutLong(field.Name, ContextBundle.GetLong(field.Name));
                    }
                    else if (field.FieldType == typeof(char)) {
                        args.PutChar(field.Name, ContextBundle.GetChar(field.Name));
                    }
                    else if (typeof(Parcelable).IsAssignableFrom(field.FieldType)) {
                        args.PutParcelable(field.Name, ContextBundle.GetParcelable(field.Name) as IParcelable);
                    }
                    else if (field.FieldType is Java.IO.ISerializable) {
                        args.PutSerializable(field.Name, ContextBundle.GetSerializable(field.Name));
                    }
                    else if (field.FieldType.IsValueType == false) { //Runtime serialization for reference type.. use json.net serialization
                        args.PutString(field.Name, ContextBundle.GetString(field.Name));
                    }
                    else {
                        //TODO: Add support for arrays
                        throw new ArgumentException(string.Format("Unsuported type. Cannot pass value to variable {0} of step {1}. Variable type is unsuported.",
                                field.Name, step.GetType().FullName));
                    }
                }
                catch (FieldAccessException f) {
                    throw new ArgumentException(string.Format("Unable to access the field: {0}. Only public fields are supported", field.Name), f);
                }
            }

            if (step is WizardFragment)
                BindFields((WizardFragment)step, args);
            else
                step.Arguments = args;
        }

        /// <summary>
        /// Extracts the data from the Fragment and stores it into the Bundle
        /// </summary>
        /// <param name="step"></param>
        public void PersistStepContext(Android.Support.V4.App.Fragment step)
        {
            var fields = step.GetType().GetFields();

            foreach (var field in fields) {
                if (field.CustomAttributes == null || !field.CustomAttributes.Any(a => a.AttributeType == typeof(WizardStateAttribute)))
                    continue; // Only process those fields that are decorated with WizardStateAttribute

                try {
                    if (field.FieldType == typeof(string)) {
                        ContextBundle.PutString(field.Name, field.GetValue(step) as string);
                    }
                    else if (field.FieldType == typeof(int)) {
                        ContextBundle.PutInt(field.Name, (int)field.GetValue(step));
                    }
                    else if (field.FieldType == typeof(bool)) {
                        ContextBundle.PutBoolean(field.Name, (bool)field.GetValue(step));
                    }
                    else if (field.FieldType == typeof(double)) {
                        ContextBundle.PutDouble(field.Name, (double)field.GetValue(step));
                    }
                    else if (field.FieldType == typeof(float)) {
                        ContextBundle.PutFloat(field.Name, (float)field.GetValue(step));
                    }
                    else if (field.FieldType == typeof(short)) {
                        ContextBundle.PutShort(field.Name, (short)field.GetValue(step));
                    }
                    else if (field.FieldType == typeof(DateTime)) {
                        ContextBundle.PutLong(field.Name, ((DateTime)field.GetValue(step)).Ticks);
                    }
                    else if (field.FieldType == typeof(char)) {
                        ContextBundle.PutChar(field.Name, (char)field.GetValue(step));
                    }
                    else if (typeof(Parcelable).IsAssignableFrom(field.FieldType)) { //Support for Pacelable to be deprecated
                        ContextBundle.PutParcelable(field.Name, field.GetValue(step) as IParcelable);
                    }
                    else if (field.FieldType is Java.IO.ISerializable) { //Support for Java.IO.ISerializable to be deprecated
                        ContextBundle.PutSerializable(field.Name, field.GetValue(step) as Java.IO.ISerializable); 
                    }
                    else if (field.FieldType.IsValueType == false) { //Runtime serialization for reference type.. use json.net serialization
                        ContextBundle.PutString(field.Name, Newtonsoft.Json.JsonConvert.SerializeObject(field.GetValue(step)));
                    }
                    else {
                        //TODO: Add support for arrays
                        throw new ArgumentException(string.Format("Unsuported type. Cannot pass value to variable {0} of step {1}. Variable type is unsuported.",
                                field.Name, step.GetType().FullName));
                    }
                }
                catch (FieldAccessException f) {
                    throw new ArgumentException(string.Format("Unable to access the field: {0}. Only public fields are supported", field.Name), f);
                }
            }
        }

        #endregion


        private void BindFields(WizardFragment wizardFragment, Bundle args)
        {

            var fields = wizardFragment.GetType().GetFields(); //Scan the step for fields annotated with WizardState and bind value if found in step's arguments
            
            foreach (var field in fields) {
                if (field.CustomAttributes == null || !field.CustomAttributes.Any(a => a.AttributeType == typeof(WizardStateAttribute)))
                    continue;

                try {
                    if (field.GetType() == typeof(DateTime)) {
                        field.SetValue(wizardFragment, new DateTime(args.GetLong(field.Name)));
                    }
                    else {
                        //var value = args.Get(field.Name); //This wont work when passed to field.SetValue
                        var value = args.GetValue(field.Name, field.FieldType); //Workaround
                        field.SetValue(wizardFragment, value);
                    }
                }
                catch (FieldAccessException f) {
                    throw new ArgumentException(string.Format("Unable to access the field: {0}. Only public fields are supported", field.Name), f);
                }
            }
        }
   }
}