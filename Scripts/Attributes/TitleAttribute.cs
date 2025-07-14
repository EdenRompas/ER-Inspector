using UnityEngine;

namespace ERInspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class TitleAttribute : PropertyAttribute
    {
        public string title;

        public TitleAttribute(string title)
        {
            this.title = title;
        }
    }
}