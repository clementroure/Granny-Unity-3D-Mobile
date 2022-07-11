using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Speech Recognizer demo using controllers and localize
//音声認識でコントローラ（～Controller）とローカライズを利用したデモ
public class SpeechRecognizerTest3 : MonoBehaviour {

    public Text displayText;
    public Toggle webSearchToggle;
    public Button recongizerButton;
    public Animator circleAnimator;
    public Animator voiceAnimator;


    //Message when recognizer start.
    public LocalizeString recognizerStartMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Starting Recognizer..."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "音声認識を起動してます…"),
        });

    //Message when recognizer ready.
    public LocalizeString recognizerReadyMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Waiting voice..."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "音声を待機中…"),
        });

    //Message when recognizer begin.
    public LocalizeString recognizerBeginMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Recognizing voice..."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "音声を取得しています…"),
        });

    public bool isRecognitionMultiChoice = true;        //Use 'MultiChoice' for word selection methods (false is 'SingleChoice').
    public bool appendDateTimeStringOnSave = true;      //Add a DateTime string to the file name.


    //SpeechRecognizer
    public SpeechRecognizerController speechRecognizerControl;
    public SpeechRecognizerDialogController speechRecognizerDialog;

    //Mainly 'WebSearchController.StartSearch' is called.
    public WebSearchController webSearchControl;

    //Mainly 'SelectDialogController.Show' is called.
    public SelectDialogController selectDialogControl;

    //Mainly 'SingleChoiceDialogController.Show' is called.
    public SingleChoiceDialogController singleChoiceDialogControl;

    //Mainly 'MultiChoiceDialogController.Show' is called.
    public MultiChoiceDialogController multiChoiceDialogControl;

    //Mainly 'StorageSaveTextController.Show, .CurrentValue' is called.
    public StorageSaveTextController storageSaveTextControl;

    //When dynamically generating file name to save.
    //保存するファイル名を動的に生成するとき
    public bool autoSavefileName = true;



    //==========================================================

    // Use this for initialization
    private void Start () {
        //if (speechRecognizerDialog != null)
        //{
        //    DisplayText(displayText.text == "" ? "" : "\n", true); 
        //    DisplayText("IsSupportedRecognizer = " + speechRecognizerDialog.IsSupportedRecognizer + "\n"
        //        + "'RECORD_AUDIO' permission = " + speechRecognizerDialog.IsPermissionGranted, true);
        //}
    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}


    //==========================================================
    //Display text string

    //Display text string (and for reading)
    public void DisplayText(object message)
    {
        if (displayText != null)
            displayText.text = message.ToString();
    }

    public void DisplayText(object message, bool add)
    {
        if (displayText != null)
        {
            if (add)
                displayText.text += message;
            else
                displayText.text = message.ToString();
        }
    }

    public void DisplayTextLine(object message)
    {
        DisplayText(message + "\n", true);
    }

    public void DisplayText(object[] words)
    {
        if (displayText != null)
            displayText.text = string.Join("\n", words.Select(e => e.ToString()).ToArray());
    }

    public void DisplayPermission(string permission, bool granted)
    {
        if (displayText != null)
            displayText.text += permission.Replace("android.permission.", "") + " = " + granted + "\n";
    }
    
    //==========================================================
    //Function of text etc.
    
    public void SaveText()
    {
        if (storageSaveTextControl != null && displayText != null && !string.IsNullOrEmpty(displayText.text))
        {
            if (autoSavefileName)
            {
                //Make a part of the text of the first line a file name.    //最初の行のテキストの一部をファイル名にする
                string str = displayText.text.Split('\n')[0];   
                string file = str.Substring(0, Mathf.Min(str.Length, 16));
                if (appendDateTimeStringOnSave)
                    file += "_" + DateTime.Now.ToString("yyMMdd_HHmmss");
                if (string.IsNullOrEmpty(file))
                    file = "NewRecognition";
                if (!file.EndsWith(".txt"))
                    file += ".txt";
                storageSaveTextControl.CurrentValue = file;
            }

            storageSaveTextControl.Show(displayText.text);
        }
    }


    //Search words by web.
    public void StartWebSearch(string word)
    {
        if (webSearchControl != null)
            webSearchControl.StartSearch(word);
    }

    //Toggle button (webSearchToggle) to switch WebSearch.
    public void SwitchWebSearch(string[] words)
    {
        if (webSearchToggle != null && webSearchToggle.isOn)
        {
            if (words.Length > 1)
            {
                if (selectDialogControl != null)
                    selectDialogControl.Show(words);
            }
            else
                StartWebSearch(words[0]);    //When there is only one word.
        }
        else
        {
            if (words.Length > 1)
            {
                if (isRecognitionMultiChoice)
                {
                    if (multiChoiceDialogControl != null)
                        multiChoiceDialogControl.Show(words);
                }
                else
                {
                    if (singleChoiceDialogControl != null)
                        singleChoiceDialogControl.Show(words);
                }
            }
            else
                DisplayText(words[0]);    //When there is only one word.
        }
    }


    //==========================================================
    //Example with Google dialog

    //Receive results from speech recognition with dialog.
    public void OnResultSpeechRecognizerDialog(string[] words)
    {
        DisplayText(words);
        SwitchWebSearch(words);
    }


    //==========================================================
    //Example without dialog (Callback handlers)

    //Callback handler for start Speech Recognizer
    public void OnStartRecognizer()
    {
        if (speechRecognizerControl != null)
        {
            if (speechRecognizerControl.IsSupportedRecognizer && speechRecognizerControl.IsPermissionGranted)
            {
                DisplayText(recognizerStartMessage);
                StartUI();
            }
        }
    }

    //Callback handler for microphone standby state
    public void OnReady()
    {
        DisplayText(recognizerReadyMessage);
        ReadyUI();
    }

    ///Callback handler for microphone begin speech recognization state
    public void OnBegin()
    {
        DisplayText(recognizerBeginMessage);
        BeginUI();
    }

    //Receive the result when speech recognition succeed.
    public void OnResult(string[] words)
    {
        ResetUI();
        DisplayText(words);
        SwitchWebSearch(words);
    }

    //Receive the error when speech recognition fail.
    public void OnError(string message)
    {
        ResetUI();
        DisplayText(message);
    }


    //==========================================================
    //UI

    //Start Recognizer UI
    private void StartUI()
    {
        if (recongizerButton != null)
            recongizerButton.interactable = false;
    }

    //Microphone standby UI
    private void ReadyUI()
    {
        if (circleAnimator != null)
            circleAnimator.SetTrigger("ready");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("ready");
    }

    //Microphone begin speech recognization UI
    private void BeginUI()
    {
        if (circleAnimator != null)
            circleAnimator.SetTrigger("speech");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("speech");
    }

    //Reset UI
    public void ResetUI()
    {
        if (circleAnimator != null)
            circleAnimator.SetTrigger("stop");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("stop");

        if (recongizerButton != null)
            recongizerButton.interactable = true;
    }

    //Callback handler for locale change dropdown (OnValueChanged)
    public void OnLocaleValueChanged(Dropdown localeDropdown)
    {
        if (localeDropdown == null)
            return;

        string loc = localeDropdown.captionText.text;
        if (speechRecognizerControl != null)
            speechRecognizerControl.Locale = (loc == AndroidLocale.Default) ? "" : loc; //To make it the system default, put an empty character ("").
        if (speechRecognizerDialog != null)
            speechRecognizerDialog.Locale = (loc == AndroidLocale.Default) ? "" : loc;  //To make it the system default, put an empty character ("").
    }
}
