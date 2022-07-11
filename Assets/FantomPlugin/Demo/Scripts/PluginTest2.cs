using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Android widgets demo using controllers
//･'XDebug' is a library that sees the debug log at runtime (When necessary, please enable 'DebugConsole' on the hierarchy).
public class PluginTest2 : MonoBehaviour {

    public Text apiLevelText;                           //For display of device API Level
    public Text languageText;                           //For display of system language
    public Text displayText;                            //For display of any text
    public string openURL;                              //URL to open in browser etc.
    public Toggle hardVolToggle;                        //Availability of hardware volume button (for property synchronization at startup).
    public Dropdown vibratorDropdown;                   //Vibrator Type select

    //Messages
    public string toastMessage = "The current time is ";    //Message when 'TestToast()' execute.
    public string toastTimeFormat = "HH:mm:ss";             //Message when 'TestToast()' execute.
    public string customResetMessage = "The setting was reset.";    //Message when "return=true" was included in the return value (OnCustom). //設定がリセットされました

    //Register 'ToastController.Show' in the inspector.
    [Serializable] public class ToastHandler : UnityEvent<string> { }   //message
    public ToastHandler OnToast;

    //Register 'CustomDialogController.ResetValue' in the inspector.
    public UnityEvent OnCustomReset;


    // Use this for initialization
    private void Start () {
        //Get Device API Level
        if (apiLevelText != null)
        {
            XDebug.Log(SystemInfo.operatingSystem);
#if !UNITY_EDITOR && UNITY_ANDROID
            int apiLevel = AndroidPlugin.GetAPILevel();
            apiLevelText.text = "Device API Level = " + apiLevel;
            XDebug.Log("Device API Level = " + apiLevel);
#endif
        }

        //Get system language
        if (languageText != null)
        {
            SystemLanguage lang = Application.systemLanguage;
            var strVal = Enum.GetName(typeof(SystemLanguage), lang);
            languageText.text = "Language = " + (int)lang + " (" + strVal + ")";
        }

        if (hardVolToggle != null)
        {
            hardVolToggle.isOn = FindObjectOfType<HardVolumeController>().HardOperation;
            OnHardVolumeOperationChanged(hardVolToggle.isOn);
        }

        if (vibratorControl == null)
        {
            vibratorControl = FindObjectOfType<VibratorController>();
            if (vibratorControl != null)
                XDebug.Log("IsSupportedVibrator = " + vibratorControl.IsSupportedVibrator);
        }
    }

    //This demo will reset some saved values.
    private void OnApplicationQuit()
    {
        ResetSavedValue();
    }

    // Update is called once per frame
    //private void Update () {

    //}

    
    //This demo will reset some saved values.
    public void ResetSavedValue()
    {
        //Reset the state of the checkbox.
        YesNoWithCheckBoxDialogController[] ynDlg = FindObjectsOfType<YesNoWithCheckBoxDialogController>();
        foreach (var item in ynDlg)
            item.ResetChecked();
    }

    //Display message by XDebug (for register callback in inpector)
    public void DisplayLog(string message)
    {
        XDebug.Log(message);
    }

    public void DisplayLog(string[] message)
    {
        XDebug.Log(string.Join("\n", message));
    }

    public void DisplayLog(int value)
    {
        XDebug.Log(value);
    }

    //Display message by Text (for register callback in inpector)
    public void DisplayText(string message)
    {
        if (displayText != null)
            displayText.text = message;
    }

    public void DisplayText(string[] message)
    {
        if (displayText != null)
            displayText.text = string.Join("\n", message);
    }

    public void DisplayText(int value)
    {
        if (displayText != null)
            displayText.text = value.ToString();
    }

    //Display message by Toast (for register callback in inpector)
    public void ShowToast(string message)
    {
        if (OnToast != null)
            OnToast.Invoke(message);
    }

    public void ShowToast(string[] message)
    {
        if (OnToast != null)
            OnToast.Invoke(string.Join("\n", message));
    }

    public void ShowToast(int value)
    {
        if (OnToast != null)
            OnToast.Invoke(value.ToString());
    }

    public void ShowToast(float value)
    {
        if (OnToast != null)
            OnToast.Invoke(value.ToString());
    }

    

    //ToastController demo (show current time)
    public void TestToast()
    {
        DateTime dt = DateTime.Now;
        ShowToast(toastMessage + dt.ToString(toastTimeFormat));
    }



    //YesNoWithCheckBoxDialogController demo (when 'Yes' button pressed)
    public void OnYes(string value, bool check)
    {
        XDebug.Log("PluginTest2.OnYes called");

        string str = value + ", checked = " + check;
        XDebug.Log(str);

        if (OnToast != null)
            OnToast.Invoke(str);
    }

