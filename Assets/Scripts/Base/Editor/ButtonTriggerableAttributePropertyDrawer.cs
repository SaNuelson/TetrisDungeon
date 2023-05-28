using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ButtonTriggerableAttribute))]
public class ButtonTriggerableAttributePropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ButtonTriggerableAttribute targetAttribute = attribute as ButtonTriggerableAttribute;
        Type methodOwnerType = property.serializedObject.targetObject.GetType();
        MethodInfo targetMethod = methodOwnerType.GetMethod(targetAttribute.FunctionName);

        if (GUI.Button(position, targetAttribute.ButtonName))
        {
            if (targetMethod != null)
            {
                targetMethod.Invoke(property.serializedObject.targetObject, null);
            }
            else
            {
                Debug.LogError($"Failed to invoke method '{targetAttribute.FunctionName}'.");
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
