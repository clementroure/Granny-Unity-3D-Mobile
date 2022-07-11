using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FantomLib;

// Android Widgets Demo
// http://fantom1x.blog130.fc2.com/blog-entry-273.html
public class PluginTest : MonoBehaviour
{

    public Text languageText;               //Display of system language
    public string openURL;                  //URL to open with button -> Notification
    public Toggle ynCheckToggle;            //Make it a Yes/No Dialog with a checkbox
    public Toggle okCheckToggle;            //Make it a OK Dialog with a checkbox
    public Toggle switchKeyToggle;          //Make the return value of the Switch Dialog a format "key=value"
    public Toggle hardVolToggle;            //Whether Hardware Volume buttons can be used (for property synchronization at startup)

    const string CHECKED_PREF = "_checked"; //For saving the state of the checkbox (PlayerPrefs)

#if UNITY_EDITOR
    public Color debugColor;                //Color format conversion input (Editor only Demo)
    public Color outColor;                  //Color format conversion output (Editor only Demo)
#endif

    // Use this for initialization
    private void Start()
    {
        if (languageText != null)
        {
            SystemLanguage lang = Application.systemLanguage;
            var strVal = Enum.GetName(typeof(SystemLanguage), lang);
            languageText.text = "Language = " + (int)lang + " (" + strVal + ")";
        }

        bool check = XPlayerPrefs.GetBool(gameObject.name + CHECKED_PREF, true);    //It saves the boolean value
        if (ynCheckToggle != null)
            ynCheckToggle.isOn = check;

        if (okCheckToggle != null)
            okCheckToggle.isOn = check;

        if (hardVolToggle != null)
            hardVolToggle.isOn = FindObjectOfType<HardVolumeController>().HardOperation;

#if UNITY_EDITOR
        //Color format conversion Demo (Editor only Demo)
        string htmlString = ColorUtility.ToHtmlStringRGBA(debugColor);
        Debug.Log("htmlString = " + htmlString);
        Debug.Log("GetColorCodeString = " + XColor.GetColorCodeString(htmlString));
        Debug.Log("RedValue(html)  = " + XColor.RedValue(htmlString));
        Debug.Log("RedValue(Color) = " + debugColor.RedValue());
        Debug.Log("GreenValue(html)  = " + XColor.GreenValue(htmlString));
        Debug.Log("GreenValue(Color) = " + debugColor.GreenValue());
        Debug.Log("BlueValue(html)  = " + XColor.BlueValue(htmlString));
        Debug.Log("BlueValue(Color) = " + debugColor.BlueValue());
        Debug.Log("AlphaValue(html)  = " + XColor.AlphaValue(htmlString));
        Debug.Log("AlphaValue(Color) = " + debugColor.AlphaValue());
        int argb = debugColor.ToIntARGB();
        Debug.Log("ToIntARGB = " + argb + ", ToColor->html = " + ColorUtility.ToHtmlStringRGBA(XColor.ToColor(argb)));
        outColor = XColor.ToColor(argb);
#endif
    }


    public void OnDestroy()     //To call even when the application is closed -> public
    {
#if UNITY_EDITOR
        Debug.Log("AndroidPlugin.Release called");
#elif UNITY_ANDROID
        AndroidPlugin.Release();
#endif
    }


    // Update is called once per frame
    //private void Update()
    //{

    //}


    //Call Android Toast
    public void ShowToast(string message)
    {
#if UNITY_EDITOR
        Debug.Log("ShowToast : " + message);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast(message);
#endif
    }


    //Call Android Toast with delay
    private IEnumerator DelayedToast(string message, float seconds, bool longDuration = false)
    {
        yield return new WaitForSeconds(seconds);
#if UNITY_EDITOR
        Debug.Log("DelayedToast : message = " + message);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast(message, longDuration);
#endif
    }


    //Android Toast Demo (show time now)
    public void TestToast()
    {
        DateTime dt = DateTime.Now;
        ShowToast("Time Now - " + dt.ToString("HH:mm:ss"));
    }


    //Interrupt display of Android Toast
    public void CancelToast()
    {
#if UNITY_EDITOR
        Debug.Log("CancelToast called");
#elif UNITY_ANDROID
        AndroidPlugin.CancelToast();
#endif
    }


    //Android Yes/No Dialog Demo 
    public void TestDialogYesNo()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogYesNo called");
#elif UNITY_ANDROID
        if (ynCheckToggle != null && ynCheckToggle.isOn)
            AndroidPlugin.ShowDialogWithCheckBox("This is 'Yes/No' Dialog Demo", "Are you ready?", "Remember me", Color.cyan, ynCheckToggle.isOn, gameObject.name, "OnReceiveChecked", "Yes", "Ok, Let's go!", "No", "Well, I'm waiting...", "android:Theme.DeviceDefault.Dialog.Alert");
        else
            AndroidPlugin.ShowDialog("This is 'Yes/No' Dialog Demo", "Are you ready?", gameObject.name, "ShowToast", "Yes", "Ok, Let's go!", "No", "Well, I'm waiting...", "android:Theme.DeviceDefault.Light.Dialog.Alert");
#endif
    }


    //Android OK Dialog Demo 
    public void TestDialogOK()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogOK called");