    //YesNoWithCheckBoxDialogController demo (when 'No' button pressed)
    public void OnNo(string value, bool check)
    {
        XDebug.Log("PluginTest2.OnNo called");

        string str = value + ", checked = " + check;
        XDebug.Log(str);

        if (OnToast != null)
            OnToast.Invoke(str);
    }


    //OKWithCheckBoxDialogController demo (when dialog closed)
    public void OnClose(string value, bool check)
    {
        XDebug.Log("PluginTest2.OnClose called");

        string str = value + ", checked = " + check;
        XDebug.Log(str);

        if (OnToast != null)
            OnToast.Invoke(str);
    }


    //MultiChoiceDialogController demo (words) (When 'OK' button pressed)
    public void OnMultiChoice(string[] values)
    {
        XDebug.Log("PluginTest2.OnMultiChoice called");

        string str = string.Join(", ", values);
        XDebug.Log(string.IsNullOrEmpty(str) ? "(Empty)" : str);

        if (OnToast != null)
            OnToast.Invoke(str);
    }

    //MultiChoiceDialogController demo (indexes) (When 'OK' button pressed)
    public void OnMultiChoice(int[] indexes)
    {
        XDebug.Log("PluginTest2.OnMultiChoice called");

        string str = string.Join(", ", indexes.Select(e => e.ToString()).ToArray());
        XDebug.Log(string.IsNullOrEmpty(str) ? "(Empty)" : str);

        if (OnToast != null)
            OnToast.Invoke(str);
    }

    //MultiChoiceDialogController demo (words) (When change the selection state)
    public void OnMultiChoiceChanged(string[] values)
    {
        string str = string.Join(", ", values);
        XDebug.Log("OnMultiChoiceChanged : " + (string.IsNullOrEmpty(str) ? "(Empty)" : str));
    }

    //MultiChoiceDialogController demo (indexes)) (When change the selection state)
    public void OnMultiChoiceChanged(int[] indexes)
    {
        string str = string.Join(", ", indexes.Select(e => e.ToString()).ToArray());
        XDebug.Log("OnMultiChoiceChanged : " + (string.IsNullOrEmpty(str) ? "(Empty)" : str));
    }


    //SwitchDialogController demo (When 'OK' button pressed)
    public void OnSwitches(Dictionary<string, bool> values)
    {
        XDebug.Log("PluginTest2.OnSwitches called");

        string str = string.Join(", ", values.Select(e => e.Key + "=" + e.Value).ToArray());
        XDebug.Log(str);

        if (OnToast != null)
            OnToast.Invoke(str);
    }

    //SwitchDialogController demo (When switch pressing)
    public void OnSwitchChanged(string key, bool value)
    {
        XDebug.Log("OnSwitchChanged : " + key + " = " + value);
    }


    //SliderDialogController demo (When 'OK' button pressed)
    public void OnSliders(Dictionary<string, float> values)
    {
        XDebug.Log("PluginTest2.OnSliders called");

        string str = string.Join(", ", values.Select(e => e.Key + "=" + e.Value).ToArray());
        XDebug.Log(str);

        if (OnToast != null)
            OnToast.Invoke(str);
    }

    //SliderDialogController demo (When sliders moving)
    public void OnSliderChanged(string key, float value)
    {
        XDebug.Log("OnSliderChanged : " + key + " = " + value);
    }



    //CustomDialogController demo (When 'OK' button pressed)
    public void OnCustom(Dictionary<string, string> dic)
    {
        XDebug.Log("PluginTest2.OnCustom called");

        string str = string.Join(", ", dic.Select(e => e.Key + "=" + e.Value).ToArray());
        XDebug.Log(str);

        if (dic.ContainsKey("reset") && dic["reset"].ToLower() == "true")   //'Reset switch' in dialog
        {
            if (OnCustomReset != null)
                OnCustomReset.Invoke();

            if (OnToast != null)
                OnToast.Invoke(customResetMessage);
        }
        else
        {
            if (OnToast != null)
                OnToast.Invoke(str);
        }
    }

    //CustomDialogController demo (When item state changed)
    public void OnCustomChanged(string key, string value)
    {
        XDebug.Log("OnCustomChanged : " + key + " = " + value);
    }


    //When toggle button is switched (for debug)
    public void OnHardVolumeOperationChanged(bool isOn)
    {
        XDebug.Log("Hardware Volume button " + (isOn ? "enabled" : "disabled"));
    }


    //Vibrator demo
    VibratorController vibratorControl;

    //Callback handler When changing the type of vibrator.
    public void OnVibratorTypeChanged(int index)
    {
        if (vibratorDropdown != null)
        {
            if (vibratorControl == null)
                vibratorControl = FindObjectOfType<VibratorController>();
            if (vibratorControl != null)
                vibratorControl.vibratorType = (VibratorController.VibratorType)Enum.ToObject(typeof(VibratorController.VibratorType), index);
        }
    }
}
