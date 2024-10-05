using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EREditor.Inspector
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : Attribute { }

#if UNITY_EDITOR

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonFunctionEditor : Editor
    {
        private bool isShowParameterWindow = false;
        private MethodInfo methodWithParameters;
        private object[] parameterValues;
        private GUIStyle backgroundStyle;

        private void InitializeStyles()
        {
            backgroundStyle = new GUIStyle(GUI.skin.window);
            backgroundStyle.padding = new RectOffset(5, 5, 5, 5);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InitializeStyles();

            Type inspectedType = target.GetType();

            MethodInfo[] methods = inspectedType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods)
            {
                if (Attribute.IsDefined(method, typeof(ButtonAttribute)))
                {
                    string methodName = Regex.Replace(method.Name, "(\\B[A-Z])", " $1");
                    methodName = char.ToUpper(methodName[0]) + methodName.Substring(1);

                    ParameterInfo[] parameters = method.GetParameters();

                    string addedName = parameters.Length > 0 ? " - Click To Set Params" : "";

                    if (GUILayout.Button(methodName + addedName))
                    {
                        if (parameters.Length > 0)
                        {
                            isShowParameterWindow = true;
                            methodWithParameters = method;
                            parameterValues = new object[parameters.Length];
                        }
                        else
                        {
                            method.Invoke(target, null);
                        }
                    }
                }
            }

            if (isShowParameterWindow)
            {
                EditorGUILayout.BeginVertical(backgroundStyle);
                DisplayParameterWindow();

                if (GUILayout.Button("Invoke"))
                {
                    if (methodWithParameters != null)
                    {
                        methodWithParameters.Invoke(target, parameterValues);
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        private void DisplayParameterWindow()
        {
            ParameterInfo[] parameters = methodWithParameters.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                Type parameterType = parameters[i].ParameterType;

                if (parameters[i].ParameterType == typeof(int))
                {
                    if (parameterValues[i] == null)
                        parameterValues[i] = 0;

                    parameterValues[i] = EditorGUILayout.IntField(parameters[i].Name, (int)parameterValues[i]);
                }
                else if (parameters[i].ParameterType == typeof(float))
                {
                    if (parameterValues[i] == null)
                        parameterValues[i] = 0;

                    parameterValues[i] = EditorGUILayout.FloatField(parameters[i].Name, (float)parameterValues[i]);
                }
                else if (parameters[i].ParameterType == typeof(string))
                {
                    parameterValues[i] = EditorGUILayout.TextField(parameters[i].Name, (string)parameterValues[i]);
                }
                else if (parameters[i].ParameterType == typeof(GameObject))
                {
                    parameterValues[i] = (GameObject)EditorGUILayout.ObjectField(parameters[i].Name, (GameObject)parameterValues[i], typeof(GameObject), true);
                }
                else if (parameters[i].ParameterType.IsSubclassOf(typeof(Component)))
                {
                    parameterValues[i] = (Component)EditorGUILayout.ObjectField(parameters[i].Name, (Component)parameterValues[i], parameters[i].ParameterType, true);
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(parameterType))
                {
                    parameterValues[i] = EditorGUILayout.ObjectField(parameters[i].Name, (UnityEngine.Object)parameterValues[i], parameterType, true);
                }
            }
        }
    }

#endif
}