#elif UNITY_ANDROID
        if (okCheckToggle != null && okCheckToggle.isOn)
            AndroidPlugin.ShowDialogWithCheckBox("This is 'OK' Dialog Demo", "Let's enjoy creative life!", "Remember me", 0, okCheckToggle.isOn, gameObject.name, "OnReceiveChecked", "OK", "This world is amazing!", "android:Theme.DeviceDefault.Light.Dialog.Alert");
        else
            AndroidPlugin.ShowDialog("This is 'OK' Dialog Demo", "Let's enjoy creative life!", gameObject.name, "ShowToast", "OK", "This world is wonderful!", "android:Theme.DeviceDefault.Dialog.Alert");
#endif
    }


    //Android Dialog with checkbox -> Callback handler
    private void OnReceiveChecked(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnReceiveChecked : " + message);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast(message);
#endif
        bool check = message.Contains("CHECKED_TRUE");  //"CHECKED_FALSE" when false

        if (ynCheckToggle != null)
            ynCheckToggle.isOn = !check;

        if (okCheckToggle != null)
            okCheckToggle.isOn = !check;

        XPlayerPrefs.SetBool(gameObject.name + CHECKED_PREF, !check);   //Save a boolean value (PlayerPrefs)
        PlayerPrefs.Save();
    }


    //Android Select Dialog Demo
    public void TestDialogSelect()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogSelect called");
#elif UNITY_ANDROID
        string[] items = { "Saber", "Rancer", "Archer", "Caster", "Assassin" };
        AndroidPlugin.ShowSelectDialog("Select a Servant", items, gameObject.name, "ShowToast");
#endif
    }


    //Android Single Choice Dialog Demo
    public void TestDialogSingleChoice()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogSingleChoice called");
#elif UNITY_ANDROID
        string[] items = { "Saber", "Rancer", "Archer", "Caster", "Assassin" };
        AndroidPlugin.ShowSingleChoiceDialog("Select a Servant", items, 0, gameObject.name, "ShowToast");
#endif
    }


    //Android Multi Choice Dialog Demo
    public void TestDialogMultiChoice()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogMultiChoice called");
#elif UNITY_ANDROID
        string[] items = { "Saber", "Rancer", "Archer", "Caster", "Assassin" };
        AndroidPlugin.ShowMultiChoiceDialog("Select Servants", items, null, gameObject.name, "ShowToast");
#endif
    }



    //Android Notification Demo
    public void TestNotification()
    {
#if UNITY_EDITOR
        Debug.Log("TestNotification called");
#elif UNITY_ANDROID
        if (!string.IsNullOrEmpty(openURL))
            AndroidPlugin.ShowNotificationToOpenURL(Application.productName, "Open the homepage!", openURL);
        else
            AndroidPlugin.ShowNotification(Application.productName, "Display only.");

        AndroidPlugin.ShowToast("Look at the Notification!");
#endif
    }


    //Start Open URL Demo
    public void TestOpenURL()
    {
#if UNITY_EDITOR
        Debug.Log("StartOpenURL : url = " + openURL);
        Application.OpenURL(openURL);
#elif UNITY_ANDROID
        AndroidPlugin.StartOpenURL(openURL);    //Open homepage
        //AndroidPlugin.StartActionURI("android.intent.action.VIEW", openURL);  //Same as "StartOpenURL()"
#endif
    }


    //Start Action to URI Demo (Open URL, Open Google Map or StreetView)
    public void TestActionURI(int what = 0)
    {
#if UNITY_EDITOR
        Debug.Log("StartActionURI");
#elif UNITY_ANDROID
        switch (what)
        {
            default:
            case 0:
                AndroidPlugin.StartActionURI("android.intent.action.VIEW", openURL);  //Open homepage (Same as "StartOpenURL()")
                break;
            case 1:
                AndroidPlugin.StartActionURI("android.intent.action.VIEW", "geo:37.7749,-122.4194?q=restaurants");   //Google Map (search word: restaurants)
                break;
            case 2:
                AndroidPlugin.StartActionURI("android.intent.action.VIEW", "google.streetview:cbll=29.9774614,31.1329645&cbp=0,30,0,0,-15");   //Street View
                break;
            case 3:
                AndroidPlugin.StartActionURI("android.intent.action.SENDTO", "mailto:xxx@example.com");   //Launch mailer
                break;
        }
#endif
    }


#pragma warning disable 0414    //The private field value is never used

    //Android Text Input Dialog Demo

    private string editText = "hogehoge";   //Initial text string

    //Android Single Line Text Dialog Demo
    public void TestDialogText()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogText called");
#elif UNITY_ANDROID
        //AndroidPlugin.ShowSingleLineTextDialog("Edit text", editText, 16, gameObject.name, "OnReceiveText");
        AndroidPlugin.ShowSingleLineTextDialog("Edit text", "It is message", editText, 16, gameObject.name, "OnReceiveText");
