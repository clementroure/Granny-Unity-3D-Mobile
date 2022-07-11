using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Demonstration of simple text editing and file reading and writing.
//簡易的なテキスト編集とファイルの読み書きのデモ。
public class TextEditTest : MonoBehaviour {

    public string filePrefix = "text";          //Prefix of filename to be saved.
    public bool appendDateTimeString = true;    //Add a DateTime string to the file name.

    public Text displayText;

    //Load/Save success message
    public string loadSuccessMessage = "Successfully loading text from a file.";
    public string saveSuccessMessage = "Successfully saving text to a file.";


    //Register 'ToastController.Show' in the inspector.
    [Serializable] public class ToastHandler : UnityEvent<string> { }
    public ToastHandler OnToast;

    //Register 'MultiLineTextDialogController.Show' in the inspector.
    [Serializable] public class TextEditHandler : UnityEvent<string> { }
    public TextEditHandler OnTextEdit;

    //Register 'StorageLoadTextController.Show' in the inspector.
    public UnityEvent OnOpenAndLoadText;

    //Register 'StorageSaveTextController.Show' in the inspector.
    [Serializable] public class OpenAndSaveTextHandler : UnityEvent<string> { }   //text
    public OpenAndSaveTextHandler OnOpenAndSaveText;

    //When dynamically generating file name to save.
    //保存するファイル名を動的に生成するとき
    //Register 'StorageSaveTextController.CurrentValue' in the inspector.
    [Serializable] public class SetSaveFileNameHandler : UnityEvent<string> { }   //fileName
    public SetSaveFileNameHandler OnSetSaveFileName;



    // Use this for initialization
    private void Start () {
        
    }

    // Update is called once per frame
    //private void Update () {

    //}



    //Display text string (and for reading)
    public void DisplayText(string message, bool add = false)
    {
        if (displayText != null)
        {
            if (add)
            {
                string text = displayText.text;
                if (text[text.Length - 1] != '\n')
                    displayText.text += "\n";
                displayText.text += message + "\n";
            }
            else
                displayText.text = message;
        }
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
        DisplayText(text);
    }


    //Open system storage application to load text file.
    public void LoadText()
    {
        if (OnOpenAndLoadText != null)
            OnOpenAndLoadText.Invoke();
    }

    //Callback handler when file load succeeded.
    public void ReceiveResultLoadText(string text)  //loaded text
    {
        DisplayText(text);

        if (OnToast != null)
            OnToast.Invoke(loadSuccessMessage);
    }

    //Callback handler when an error occurs in file load.
    public void ReceiveErrorLoadText(string message)
    {
        DisplayText("ReceiveErrorLoadText : " + message, true);
    }


    //Dynamically generate file name to be saved and open system storage application.
    //保存するファイル名を動的に生成し、システムストレージアプリを開く。
    public void SaveText()
    {
        if (OnOpenAndSaveText != null && displayText != null && !string.IsNullOrEmpty(displayText.text))
        {
            if (OnSetSaveFileName != null)
            {
                string file = filePrefix;
                if (appendDateTimeString)
                    file += "_" + DateTime.Now.ToString("yyMMdd_HHmmss");
                if (string.IsNullOrEmpty(file))
                    file = "NewDocumet";
                if (!file.EndsWith(".txt"))
                    file += ".txt";
                OnSetSaveFileName.Invoke(file);
            }

            OnOpenAndSaveText.Invoke(displayText.text);
        }
    }

    //Callback handler when file save succeeded.
    public void ReceiveResultSaveText(string fileName)  //saved filename (not include directory path)
    {
        XDebug.Log("FileName = " + fileName);
            
        if (OnToast != null)
            OnToast.Invoke(saveSuccessMessage);
    }

    //Callback handler when an error occurs in file save.
    public void ReceiveErrorSaveText(string message)
    {
        DisplayText("ReceiveErrorSaveText : " + message, true);
    }


    //Callback handler from QRCodeScannerController.OnResult
    public void ReceiveQRCodeScanner(string text)
    {
        if (!string.IsNullOrEmpty(text))
            DisplayText(text);
        else
            DisplayText("(QR Code Scan was canceled)", true);
    }

}
