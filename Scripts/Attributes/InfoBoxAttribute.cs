using UnityEngine;

namespace ERInspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class InfoBoxAttribute : PropertyAttribute
    {
        public string message;
        public float viewWidth;

        public InfoBoxAttribute(string message)
        {
            this.message = message;
        }
    }
}