#endif
    }


    //Android Multi Line Text Dialog Demo
    public void TestDialogTextMulitLine()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogTextMulitLine called");
#elif UNITY_ANDROID
        //AndroidPlugin.ShowMultiLineTextDialog("Edit text", editText, 0, 5, gameObject.name, "OnReceiveText");
        AndroidPlugin.ShowMultiLineTextDialog("Edit text", "It is message", editText, 0, 5, gameObject.name, "OnReceiveText");
#endif
    }


    //Android Text Input Dialog -> Callback handler
    private void OnReceiveText(string message)
    {
        editText = message.Trim();
        ShowToast(editText);
    }


    //Android Numeric Text Dialog Demo
    public void TestDialogNumeric()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogNumeric called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowNumericTextDialog("Numeric text only", "Only unsigned integers\n(Limit: 6 digits)", 123, 6, false, false, gameObject.name, "ShowToast");
        //AndroidPlugin.ShowNumericTextDialog("Allow Decimal with sign", -3.14f, 6, true, true, gameObject.name, "ShowToast");
#endif
    }


    //Android Alpha Numeric Text Dialog Demo
    public void TestDialogAlphaNumeric()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogAlphaNumeric called");
#elif UNITY_ANDROID
        //AndroidPlugin.ShowAlphaNumericTextDialog("Alpha Numeric text only", "No symbol", "", 8, "", gameObject.name, "ShowToast");
        AndroidPlugin.ShowAlphaNumericTextDialog("Alpha Numeric text\nand allow (_-)", "alpha_numeric-", 16, "_-", gameObject.name, "ShowToast");
#endif
    }


    //Android Password Text Dialog Demo
    public void TestDialogPassword()
    {
#if UNITY_EDITOR
        Debug.Log("TestDialogPassword called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowPasswordTextDialog("Enter Password", "password", 16, false, this.gameObject.name, "ShowToast");
        //AndroidPlugin.ShowPasswordTextDialog("Enter PIN", "Just 4 digits", "", 4, true, this.gameObject.name, "ShowToast");
#endif
    }


    //Android DatePicker Dialog Demo
    public void TestDatePicker()
    {
#if UNITY_EDITOR
        Debug.Log("TestDatePicker called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowDatePickerDialog("", "yyyy/M/d", this.gameObject.name, "ShowToast");
#endif
    }


    //Android TimePicker Dialog Demo
    public void TestTimePicker()
    {
#if UNITY_EDITOR
        Debug.Log("TestDatePicker called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowTimePickerDialog("", "H:mm", this.gameObject.name, "ShowToast");
#endif
    }


    //Switch Dialog parameters
    string[] switchItems = { "UnityChan", "PronamaChan", "QueryChan", "SapphiartChan", "AliciaSolid", "TohokuZunko", "OcuTan" };
    string[] switchKeys = { "utc", "pronama", "query", "sapphiart", "alicia", "zunko", "ocutan" };
    bool[] switchChecks;                        //For save Switch state (null -> all false)
    const string SWITCH_PREF = "_switches";     //add name (PlayerPrefs) 

    //Android Switch Dialog Demo
    public void TestDialogSwitch()
    {
        if (switchChecks == null)   
            switchChecks = XPlayerPrefs.GetArray(gameObject.name + SWITCH_PREF, new bool[switchItems.Length]);  //(*) Note that the array will be the saved length

        //(*) Activate the "DebugConsole" in the hierarchy to see its parameters.
        XDebug.Clear();
        XDebug.Log("(PlayerPrefs or init)");
        XDebug.Log(switchChecks.Select(e => e.ToString()).Aggregate((s, a) => s + ", " + a), 3);

#if UNITY_EDITOR
        Debug.Log("TestDialogSwitch called");
#elif UNITY_ANDROID
        string[] keys = (switchKeyToggle != null && !switchKeyToggle.isOn) ? null : switchKeys;
        AndroidPlugin.ShowSwitchDialog("This is Switch Dialog Demo", "Switch the Party Character", switchItems, keys, switchChecks, 0, gameObject.name, "ReceiveSwitches", "OK", "Cancel");
#endif
    }

    //Android Switch Dialog -> Callback handler
    private void ReceiveSwitches(string message)
    {
#if UNITY_EDITOR
        Debug.Log("ReceiveSwitches : message = " + message);
#endif
        ShowToast(message);

        //(*) Activate the "DebugConsole" in the hierarchy to see its parameters.
        XDebug.Clear();
        XDebug.Log("(ReceiveSwitches : message)");
        XDebug.Log(message, 3);

        string str = "";
        string[] arr = message.Split('\n');
        for (int i = 0; i < arr.Length; i++)
        {
            switchChecks[i] = arr[i].EndsWith("true");
            if (switchChecks[i])
                str += switchItems[i] + " entered the party.\n";
        }

        XPlayerPrefs.SetArray(gameObject.name + SWITCH_PREF, switchChecks); //Save array values (PlayerPrefs)
        StartCoroutine(DelayedToast(str.Trim(), 3));     //Show Android Toast with delay
    }

}