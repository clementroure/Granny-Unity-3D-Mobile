using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

// Android widgets demo using controllers and localize string
//･'XDebug' is a library that sees the debug log at runtime (When necessary, please enable 'DebugConsole' on the hierarchy).
//
// Android ウィジェットでコントローラ（～Controller）とローカライズを利用したデモ
//・「XDebug」はランタイム時に Debug.Log を見るライブラリです（必要なときに、ヒエラルキーにある「DebugConsole」を有効にして下さい）。
public class PluginTest3 : MonoBehaviour {

    public Text apiLevelText;                           //For display of device API Level
    public Text languageText;                           //For display of system language
    public Text displayText;                            //For display of any text
    public Toggle hardVolToggle;                        //Availability of hardware volume button (for property synchronization at startup).
    public Dropdown vibratorDropdown;                   //Vibrator Type select

    //Register 'ToastController' in the inspector
    public ToastController toastControl;                //Mainly 'ToastController.Show' is called

    public SystemLanguage localizeLanguage = SystemLanguage.Unknown;   //current localize language
    public LanguageDropdown localizeDropdown;           //language dropdown functions

    //Messages
    //Message when 'TestToast()' execute.
    public LocalizeString toastMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "The current time is "),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "現在の時刻は "),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "当前时间是 "),
            new LocalizeString.Data(SystemLanguage.Korean, "현재 시간은 "),
        });

    public string toastTimeFormat = "HH:mm:ss";             //Message when 'TestToast()' execute.

    public LocalizeString notifLookMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Look at the Notification!"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "通知を見てね！"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "看通知！"),
            new LocalizeString.Data(SystemLanguage.Korean, "통지를 봐!"),
        });

    //Register 'VibratorController' in the inspector
    public VibratorController vibratorControl;


    //Register 'CustomDialogController' in the inspector
    public CustomDialogController customControl;            //Mainly 'CustomDialogController.ResetValue' is called.

    //Message when "reset=true" was included in the return value (OnCustom).
    public LocalizeString resetSettingMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "The setting was reset."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "設定がリセットされました"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "该设置被重置"),
            new LocalizeString.Data(SystemLanguage.Korean, "설정이 초기화되었습니다"),
        });

    //Message when 'yes' response
    public LocalizeString yesResponseMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Oh! it is nice!"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "ふむ、それは良いことだ。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "哦! 很好！"),
            new LocalizeString.Data(SystemLanguage.Korean, "오! 그거 좋네!"),
        });

    //Message when 'no' response
    public LocalizeString noResponseMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "All right. Surely it gets better."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "大丈夫だ。問題ない。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "好吧。它当然会变得更好。"),
            new LocalizeString.Data(SystemLanguage.Korean, "괜찮아. 확실히 더 좋아집니다."),
        });

    //Message when 'ok' response
    public LocalizeString okResponseMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "This world is wonderful!"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "この世は驚きに満ちている！"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "这个世界太棒了！"),
            new LocalizeString.Data(SystemLanguage.Korean, "이 세상은 놀라워요!"),
        });




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
            languageText.text = "System Language = " + (int)lang + " (" + strVal + ")";
        }

        if (hardVolToggle != null)
        {
            hardVolToggle.isOn = FindObjectOfType<HardVolumeController>().HardOperation;
            OnHardVolumeOperationChanged(hardVolToggle.isOn);
        }

        if (vibratorControl != null)
            XDebug.Log("IsSupportedVibrator = " + vibratorControl.IsSupportedVibrator);
    }

    //This demo will reset some saved values.
    private void OnApplicationQuit()
    {
        ResetSavedChecked();
    }

    // Update is called once per frame
    //private void Update () {

    //}

    
    //==========================================================

    //This demo will reset some saved values.
    public void ResetSavedChecked()
    {
        //Reset the state of the checkbox.
        YesNoWithCheckBoxDialogController[] ynDlg = FindObjectsOfType<YesNoWithCheckBoxDialogController>();
        foreach (var item in ynDlg)
            item.ResetChecked();
    }


    //==========================================================
    //Display message by UI-Text (for register callback in inpector)

    public void DisplayText(object message)
    {
        if (displayText != null)
            displayText.text = message.ToString();
    }

    public void DisplayText(int index)
    {
        if (displayText != null)
            displayText.text = index.ToString();
    }

    public void DisplayText(object message, bool check)
    {
        if (displayText != null)
            displayText.text = message + " = " + check;
    }

    public void DisplayText(object key, string value)
    {
        if (displayText != null)
            displayText.text = key + " = " + value;
    }

    public void DisplayText(object key, float value)
    {
        if (displayText != null)
            displayText.text = key + " = " + value;
    }

    public void DisplayText(object[] messages)
    {
        if (displayText != null)
            displayText.text = (messages == null || messages.Length == 0)
                ? "(Empty)" : string.Join(", ", messages.Select(e => e.ToString()).ToArray());
    }

    public void DisplayText(Dictionary<string, string> values)
    {
        if (displayText != null)
            displayText.text = ((values == null || values.Count == 0)
               ? "(Empty)" : string.Join("\n", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void DisplayText(Dictionary<string, bool> values)
    {
        if (displayText != null)
            displayText.text = ((values == null || values.Count == 0)
               ? "(Empty)" : string.Join("\n", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void DisplayText(Dictionary<string, float> values)
    {
        if (displayText != null)
            displayText.text = ((values == null || values.Count == 0)
               ? "(Empty)" : string.Join("\n", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void ClearText()
    {
        if (displayText != null)
            displayText.text = "";
    }


    //==========================================================
    //Display message by XDebug.Log (for register callback in inpector)

    public void DisplayLog(object message)
    {
        XDebug.Log(message);
    }

    public void DisplayLog(int index)
    {
        XDebug.Log(index);
    }

    public void DisplayLog(object message, bool check)
    {
        XDebug.Log(message + " = " + check);
    }

    public void DisplayLog(object key, string value)
    {
        XDebug.Log(key + " = " + value);
    }

    public void DisplayLog(object key, float value)
    {
        XDebug.Log(key + " = " + value);
    }

    public void DisplayLog(object[] message)
    {
        XDebug.Log((message == null || message.Length == 0)
            ? "(Empty)" : string.Join(", ", message.Select(e => e.ToString()).ToArray()));
    }

    public void DisplayLog(int[] indexes)
    {
        XDebug.Log((indexes == null || indexes.Length == 0)
            ? "(Empty)" : string.Join(", ", indexes.Select(e => e.ToString()).ToArray()));
    }

    public void DisplayLog(Dictionary<string, string> values)
    {
        XDebug.Log((values == null || values.Count == 0)
            ? "(Empty)" : string.Join(", ", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void DisplayLog(Dictionary<string, bool> values)
    {
        XDebug.Log((values == null || values.Count == 0)
            ? "(Empty)" : string.Join(", ", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void DisplayLog(Dictionary<string, float> values)
    {
        XDebug.Log((values == null || values.Count == 0)
            ? "(Empty)" : string.Join(", ", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void ClearLog()
    {
        XDebug.Clear();
    }

    public void DisplayLogPermission(string permission, bool granted)
    {
        XDebug.Log(permission.Replace("android.permission.", "") + " = " + granted);
    }

    //==========================================================
    //Display message by UI-Text (for register callback in inpector)

    public void ShowToast(object message)
    {
        if (toastControl != null)
            toastControl.Show(message.ToString());
    }

    public void ShowToast(int index)
    {
        if (toastControl != null)
            toastControl.Show(index.ToString());
    }

    public void ShowToast(object message, bool check)
    {
        if (toastControl != null)
            toastControl.Show(message + " = " + check);
    }

    public void ShowToast(object key, string value)
    {
        if (toastControl != null)
            toastControl.Show(key + " = " + value);
    }

    public void ShowToast(object key, float value)
    {
        if (toastControl != null)
            toastControl.Show(key + " = " + value);
    }


    public void ShowToast(object[] messages)
    {
        if (toastControl != null)
           toastControl.Show((messages == null || messages.Length == 0)
               ? "(Empty)" : string.Join("\n", messages.Select(e => e.ToString()).ToArray()));
    }

    public void ShowToast(Dictionary<string, string> values)
    {
        if (toastControl != null)
           toastControl.Show((values == null || values.Count == 0)
               ? "(Empty)" : string.Join("\n", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void ShowToast(Dictionary<string, bool> values)
    {
        if (toastControl != null)
           toastControl.Show((values == null || values.Count == 0)
               ? "(Empty)" : string.Join("\n", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }

    public void ShowToast(Dictionary<string, float> values)
    {
        if (toastControl != null)
           toastControl.Show((values == null || values.Count == 0)
               ? "(Empty)" : string.Join("\n", values.Select(e => e.Key + "=" + e.Value).ToArray()));
    }
    
    //==========================================================
    //Demos

    //ToastController demo (show current time)
    public void TestToast()
    {
        DateTime dt = DateTime.Now;
        ShowToast(toastMessage + dt.ToString(toastTimeFormat));
    }

    //Notification demo
    public void ShowToastSettingReset()
    {
        if (toastControl != null)
            toastControl.Show(resetSettingMessage.TextByLanguage(localizeLanguage));
    }


    //CustomDialogController demo (When 'OK' button pressed)
    public void OnCustom(Dictionary<string, string> dic)
    {
        string str = string.Join(", ", dic.Select(e => e.Key + "=" + e.Value).ToArray());
        XDebug.Log(str);

        if (dic.ContainsKey("reset") && dic["reset"].ToLower() == "true")   //'Reset switch' in dialog
        {
            if (customControl != null)
                customControl.ResetValue();

            ShowToastSettingReset();
        }
        else
        {
            if (toastControl != null)
                toastControl.Show(str);
        }
    }


    //When toggle button is switched (for debug)
    public void OnHardVolumeOperationChanged(bool isOn)
    {
        XDebug.Log("Hardware Volume button " + (isOn ? "enabled" : "disabled"));
    }


    //Vibrator demo
    //Callback handler When changing the type of vibrator.
    public void OnVibratorTypeChanged(int index)
    {
        if (vibratorDropdown != null && vibratorControl != null)
        {
            vibratorControl.vibratorType = (VibratorController.VibratorType)Enum.ToObject(typeof(VibratorController.VibratorType), index);
            XDebug.Log("Vibrator Type = " + vibratorControl.vibratorType);
        }
    }


    //Notification demo
    public void ShowToastNotificationLook()
    {
        if (toastControl != null)
            toastControl.Show(notifLookMessage.TextByLanguage(localizeLanguage));
    }


    //Yes/No, OK dialog demo
    public void OnResponse(string result)
    {
        XDebug.Log("result = " + result);

        switch (result)
        {
            case "yes":
                ShowToast(yesResponseMessage.TextByLanguage(localizeLanguage));
                break;
            case "no":
                ShowToast(noResponseMessage.TextByLanguage(localizeLanguage));
                break;
            case "ok":
                ShowToast(okResponseMessage.TextByLanguage(localizeLanguage));
                break;
        }
    }

    //Yes/No, OK dialog with CheckBox demo
    public void OnResponse(string result, bool check)
    {
        XDebug.Log("result = " + result + ", checked = " + check);
        ShowToast("result = " + result + ", checked = " + check);
    }


    //Callback handler for 'LocalizeLanguageChanger'
    public void OnLanguageChanged(SystemLanguage language)
    {
        if (language == SystemLanguage.Unknown)
            XDebug.Log("Localize language changed : (System language)");
        else
            XDebug.Log("Localize language changed : " + language);

        localizeLanguage = language;

        //Sync dropdown on start up (Basically, when load setting (PlayerPrefs) and changed only)
        if (localizeDropdown != null && language != SystemLanguage.Unknown)
        {
            if (!localizeDropdown.IsSelectedLanguage(language))
                localizeDropdown.SetLanguage(language);
        }
    }

}
