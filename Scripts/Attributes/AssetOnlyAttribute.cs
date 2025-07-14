using UnityEngine;

namespace ERInspector
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class AssetOnlyAttribute : PropertyAttribute { }
}