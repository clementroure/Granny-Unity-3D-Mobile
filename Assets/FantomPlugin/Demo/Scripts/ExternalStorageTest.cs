using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Get External Storage folder information and Storage Access Framework demo
public class ExternalStorageTest : MonoBehaviour {

    public Text displayText;

    public string saveSuccessMessage = "Successfully saving text to a file.";


    //Register 'ToastController.Show' in the inspector.
    [Serializable] public class ToastHandler : UnityEvent<string> { }   //message
    public ToastHandler OnToast;

    //Register 'MediaScannerController.StartScan' in the inspector.
    [Serializable] public class MediaScanHandler : UnityEvent<string> { }   //path
    public MediaScanHandler OnMediaScan;



    // Use this for initialization
    private void Start () {

    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}


    //Get the storage information
    //ストレージの情報を取得する
    public void GetInfo()
    {
        XDebug.Clear();
        //XDebug.Log("Application.dataPath = " + Application.dataPath);                         //ゲームデータのフォルダーパスを返します（読み取り専用）
        XDebug.Log("Application.persistentDataPath = " + Application.persistentDataPath);       //永続的なデータディレクトリのパスを返します（読み取り専用）
        //XDebug.Log("Application.streamingAssetsPath = " + Application.streamingAssetsPath);   //StreamingAssets フォルダーへのパスが含まれています（読み取り専用）
        //XDebug.Log("Application.temporaryCachePath = " + Application.temporaryCachePath);     //一時的なデータ、キャッシュのディレクトリパスを返します（読み取り専用）値はデータを保存できる一時的なディレクトリパスです。

#if !UNITY_EDITOR && UNITY_ANDROID
        XDebug.Log("----------------------------");
        XDebug.Log("IsExternalStorageEmulated = " + AndroidPlugin.IsExternalStorageEmulated());
        XDebug.Log("IsExternalStorageRemovable = " + AndroidPlugin.IsExternalStorageRemovable());
        XDebug.Log("IsExternalStorageMounted = " + AndroidPlugin.IsExternalStorageMounted());
        XDebug.Log("IsExternalStorageMountedReadOnly = " + AndroidPlugin.IsExternalStorageMountedReadOnly());
        XDebug.Log("IsExternalStorageMountedReadWrite = " + AndroidPlugin.IsExternalStorageMountedReadWrite());
        XDebug.Log("GetExternalStorageState = " + AndroidPlugin.GetExternalStorageState());
        XDebug.Log("GetExternalStorageDirectory = " + AndroidPlugin.GetExternalStorageDirectory());
        XDebug.Log("GetExternalStorageDirectoryAlarms = " + AndroidPlugin.GetExternalStorageDirectoryAlarms());
        XDebug.Log("GetExternalStorageDirectoryDCIM = " + AndroidPlugin.GetExternalStorageDirectoryDCIM());
        XDebug.Log("GetExternalStorageDirectoryDocuments = " + AndroidPlugin.GetExternalStorageDirectoryDocuments());
        XDebug.Log("GetExternalStorageDirectoryDownloads = " + AndroidPlugin.GetExternalStorageDirectoryDownloads());
        XDebug.Log("GetExternalStorageDirectoryMovies = " + AndroidPlugin.GetExternalStorageDirectoryMovies());
        XDebug.Log("GetExternalStorageDirectoryMusic = " + AndroidPlugin.GetExternalStorageDirectoryMusic());
        XDebug.Log("GetExternalStorageDirectoryNotifications = " + AndroidPlugin.GetExternalStorageDirectoryNotifications());
        XDebug.Log("GetExternalStorageDirectoryPictures = " + AndroidPlugin.GetExternalStorageDirectoryPictures());
        XDebug.Log("GetExternalStorageDirectoryPodcasts = " + AndroidPlugin.GetExternalStorageDirectoryPodcasts());
        XDebug.Log("GetExternalStorageDirectoryRingtones = " + AndroidPlugin.GetExternalStorageDirectoryRingtones());
#endif
    }


    //Callback handler when MediaScanner complete
    //MediaScanner 完了コールバック
    public void ReceiveMediaScanner(string path)
    {
        XDebug.Log("ReceiveMediaScanner complete : " + path);
    }


    //Save the text of the displayed information in a file.
    //(*)To write to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //
    //表示されている情報のテキストをファイルに保存する。
    //※Android で External Storage に書き込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    //Required : <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    public void SaveInfoText()
    {
#if UNITY_EDITOR
        Debug.Log("SaveInfoText called");
#elif UNITY_ANDROID
        if (AndroidPlugin.IsExternalStorageMountedReadWrite())
        {
            if (displayText != null && !string.IsNullOrEmpty(displayText.text))
            {
                string path = AndroidPlugin.GetExternalStorageDirectoryDocuments();   //API 19 or higher
                if (string.IsNullOrEmpty(path))
                    path = AndroidPlugin.GetExternalStorageDirectory();

                string file = path + "/info_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".txt";
                if (SaveText(displayText.text, file))
                {
                    XDebug.Log("Save to : " + file);

                    if (OnToast != null)
                        OnToast.Invoke(saveSuccessMessage);

                    if (OnMediaScan != null)
                        OnMediaScan.Invoke(file);
                }
            }
        }
#endif
    }


    //Required : '<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'.
    //Save to a text file
    //(*) The file may not be visible until MediaScanner runs.
    //
    //テキストファイルに保存
    //※MediaScanner が走るまではファイルが見えないことがある
    private static bool SaveText(string text, string path)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(text);
                writer.Flush();
                writer.Close();
            }
        }
        catch (Exception e)
        {
            XDebug.Log(e.Message);  //Access to the path "filename" is denied. -> required : '<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'.
            return false;
        }
        return true;
    }

}
