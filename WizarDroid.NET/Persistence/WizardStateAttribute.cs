using System;

namespace WizarDroid.NET.Persistence
{
    /// <summary>
    /// Each context variable whose state must be preserved should be decorated with the [WizardState] attribute.
    /// The attribute may only be applied to public fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Field )]
    public class WizardStateAttribute : Attribute
    {
    }
}