using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Text To Speech demo using controllers
public class TextToSpeechTest2 : MonoBehaviour {

    public Text displayText;
    public Text statusText;
    public Animator statusAnimator;

    public Text speedText;
    public Text pitchText;

    //Running message
    public string startMessage = "Speaking";    //Message when speech start.        //発声中
    public string doneMessage = "Finished";     //Message when speech finished.     //発声終了
    public string stopMessage = "Interrupted";  //Message when speech interrupted.  //発声中断

    //Initialization/error message
    public string ttsAvailableMessage = "Text To Speech is available.";             //Message when TTS available             //テキスト読み上げが利用できます。
    public string ttsInitializationError = "Failed to initialize Text To Speech.";  //Message when TTS initialization error  //テキスト読み上げの初期化に失敗しました
    public string ttsLocaleError = "It is a language that can not be used.";        //Message when TTS locale error          //利用できない言語です。

    public Button ttsSearchButton;  //Market Search
    public Button ttsButton;        //Google TTS
    public Button ttsButtonN2;      //KDDI N2 TTS

    public AppInstallCheckController OnGooglePlayCheck;
    public AppInstallCheckController OnGoogleTTSCheck;
    public AppInstallCheckController OnKddiTTSCheck;

    //Register 'TextToSpeechController.StartSpeech' in the inspector.
    [Serializable] public class TTSStartHandler : UnityEvent<string> { }
    public TTSStartHandler OnTTSStart;

    //Register 'MultiLineTextDialogController.Show' in the inspector.
    [Serializable] public class TextEditHandler : UnityEvent<string> { }
    public TextEditHandler OnTextEdit;



    // Use this for initialization
    private void Start () {

    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}


    //==========================================================
    //Check the installation of TTS
    //TTSのインストールをチェックする

    const string GooglePlay = "com.android.vending";        //Google Play
    const string TTSPackageName = "com.google.android.tts"; //Google TTS
    const string TTSPackageNameN2 = "jp.kddilabs.n2tts";    //KDDI N2 TTS
    int isInstalledGooglePlay = -1;     //checked flag

    //Check install coding sample
    public void CheckTTS(string ttsPackageName, string ttsName, Button ttsButton)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        if (isInstalledGooglePlay == -1)    //at first time only
            isInstalledGooglePlay = AndroidPlugin.IsExistApplication(GooglePlay) ? 1 : 0;

        if (isInstalledGooglePlay == 1)
        {
            if (ttsSearchButton != null)
                ttsSearchButton.interactable = true;

            if (AndroidPlugin.IsExistApplication(ttsPackageName))
            {
                string appName = AndroidPlugin.GetApplicationName(ttsPackageName);
                string verName = AndroidPlugin.GetVersionName(ttsPackageName);
                DisplayText("\n" + appName + " ver." + verName + " is installed.", true);
                if (ttsButton != null)
                    ttsButton.interactable = false;
            }
            else
            {
                DisplayText("\n" + ttsName + " is not installed.", true);
                if (ttsButton != null)
                    ttsButton.interactable = true;
            }
        }
        else
        {
            DisplayText("\nGoogle Play is not installed.", true);
            if (ttsSearchButton != null)
                ttsSearchButton.interactable = false;
        }
#endif
    }

    //AppInstallCheckController callback handlers (using Controller sample)

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
        DisplayText("\n" + appName + " ver." + verName + " is installed.", true);
        if (ttsButton != null)
            ttsButton.interactable = false;
    }

    public void OnGoogleTTSNotInstalled()
    {
        DisplayText("\nGoogle TTS is not installed.", true);
        if (ttsButton != null)
            ttsButton.interactable = true;
    }

    public void OnKddiTTSInstalled(string appName, int verCode, string verName)
    {
        DisplayText("\n" + appName + " ver." + verName + " is installed.", true);
        if (ttsButtonN2 != null)
            ttsButtonN2.interactable = false;
    }

    public void OnKddiTTSNotInstalled()
    {
        DisplayText("\nKDDI TTS is not installed.", true);
        if (ttsButtonN2 != null)
            ttsButtonN2.interactable = true;
    }


    //==========================================================
    //Display and edit text string
    
    //Display text string (and for reading)
    public void DisplayText(string message, bool add = false)
    {
        if (displayText != null)
        {
            if (add)
                displayText.text += message;
            else
                displayText.text = message;
        }
    }

    //Display status message
    public void DisplayStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
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

    //Call the text(reading) edit dialog
    public void EditText()
    {
        if (OnTextEdit != null && !string.IsNullOrEmpty(displayText.text))
            OnTextEdit.Invoke(displayText.text);
    }

    //Callback handler for text edit dialog result
    public void OnEditText(string text)
    {
        DisplayText(text.Trim());
    }



    //==========================================================
    //Example Text To Speech (Callback handlers)
    
    //TextToSpeechController.StartSpeech call
    public void StartTTS()
    {
        if (OnTTSStart != null)
            OnTTSStart.Invoke(displayText.text);
    }

    //Receive status message from callback
    public void OnStatus(string message)
    {
        DisplayStatus(message);

        if (message.StartsWith("SUCCESS_INIT"))
            DisplayText("\n" + ttsAvailableMessage, true);
        else if (message.StartsWith("ERROR_LOCALE_NOT_AVAILABLE"))
            DisplayText("\n" + ttsInitializationError + "\n" + ttsLocaleError, true);
        else if (message.StartsWith("ERROR_INIT"))
            DisplayText("\n" + ttsInitializationError, true);
        else
            DisplayText("\n" + message, true);

        TextToSpeechController ttsController = FindObjectOfType<TextToSpeechController>();
        if (ttsController != null)
        {
            DisplayText("\nInitializeStatus = " + ttsController.InitializeStatus, true);
            DisplayText(", IsInitializeSuccess = " + ttsController.IsInitializeSuccess, true);
            //DisplayText("\n" + ttsController.SaveKey + " : " + PlayerPrefs.GetString(ttsController.SaveKey), true);    //json
        }

        if (isInstalledGooglePlay == -1)    //at first time only
        {
            //Coding sample
            //if (ttsButton != null)
            //    CheckTTS(TTSPackageName, "Google TTS", ttsButton);
            //if (ttsButtonN2 != null)
            //    CheckTTS(TTSPackageNameN2, "KDDI TTS", ttsButtonN2);

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

}
