using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Text To Speech demo using controllers
public class TextToSpeechTest3 : MonoBehaviour {

    //Maily 'TextToSpeechController.StartSpeech', '.Locale' is called.
    public TextToSpeechController textToSpeechControl;

    public Text displayText;
    public Text statusText;
    public Animator statusAnimator;

    public Text speedText;
    public Text pitchText;

    public Dropdown localeDropdown;
    public bool useAndroidLocale = true;

    //Running messages
    //Message when speech start.
    public LocalizeString startMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Speaking"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "発声中"),
        });

    //Message when speech finished.
    public LocalizeString doneMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Finished"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "発声終了"),
        });

    //Message when speech interrupted.
    public LocalizeString stopMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Interrupted"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "発声中断"),
        });


    //Initialization/error messages
    //Message when TTS available 
    public LocalizeString ttsAvailableMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Text To Speech is available."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "テキスト読み上げが利用できます。"),
        });

    //Message when TTS initialization error
    public LocalizeString ttsInitializationError = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Failed to initialize Text To Speech."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "テキスト読み上げの初期化に失敗しました。"),
        });

    //Message when TTS locale error
    public LocalizeString ttsLocaleError = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "It is a language that can not be used."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "利用できない言語です。"),
        });

    //TTS
    public Button ttsSearchButton;  //Market Search
    public Button ttsButton;        //Google TTS
    public Button ttsButtonN2;      //KDDI N2 TTS

    public AppInstallCheckController OnGooglePlayCheck;
    public AppInstallCheckController OnGoogleTTSCheck;
    public AppInstallCheckController OnKddiTTSCheck;


    //==========================================================

    // Use this for initialization
    private void Start () {
        if (useAndroidLocale && localeDropdown != null)
        {
            localeDropdown.ClearOptions();
            List<string> list = new List<string>(AndroidLocale.ConstantValues);
            localeDropdown.AddOptions(list);
            localeDropdown.value = 0;

            if (textToSpeechControl != null)
            {
                int idx = list.IndexOf(textToSpeechControl.Locale);
                if (idx >= 0)
                    localeDropdown.value = idx;     //'OnValueChanged' event occurs
            }
        }
    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}


    //==========================================================
    //Check the installation of TTS
    //TTSのインストールをチェックする
    //AppInstallCheckController callback handlers (using Controller sample)

    int isInstalledGooglePlay = -1;     //checked flag

    public void OnGooglePlayInstalled(string appName, int verCode, string verName)
    {
        isInstalledGooglePlay = 1;
        if (ttsSearchButton != null)
            ttsSearchButton.interactable = true;
    }

    public void OnGooglePlayNotInstalled()
    {
        isInstalledGooglePlay = 0;
        if (ttsSearchButton != null)
            ttsSearchButton.interactable = false;
    }

    public void OnGoogleTTSInstalled(string appName, int verCode, string verName)
    {
        DisplayText(appName + " ver." + verName + " is installed.", true, true);
        if (ttsButton != null)
            ttsButton.interactable = false;
    }

    public void OnGoogleTTSNotInstalled()
    {
        DisplayText("Google TTS is not installed.", true, true);
        if (ttsButton != null)
            ttsButton.interactable = true;
    }

    public void OnKddiTTSInstalled(string appName, int verCode, string verName)
    {
        DisplayText(appName + " ver." + verName + " is installed.", true, true);
        if (ttsButtonN2 != null)
            ttsButtonN2.interactable = false;
    }

    public void OnKddiTTSNotInstalled()
    {
        DisplayText("KDDI TTS is not installed.", true, true);
        if (ttsButtonN2 != null)
            ttsButtonN2.interactable = true;
    }


    //==========================================================
    //Display and edit text string
    
    //Display text string (and for reading)
    public void DisplayText(object message, bool add = false, bool newline = false)
    {
        if (displayText != null)
        {
            if (add)
                displayText.text += message + (newline ? "\n" : "");
            else
                displayText.text = message + (newline ? "\n" : "");
        }
    }

    //Display status message
    public void DisplayStatus(object message)
    {
        if (statusText != null)
            statusText.text = message.ToString();
    }

    //Display speech speed
    public void DisplaySpeed(float speed)
    {
        if (speedText != null)
            speedText.text = string.Format("Speed : {0:F2}", speed);
    }

    //Display voice pitch
    public void DisplayPitch(float pitch)
    {
        if (pitchText != null)
            pitchText.text = string.Format("Pitch : {0:F2}", pitch);
    }


    //==========================================================
    //Example Text To Speech (Callback handlers)
    
    //TextToSpeechController.StartSpeech call
    public void StartTTS()
    {
        if (textToSpeechControl != null)
            textToSpeechControl.StartSpeech(displayText.text);
    }

    //Receive status message from callback
    public void OnStatus(string message)
    {
        DisplayStatus(message);

        if (message.StartsWith("SUCCESS_INIT"))
            DisplayText(ttsAvailableMessage, true, true);
        else if (message.StartsWith("ERROR_LOCALE_NOT_AVAILABLE"))
            DisplayText(ttsInitializationError + "\n" + ttsLocaleError, true, true);
        else if (message.StartsWith("ERROR_INIT"))
            DisplayText(ttsInitializationError, true, true);
        else
            DisplayText(message, true, true);

        if (textToSpeechControl != null)
        {
            DisplayText("InitializeStatus = " + textToSpeechControl.InitializeStatus, true);
            DisplayText(", IsInitializeSuccess = " + textToSpeechControl.IsInitializeSuccess, true, true);
            //DisplayText("\n" + textToSpeechControl.SaveKey + " : " + PlayerPrefs.GetString(textToSpeechControl.SaveKey), true);    //json
        }

        if (isInstalledGooglePlay == -1)    //at first time only
        {
            //Using Controller sample
            if (OnGooglePlayCheck != null)
                OnGooglePlayCheck.CheckInstall();
            if (OnGoogleTTSCheck != null)
                OnGoogleTTSCheck.CheckInstall();
            if (OnKddiTTSCheck != null)
                OnKddiTTSCheck.CheckInstall();
        }
    }

    //Callback handler for start speaking
    public void OnStart()
    {
        if (statusAnimator != null)
            statusAnimator.SetTrigger("blink");

        DisplayStatus(startMessage);
    }

    //Callback handler for finish speaking
    public void OnDone()
    {
        if (statusAnimator != null)
            statusAnimator.SetTrigger("stop");

        DisplayStatus(doneMessage);
    }

    //Callback handler for interrupt speaking
    public void OnStop(string message)
    {
        if (statusAnimator != null)
            statusAnimator.SetTrigger("stop");

        DisplayStatus(stopMessage);
    }

    //Callback handler for locale change dropdown (OnValueChanged)
    public void OnLocaleValueChanged(Dropdown localeDropdown)
    {
        if (localeDropdown == null)
            return;

        string loc = localeDropdown.captionText.text;
        DisplayText("Locale changed : " + loc);
        if (textToSpeechControl != null)
            textToSpeechControl.Locale = (loc == AndroidLocale.Default) ? "" : loc; //To make it the system default, put an empty character ("").
    }
}
