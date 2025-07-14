using UnityEngine;

namespace ERInspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string conditionName;

        public ShowIfAttribute(string conditionName)
        {
            this.conditionName = conditionName;
        }
    }
}