using System;

namespace ERInspector
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : Attribute { }
}