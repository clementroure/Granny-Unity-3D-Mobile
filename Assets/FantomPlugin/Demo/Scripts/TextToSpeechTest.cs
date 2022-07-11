using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using FantomLib;

// Android Text To Speech Demo
// http://fantom1x.blog130.fc2.com/blog-entry-275.html
public class TextToSpeechTest : MonoBehaviour
{

    public GameObject receiveObject;
    public Text displayText;
    public Text statusText;
    public Animator statusAnimator;

    public Text speedText;
    public Text pitchText;
    public float speakPicthStep = 0.25f;    //Text reading pitch step
    public float speakSpeedStep = 0.25f;    //Text reading speed step



    // Use this for initialization
    private void Start()
    {
        if (receiveObject == null)
            receiveObject = this.gameObject;

#if UNITY_EDITOR
        Debug.Log("InitSpeechRecognizer");
#elif UNITY_ANDROID
        AndroidPlugin.InitTextToSpeech(receiveObject.name, "OnStatus"); //Check the initialize status
#endif
    }

    // Update is called once per frame
    //private void Update () {

    //}



    //Reading text currently displayed (for button)
    public void PlayTextToSpeech()
    {
        if (displayText != null && !string.IsNullOrEmpty(displayText.text))
            StartTextToSpeech(displayText.text);
    }


    //Start Text To Speech
    public void StartTextToSpeech(string message)
    {
#if UNITY_EDITOR
        Debug.Log("StartTextToSpeech : message = " + message);
        if (!string.IsNullOrEmpty(message))
            StartCoroutine(DebugSimulate());
#elif UNITY_ANDROID
        AndroidPlugin.StartTextToSpeech(message, receiveObject.name, "OnStatus", "OnStart", "OnDone", "OnStop");
#endif
    }


    //Text To Speech status callback handler
    private void OnStatus(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnStatus");
#endif
        if (statusText != null)
            statusText.text = message;

        if (displayText != null)
        {
            if (message.StartsWith("SUCCESS_INIT"))
                displayText.text += "\nText To Speech is available.";
            else if (message.StartsWith("ERROR_LOCALE_NOT_AVAILABLE"))
                displayText.text += "\nFailed to initialize Text To Speech. It is a language that can not be used.";
            else if (message.StartsWith("ERROR_INIT"))
                displayText.text += "\nFailed to initialize Text To Speech.";
        }
    }

    //Callback handler when start reading text
    private void OnStart(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnStart");
#endif
        if (statusAnimator != null)
            statusAnimator.SetTrigger("blink");

        if (statusText != null)
        {
            //statusText.text = message;
            statusText.text = "Speaking";
        }
    }

    //Callback handler when finish reading text
    private void OnDone(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnDone");
#endif
        if (statusAnimator != null)
            statusAnimator.SetTrigger("stop");

        if (statusText != null)
        {
            //statusText.text = message;
            statusText.text = "Finished";
        }
    }

    //Callback handler when interrupted reading text
    private void OnStop(string message)
    {
#if UNITY_EDITOR
        Debug.Log("OnStop");
#endif
        if (statusAnimator != null)
            statusAnimator.SetTrigger("stop");

        if (statusText != null)
        {
            //statusText.text = message;
            statusText.text = "Stopped" + (message.StartsWith("INTERRUPTED") ? "(interrupted)" : "");
        }
    }


    //Interrupted reading text
    public void StopTextToSpeech()
    {
#if UNITY_EDITOR
        Debug.Log("StopTextToSpeech called");
#elif UNITY_ANDROID
        AndroidPlugin.StopTextToSpeech();
#endif
    }


#if UNITY_EDITOR
    //For debug (Editor only)
    private IEnumerator DebugSimulate()
    {
        OnStart("onStart");
        yield return new WaitForSeconds(3f);

        OnDone("onDone");
    }
#endif


    //Increase utterance speed of Text To Speech
    public void SpeakSpeedUp()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakSpeedUp called");
#elif UNITY_ANDROID
        SetSpeedText(AndroidPlugin.AddTextToSpeechSpeed(speakSpeedStep));
#endif
    }


    //Decrease utterance speed of Text To Speech
    public void SpeakSpeedDown()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakSpeedDown called");
#elif UNITY_ANDROID
        SetSpeedText(AndroidPlugin.AddTextToSpeechSpeed(-speakSpeedStep));
#endif
    }


    //Reset utterance speed of Text To Speech (1.0f)
    public void SpeakSpeedReset()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakSpeedReset called");
#elif UNITY_ANDROID
        SetSpeedText(AndroidPlugin.ResetTextToSpeechSpeed());
#endif
    }


    //Increase utterance pitch of Text To Speech
    public void SpeakPitchUp()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakPitchUp called");
#elif UNITY_ANDROID
        SetPitchText(AndroidPlugin.AddTextToSpeechPitch(speakPicthStep));
#endif
    }


    //Decrease utterance pitch of Text To Speech
    public void SpeakPitchDown()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakPitchDown called");
#elif UNITY_ANDROID
        SetPitchText(AndroidPlugin.AddTextToSpeechPitch(-speakPicthStep));
#endif
    }


    //Reset utterance pitch of Text To Speech (1.0f)
    public void SpeakPitchReset()
    {
#if UNITY_EDITOR
        Debug.Log("SpeakPitchReset called");
#elif UNITY_ANDROID
        SetPitchText(AndroidPlugin.ResetTextToSpeechPitch());
#endif
    }



    //Display utterance speed
    private void SetSpeedText(float speed)
    {
        if (speedText != null)
            speedText.text = string.Format("Speed : {0:F2}", speed);
    }

    //Display utterance pitch
    private void SetPitchText(float pitch)
    {
        if (pitchText != null)
            pitchText.text = string.Format("Pitch : {0:F2}", pitch);
    }



    //Call the text edit Dialog
    public void EditText()
    {
        if (displayText != null)
        {
#if UNITY_EDITOR
            Debug.Log("EditText called");
#elif UNITY_ANDROID
            AndroidPlugin.ShowMultiLineTextDialog("Edit text", displayText.text, 0, 9, receiveObject.name, "OnEditText");
#endif
        }
    }

    //Callback handler in text edit Dialog
    private void OnEditText(string message)
    {
        if (displayText != null)
            displayText.text = message.Trim();
    }

}
