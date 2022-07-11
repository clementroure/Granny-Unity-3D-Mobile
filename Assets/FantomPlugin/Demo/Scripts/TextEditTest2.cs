using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Demonstration of simple text editing and file reading, writing and localize.
//簡易的なテキスト編集とファイルの読み書きとローカライズのデモ。
public class TextEditTest2 : MonoBehaviour {

    public string filePrefix = "text";          //Prefix of filename to be saved.
    public bool appendDateTimeString = true;    //Add a DateTime string to the file name.

    public Text displayText;

    public SystemLanguage localizeLanguage = SystemLanguage.Unknown;    //current localize language

    //Load success message
    public LocalizeString loadSuccessMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Successfully loading text from a file."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "ファイルからテキストをロードしました。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "文本从文件中读取。"),
            new LocalizeString.Data(SystemLanguage.Korean, "파일에서 텍스트를로드했습니다."),
        });

    //Save success message
    public LocalizeString saveSuccessMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Successfully saving text to a file."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "テキストをファイルに保存しました。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "文本保存到文件。"),
            new LocalizeString.Data(SystemLanguage.Korean, "텍스트 파일에 저장했습니다."),
        });

    //Mainly 'ToastController.Show' is called.
    public ToastController toastControl;

    //Mainly 'MultiLineTextDialogController.Show' is called.
    public MultiLineTextDialogController multiLineTextDialogControl;

    //Mainly 'StorageLoadTextController.Show' is called.
    public StorageLoadTextController storageLoadTextControl;

    //Mainly 'StorageSaveTextController.Show, .CurrentValue' is called.
    public StorageSaveTextController storageSaveTextControl;

    //When dynamically generating file name to save.
    //保存するファイル名を動的に生成するとき
    public bool autoSaveFileName = true;


    // Use this for initialization
    private void Start () {
        
    }

    // Update is called once per frame
    //private void Update () {

    //}



    //Display text string (and for reading)
    public void DisplayText(object message, bool add = false)
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
                displayText.text = message.ToString();
        }
    }


    //Call the text(reading) edit dialog
    public void EditText()
    {
        if (multiLineTextDialogControl != null && !string.IsNullOrEmpty(displayText.text))
            multiLineTextDialogControl.Show(displayText.text);
    }

    //Callback handler for text edit dialog result
    public void OnEditText(string text)
    {
        DisplayText(text);
    }


    //Open system storage application to load text file.
    public void LoadText()
    {
        if (storageLoadTextControl != null)
            storageLoadTextControl.Show();
    }

    //Callback handler when file load succeeded.
    public void ReceiveResultLoadText(string text)  //loaded text
    {
        DisplayText(text);

        if (toastControl != null)
            toastControl.Show(loadSuccessMessage.ToString());
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
        if (storageSaveTextControl != null && displayText != null && !string.IsNullOrEmpty(displayText.text))
        {
            if (autoSaveFileName)
            {
                string file = filePrefix;
                if (appendDateTimeString)
                    file += "_" + DateTime.Now.ToString("yyMMdd_HHmmss");
                if (string.IsNullOrEmpty(file))
                    file = "NewDocumet";
                if (!file.EndsWith(".txt"))
                    file += ".txt";

                if (storageSaveTextControl.syncExtension)
                {
                    var ext = AndroidMimeType.GetExtension(storageSaveTextControl.MimeType);
                    if (ext != null)
                        file = Path.ChangeExtension(file, ext[0]);
                }
                storageSaveTextControl.CurrentValue = file;
            }

            storageSaveTextControl.Show(displayText.text);
        }
    }

    //Callback handler when file save succeeded.
    public void ReceiveResultSaveText(string fileName)  //saved filename (not include directory path)
    {
        XDebug.Log("FileName = " + fileName);
            
        if (toastControl != null)
            toastControl.Show(saveSuccessMessage.ToString());
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


    //Callback handler for 'LocalizeLanguageChanger'
    public void OnLanguageChanged(SystemLanguage language)
    {
        XDebug.Log("Localize language changed : " + language);
        localizeLanguage = language;
    }
}
