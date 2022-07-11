using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using FantomLib;

// Android Speech Recognizer Demo
// http://fantom1x.blog130.fc2.com/blog-entry-273.html#fantomPlugin_SpeechRecognizerDialog
public class SpeechRecognizerTest : MonoBehaviour {

    public GameObject receiveObject;
    public Button speechButton;
    public Animator circleAnimator;
    public Animator voiceAnimator;
    public Text displayText;
    public Toggle webSearchToggle;



    //Speech recognition support property
    private bool mIsSpeechSupport = false;  //Native calls are heavily loaded, so keep them cached.
    private bool speechChecked = false;     //Already checked

    public bool isSpeechSupport {
        get {
            if (!speechChecked)
            {
#if UNITY_EDITOR
                mIsSpeechSupport = true;    //For debug (Editor only)
#elif UNITY_ANDROID
                mIsSpeechSupport = AndroidPlugin.IsSupportedSpeechRecognizer();
#endif
                speechChecked = true;
            }
            return mIsSpeechSupport;
        }
    }



    // Use this for initialization
    private void Start () {
        if (receiveObject == null)
            receiveObject = this.gameObject;

        if (displayText != null)
            displayText.text = "isSpeechSupport = " + isSpeechSupport;
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
    //private void Update () {

    //}


    //==========================================================
    //Android Speech Recognizer with Dialog Demo

    //Android Speech Recognizer with Dialog
    public void ShowSpeechRecognizer()
    {
#if UNITY_EDITOR
        Debug.Log("ShowSpeechRecognizer called");
#elif UNITY_ANDROID
        AndroidPlugin.ShowSpeechRecognizer(receiveObject.name, "ResultSpeechRecognizer", "OK google?");
#endif

        if (displayText != null)
            displayText.text = "";
    }


    //Receive results from Speech Recognizer with Dialog -> Callback handler
    private void ResultSpeechRecognizer(string message)
    {
#if UNITY_EDITOR
        Debug.Log("ResultSpeechRecognizer called");
#endif
        SetDisplayText(message);

        string[] keywords = message.Split('\n');

        if (webSearchToggle != null && webSearchToggle.isOn)
        {
#if UNITY_EDITOR
            StartWebSearch(keywords[0]);    //The first one as a search keyword.
#elif UNITY_ANDROID
            if (keywords.Length > 1)
                AndroidPlugin.ShowSelectDialog("Select a search word", keywords, receiveObject.name, "StartWebSearch");
                //AndroidPlugin.ShowSingleChoiceDialog("Select a search word", keywords, 0, receiveObject.name, "StartWebSearch");
            else
                StartWebSearch(keywords[0]);    //When there is only one.
#endif
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (keywords.Length > 1)
                AndroidPlugin.ShowSelectDialog("Select a word", keywords, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowSingleChoiceDialog("Select a word", keywords, 0, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowMultiChoiceDialog("Select words", keywords, null, receiveObject.name, "SetDisplayText");
            else
                SetDisplayText(keywords[0]);    //When there is only one.
#endif
        }
    }


    //==========================================================
    //Android Speech Recognizer without Dialog Demo

    //Android Speech Recognizer without Dialog
    public void StartSpeechRecognizer()
    {
        if (displayText != null)
            displayText.text = "Starting Speech Recognizer...";

#if UNITY_EDITOR
        Debug.Log("StartSpeechRecognizer");
        StartCoroutine(DebugSimulate());
#elif UNITY_ANDROID
        AndroidPlugin.StartSpeechRecognizer(receiveObject.name, "OnResult", "OnError", "OnReady", "OnBegin");
#endif
        if (speechButton != null)
            speechButton.interactable = false;
    }


    //Callback handler when start waiting for speech recognition.
    private void OnReady(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnReady");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("ready");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("ready");

        if (displayText != null)
            displayText.text = "Waiting speech...";
    }


    //Callback handler when the first voice entered the microphone.
    private void OnBegin(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnBegin");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("speech");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("speech");

        if (displayText != null)
            displayText.text = "Recoginize speech...";
    }


    //Callback handler when recognition is successful.
    private void OnResult(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnResult");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("stop");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("stop");

        if (speechButton != null)
            speechButton.interactable = true;

        SetDisplayText(message);

        string[] keywords = message.Split('\n');

        if (webSearchToggle != null && webSearchToggle.isOn)
        {
#if UNITY_EDITOR
            StartWebSearch(keywords[0]);    //he first one as a search keyword.
#elif UNITY_ANDROID
            if (keywords.Length > 1)
                //AndroidPlugin.ShowSelectDialog("Select a search word", keywords, receiveObject.name, "StartWebSearch");
                AndroidPlugin.ShowSingleChoiceDialog("Select a search word", keywords, 0, receiveObject.name, "StartWebSearch");
            else
                StartWebSearch(keywords[0]);    //When there is only one.
#endif
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (keywords.Length > 1)
                //AndroidPlugin.ShowSelectDialog("Select a word", keywords, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowSingleChoiceDialog("Select a word", keywords, 0, receiveObject.name, "SetDisplayText");
                AndroidPlugin.ShowMultiChoiceDialog("Select words", keywords, null, receiveObject.name, "SetDisplayText");
                //AndroidPlugin.ShowSelectDialog("Select a word", keywords, receiveObject.name, "SetDisplayText", keywords.Select((e, i) => i + ":" + e).ToArray());
                //AndroidPlugin.ShowSingleChoiceDialog("Select a word", keywords, 0, receiveObject.name, "SetDisplayText", keywords.Select((e, i) => i + ":" + e).ToArray());
                //AndroidPlugin.ShowMultiChoiceDialog("Select words", keywords, null, receiveObject.name, "SetDisplayText", keywords.Select((e,i)=>i+":"+e).ToArray());
            else
                SetDisplayText(keywords[0]);    //When there is only one.
#endif
        }
    }


    //Callback handler when recognition is failure or error.
    private void OnError(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnError");
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("stop");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("stop");

        if (speechButton != null)
            speechButton.interactable = true;

        if (displayText != null)
            displayText.text = message;
    }


    //Interrupted speech recognition.
    public void StopSpeechRecognizer()
    {
#if UNITY_EDITOR
        Debug.Log("StopSpeechRecognizer");
#elif UNITY_ANDROID
        AndroidPlugin.ReleaseSpeechRecognizer();
#endif
        if (circleAnimator != null)
            circleAnimator.SetTrigger("stop");

        if (voiceAnimator != null)
            voiceAnimator.SetTrigger("stop");

        if (speechButton != null)
            speechButton.interactable = true;

        if (displayText != null)
            displayText.text = "Speech recognizer has been canceled.";
    }


#if UNITY_EDITOR
    //For debug (Editor only)
    private IEnumerator DebugSimulate()
    {
        OnReady("onReadyForSpeech");
        yield return new WaitForSeconds(2f);

        OnBegin("onBeginningOfSpeech");
        yield return new WaitForSeconds(5f);

        if (Random.Range(0, 10) == 0)   //Error simulate rate
            OnError("ERROR_NO_MATCH");
        else
            OnResult("ok google\noak gargle\noh goo");  //example
    }
#endif


    //Start Web Search
    public void StartWebSearch(string query)
    {
#if UNITY_EDITOR
        Debug.Log("StartWebSearch : query = " + query);
#elif UNITY_ANDROID
        AndroidPlugin.StartWebSearch(query);
        //AndroidPlugin.StartAction("android.intent.action.WEB_SEARCH", "query", query);  //Same as Web Search
#endif
    }


    //Show the text
    public void SetDisplayText(string message)
    {
        if (displayText != null)
            displayText.text = message;
    }

}
