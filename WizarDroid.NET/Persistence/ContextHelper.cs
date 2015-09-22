using System;
using Android.OS;

namespace WizarDroid.NET.Persistence
{
    /// <summary>
    /// Provides a workaround for extracting .net types from Android bundles
    /// </summary>
    internal static class ContextHelper
    {
        internal static object GetValue(this Bundle args, string key, Type type)
        {
            if (type == typeof(string)) {
                return args.GetString(key);
            }
            else if (type == typeof(int)) {
                return args.GetInt(key);
            }
            else if (type == typeof(bool)) {
                return args.GetBoolean(key);
            }
            else if (type == typeof(double)) {
                return args.GetDouble(key);
            }
            else if (type == typeof(float)) {
                return args.GetFloat(key);
            }
            else if (type == typeof(short)) {
                return args.GetShort(key);
            }
            else if (type == typeof(DateTime)) {
                return new DateTime(args.GetLong(key));
            }
            else if (type == typeof(char)) {
                return args.GetChar(key);
            }
            else if (typeof(Parcelable).IsAssignableFrom(type)) {
                return args.GetParcelable(key);
            }
            else if (type is Java.IO.ISerializable) {
                return args.GetSerializable(key);
            }
            else if (type.IsValueType == false) { //Runtime serialization for reference type.. use json.net serialization
                var value = args.GetString(key);
                if (string.IsNullOrWhiteSpace(value)) return null;
                return Newtonsoft.Json.JsonConvert.DeserializeObject(value, type);
            }
            else {
                //TODO: Add support for arrays
                throw new ArgumentException(string.Format("Unsuported type. Cannot pass value to variable {0} of step {1}. Variable type is unsuported.",
                       key, type.FullName));
            }
        }
    }
}