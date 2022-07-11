using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FantomLib;

// Android Custiom Dialog Demo
// http://fantom1x.blog130.fc2.com/blog-entry-282.html
public class CustomDialogTest : MonoBehaviour
{

    public Toggle toggleJson;           //When checked, it becomes JSON format

    public VolumeSliderDialogController volumeController;


#pragma warning disable 0219, 0649    //Variable, Field is assigned but its value is never used

    // Use this for initialization
    private void Start()
    {
        if (volumeController == null)
            volumeController = FindObjectOfType<VolumeSliderDialogController>();
    }

    // Update is called once per frame
    //private void Update()
    //{

    //}


    //For receive JSON parameters
    [Serializable]
    class Data
    {
        public bool utc;
        public bool pronama;
        public bool query;
        public bool reset;
        public string servant = "saber";
        public int master = 100;
        public int bgm = 50;
        public int voice = 50;
        public int se = 50;

        public override string ToString()
        {
            return "utc = " + utc + ", pronama = " + pronama + ", query = " + query + ", servant = " + servant
                + ", master = " + master + ", bgm = " + bgm + ", voice = " + voice + ", se = " + se + ", reset = " + reset;
        }
    }

    const string JSON_PREF = "_json";   //For save JSON parameters

    //Call Android Custom Dialog
    //http://fantom1x.blog130.fc2.com/blog-entry-282.html#fantomPlugin_CustomDialogItems
    public void OpenDialog()
    {
        if (toggleJson != null && toggleJson.isOn)  //JSON format
        {
#if UNITY_ANDROID
            Data data = new Data();
            XPlayerPrefs.GetObjectOverwrite(gameObject.name + JSON_PREF, ref data); //Default value when there is no saved data.

            //(*) Activate the "DebugConsole" in the hierarchy to see its parameters.
            XDebug.Clear();
            XDebug.Log("(PlayerPrefs or init)");
            XDebug.Log(data, 3);

            DivisorItem divisorItem = new DivisorItem(1);
            TextItem textItem = new TextItem("You can make various settings.");
            TextItem textItem1 = new TextItem("Switch the Party Character");

            SwitchItem switchItem = new SwitchItem("UnityChan", "utc", data.utc);
            SwitchItem switchItem2 = new SwitchItem("PronamaChan", "pronama", data.pronama);
            SwitchItem switchItem3 = new SwitchItem("QueryChan", "query", data.query);

            TextItem textItem2 = new TextItem("Select a Servant");

            ToggleItem toggleItem = new ToggleItem(
                    new String[] { "Saber", "Rancer", "Caster" },
                    "servant",
                    new String[] { "saber", "rancer", "caster" },
                    data.servant);

            TextItem textItem3 = new TextItem("Sound Setting");

            Dictionary<string, int> vols;
            if (volumeController != null)
                vols = volumeController.GetVolumes();
            else
                vols = new Dictionary<string, int>() { { "master", 100 }, { "bgm", 50 }, { "voice", 50 }, { "se", 50 } };

            SliderItem sliderItem = new SliderItem("Master", "master", vols["master"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem1 = new SliderItem("Music", "bgm", vols["bgm"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem2 = new SliderItem("Voice", "voice", vols["voice"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem3 = new SliderItem("Effect", "se", vols["se"], 0, 100, 0, 0, "PreviewVolume");

            TextItem textItem4 = new TextItem("All saved settings will be deleted when Reset.", Color.red);
            SwitchItem switchItem4 = new SwitchItem("Reset Setting", "reset", false, Color.blue);

            DialogItem[] items = new DialogItem[] {
                        textItem, divisorItem,
                        textItem1, switchItem, switchItem2, switchItem3, divisorItem,
                        textItem2, toggleItem, divisorItem,
                        textItem3, sliderItem, sliderItem1, sliderItem2, sliderItem3, divisorItem,
                        switchItem4, textItem4, divisorItem,
                    };
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidPlugin.ShowCustomDialog("This is Custom Dialog Demo", "", items, gameObject.name, "OnReceiveResult", true, "Apply", "Cancel");
#endif
        }
        else  //"key=value" format
        {
#if UNITY_ANDROID
            //'Param' class is basically the same as Dictionary prepared for easy handling of value type conversion and default value.
            Param pref = Param.GetPlayerPrefs(gameObject.name, new Param());  //When there is no saved data, it is newly generated (elements are empty).

            //(*) Activate the "DebugConsole" in the hierarchy to see its parameters.
            XDebug.Clear();
            XDebug.Log("(PlayerPrefs or init)");
            XDebug.Log(pref, 3);

            DivisorItem divisorItem = new DivisorItem(1);
            TextItem textItem = new TextItem("You can make various settings.");
            TextItem textItem1 = new TextItem("Switch the Party Character", AndroidColor.WHITE, XColor.ToIntARGB("#ff1493"), "center");  //(*) All color formats are the same (only trying on various tests)

            SwitchItem switchItem = new SwitchItem("UnityChan", "utc", pref.GetBool("utc", false));
            SwitchItem switchItem2 = new SwitchItem("PronamaChan", "pronama", pref.GetBool("pronama", false));
            SwitchItem switchItem3 = new SwitchItem("QueryChan", "query", pref.GetBool("query", false));

            TextItem textItem2 = new TextItem("Select a Servant", XColor.ToColor("#fff"), XColor.ToColor("0x1e90ff"), "center");  //(*) All color formats are the same (only trying on various tests)

            ToggleItem toggleItem = new ToggleItem(
                    new String[] { "Saber", "Rancer", "Caster" },
                    "servant",
                    new String[] { "saber", "rancer", "caster" },
                    pref.GetString("servant", "saber"));

            TextItem textItem3 = new TextItem("Sound Setting", XColor.ToIntARGB(Color.white), XColor.ToIntARGB(0x3c, 0xb3, 0x71), "center");//"#3cb371" (*) All color formats are the same (only trying on various tests)

            Dictionary<string, int> vols;
            if (volumeController != null)
                vols = volumeController.GetVolumes();
            else
                vols = new Dictionary<string, int>() { { "master", 100 }, { "bgm", 50 }, { "voice", 50 }, { "se", 50 } };

            SliderItem sliderItem = new SliderItem("Master", "master", vols["master"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem1 = new SliderItem("Music", "bgm", vols["bgm"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem2 = new SliderItem("Voice", "voice", vols["voice"], 0, 100, 0, 0, "PreviewVolume");
            SliderItem sliderItem3 = new SliderItem("Effect", "se", vols["se"], 0, 100, 0, 0, "PreviewVolume");

            TextItem textItem4 = new TextItem("All saved settings will be deleted when Reset.", Color.red);
            SwitchItem switchItem4 = new SwitchItem("Reset Setting", "reset", false, Color.blue);

            DialogItem[] items = new DialogItem[] {
                        textItem, divisorItem,
                        textItem1, switchItem, switchItem2, switchItem3, divisorItem,
                        textItem2, toggleItem, divisorItem,
                        textItem3, sliderItem, sliderItem1, sliderItem2, sliderItem3, divisorItem,
                        switchItem4, textItem4, divisorItem,
                    };
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidPlugin.ShowCustomDialog("This is Custom Dialog Demo", "", items, gameObject.name, "OnReceiveResult", false, "Apply", "Cancel");
#endif
        }
    }


    //When "OK", the setting completion callback handler
    //http://fantom1x.blog130.fc2.com/blog-entry-282.html#fantomPlugin_CustomDialog_result
    private void OnReceiveResult(string message)
    {
        //(*) Activate the "DebugConsole" in the hierarchy to see its parameters.
        XDebug.Clear();
        XDebug.Log("(OnReceiveResult message)");
        XDebug.Log(message, 3);

        if (!string.IsNullOrEmpty(message))
        {
            Dictionary<string, int> vols = volumeController.GetVolumes();  //For save volume

            if (toggleJson != null && toggleJson.isOn)  //JSON format
            {
                Data data = JsonUtility.FromJson<Data>(message);
                XDebug.Log("(Parse to Data [from JSON])");
                XDebug.Log(data, 3);

                if (data.reset)  //Execute setting reset
                {
                    PlayerPrefs.DeleteKey(gameObject.name + JSON_PREF);
                    volumeController.ResetVolumes();     //Return to initial state

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast("The setting was Reset");
#endif
                }
                else  //Update and save values
                {
                    //Update volume setting (*) if used Slider change callback "PreviewVolume()", it is applied in real-time, so it is not necessary.
                    vols["master"] = data.master;
                    vols["bgm"] = data.bgm;
                    vols["voice"] = data.voice;
                    vols["se"] = data.se;

                    //Save parameters (PlayerPrefs)
                    XPlayerPrefs.SetObject(gameObject.name + JSON_PREF, data);
                    volumeController.SetPrefs(vols);
                    PlayerPrefs.Save();

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast(message);
#endif
                }
            }
            else  //"key=value" format
            {
                Param pref = Param.Parse(message);
                XDebug.Log("(Parse to Param [from key=value])");
                XDebug.Log(pref, 3);

                if (pref["reset"].ToLower() == "true")  //Execute setting reset
                {
                    PlayerPrefs.DeleteKey(gameObject.name);
                    volumeController.ResetVolumes();     //Return to initial state

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast("The setting was Reset");
#endif
                }
                else  //Update and save values
                {
                    //Update volume setting (*) if used Slider change callback "PreviewVolume()", it is applied in real-time, so it is not necessary.
                    foreach (var key in vols.Keys.ToArray())
                    {
                        vols[key] = int.Parse(pref[key]);
                        pref.Remove(key);   //Remove unnecessary parameters for saving
                    }
                    pref.Remove("reset");   //Remove unnecessary parameters for saving

                    //Save parameters (PlayerPrefs)
                    if (pref.Count > 0)
                        Param.SetPlayerPrefs(gameObject.name, pref);
                    volumeController.SetPrefs(vols);
                    PlayerPrefs.Save();

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidPlugin.ShowToast(message);
#endif
                }
            }
        }
    }


    //Preview playback callback handler ('key' required)
    private void PreviewVolume(string message)
    {
#if UNITY_EDITOR
        Debug.Log("PreviewVolume : " + message);
#endif
        if (!string.IsNullOrEmpty(message) && volumeController != null)
        {
            string[] param = message.Split('=');  //"key=value" format only
            if (param.Length > 1)
            {
                //Select AudioSource from the key
                string key = param[0];
                volumeController.Play(key);

                //Set a software volume
                float vol = float.Parse(param[1]);
                volumeController.SetVolume(key, vol);
            }
        }
    }

}
