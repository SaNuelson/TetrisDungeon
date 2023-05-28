using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.AttributeUsage(System.AttributeTargets.Field)]
public class ButtonTriggerableAttribute : PropertyAttribute 
{
    private string buttonName = "";
    public string ButtonName
    {
        get { return buttonName; }
        set { buttonName = value; }
    }

    private string functionName = "";
    public string FunctionName
    {
        get { return functionName; }
        set { functionName = value; }
    }

    public ButtonTriggerableAttribute(string _FunctionToCall, string _ButtonName = null)
    {
        ButtonName = _ButtonName == null ? "Trigger " + _FunctionToCall + "()" : _ButtonName;
        FunctionName = _FunctionToCall;
    }
}
