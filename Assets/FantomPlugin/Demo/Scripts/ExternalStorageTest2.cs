using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Get External Storage folder information and Storage Access Framework and localize demo
//ストレージのフォルダ情報取得とストレージアクセス機能の利用とローカライズのデモ
public class ExternalStorageTest2 : MonoBehaviour {

    //UIs
    public Text displayText;

    public Dropdown textDropdown;
    public string[] textExt;
    public bool addTextAllType = true;

    public Dropdown fileDropdown;
    public Dropdown imageDropdown;
    public Dropdown videoDropdown;
    public Dropdown audioDropdown;

    public Button playAudioButton;
    public Button stopAudioButton;
    public AudioSource audioSource;

    //Mainly 'ToastController.Show' is called.
    public ToastController toastControl;

    //Mainly 'MediaScannerController.StartScan' is called.
    public MediaScannerController mediaScannerControl;

    //Mainly 'AndroidActionController.StartAction' is called (default action is "android.intent.action.VIEW").
    public AndroidActionController actionControl;

    //Storage Controllers
    public StorageLoadTextController storageLoadTextControl;
    public StorageSaveTextController storageSaveTextControl;
    public StorageOpenFileController storageOpenFileControl;
    public StorageOpenImageController storageOpenImageControl;
    public StorageOpenVideoController storageOpenVideoControl;
    public StorageOpenAudioController storageOpenAudioControl;
    public StorageSaveFileController storageSaveFileControl;


    //Load success message
    public LocalizeString loadSuccessMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Successfully loading text from a file."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "ファイルからテキストをロードしました。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "文本从文件中读取。"),
            new LocalizeString.Data(SystemLanguage.Korean, "텍스트 파일에서 가져 왔습니다."),
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



    //==========================================================

    // Use this for initialization
    private void Start() {
        MakeDropdown();
        SetEnableAudioButtons(false);
    }

    // Update is called once per frame
    //private void Update () {

    //}

    
    //==========================================================

    //MIME type list (Adjust the order with dropdown)
    List<string[]> fileMime;
    List<string[]> imageMime;
    List<string[]> videoMime;
    List<string[]> audioMime;
    List<string[]> textMime;

    //Initialize extension dropdown
    private void MakeDropdown()
    {
        CreateExtAndMimeListToDropdown(fileDropdown, ref fileMime, AndroidMimeType.MediaType.File, true);
        CreateExtAndMimeListToDropdown(imageDropdown, ref imageMime, AndroidMimeType.MediaType.Image, true);
        CreateExtAndMimeListToDropdown(videoDropdown, ref videoMime, AndroidMimeType.MediaType.Video, true);
        CreateExtAndMimeListToDropdown(audioDropdown, ref audioMime, AndroidMimeType.MediaType.Audio, true);
        CreateExtAndMimeListToDropdown(textDropdown, ref textMime, textExt, addTextAllType);    //for text files
    }

    //Set the extension to the dropdown and set MIME type to the list from MIME type constant values.
    //MIME type 定数から、ドロップダウンに拡張子をセットし、リストに MIME type をセットする。
    private void CreateExtAndMimeListToDropdown(Dropdown dropdown, ref List<string[]> mimeList, AndroidMimeType.MediaType mediaType, bool addAll = true)
    {
        if (dropdown == null)
            return;

        List<string> extList = new List<string>();
        mimeList = new List<string[]>();

        if (addAll)
        {
            switch (mediaType)
            {
                case AndroidMimeType.MediaType.File:
                    extList.Add("*");
                    mimeList.Add(new string[] { AndroidMimeType.File.All });
                    break;
                case AndroidMimeType.MediaType.Image:
                    extList.Add("*");
                    mimeList.Add(new string[] { AndroidMimeType.Image.All });
                    break;
                case AndroidMimeType.MediaType.Audio:
                    extList.Add("*");
                    mimeList.Add(new string[] { AndroidMimeType.Audio.All });
                    break;
                case AndroidMimeType.MediaType.Video:
                    extList.Add("*");
                    mimeList.Add(new string[] { AndroidMimeType.Video.All });
                    break;
                default:
                    break;
            }
        }

        List<string> ext; List<string[]> mime;
        AndroidMimeType.GetExtToMimeList(mediaType, out ext, out mime);
        extList.AddRange(ext);
        mimeList.AddRange(mime);

        dropdown.ClearOptions();
        dropdown.AddOptions(extList);
    }

    //Set the extension to the dropdown and set MIME type to the list from the specified extensions.
    //指定された拡張子から、ドロップダウンに拡張子をセットし、リストに MIME type をセットする。
    private void CreateExtAndMimeListToDropdown(Dropdown dropdown, ref List<string[]> mimeList, string[] extArray, bool addTextAll = true)
    {
        if (dropdown == null || extArray == null || extArray.Length == 0)
            return;

        List<string> extList = new List<string>();
        mimeList = new List<string[]>();

        if (addTextAll)
        {
            extList.Add("*");
            mimeList.Add(new string[] { AndroidMimeType.File.TextAll });
        }

        foreach (var ext in extArray)
        {
            extList.Add(ext);

            var mime = AndroidMimeType.GetMimeType(ext);
            if (mime != null)
                mimeList.Add(mime);
            else
                mimeList.Add(new string[] { AndroidMimeType.File.All });
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(extList);
    }


    //==========================================================

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


    //==========================================================
    //Simple text load/save

    //Required : <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

    //Save the text of the displayed information in a file.
    //(*) To write to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //
    //表示されている情報のテキストをファイルに保存する。
    //※Android で External Storage に書き込みをするには、「AndroidManifest.xml」にパーミッションが必要。
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

                    if (toastControl != null)
                        toastControl.Show(saveSuccessMessage.Text);

                    if (mediaScannerControl != null)
                        mediaScannerControl.StartScan(file);
                }
            }
        }
#endif
    }

    //Required : '<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />' (or 'WRITE_EXTERNAL_STORAGE') in 'AndroidManifest.xml'.

    //Load from a text file
    //(*) To read from External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //
    //テキストをファイルから読み込む
    //※Android で External Storage から読み込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    private string LoadText(string path)
    {
        StringBuilder sb = new StringBuilder(1024);

        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    sb.Append(line).Append("\n");
                }
            }
        }
        catch (Exception e)
        {
            XDebug.Log(e.Message);
            return null;
        }

        return sb.ToString();
    }

    //Required : '<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'.

    //Save to a text file
    //(*) Note the file may not be visible until MediaScanner runs.
    //(*) To write to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //(*) For security reasons, it seems that it can not be saved directly from SD card to Unity.
    //
    //テキストをファイルに保存
    //※MediaScanner が走るまではファイルが見えないことがあるので注意。
    //※Android で External Storage に書き込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    //※セキュリティ上、Unity から直接 SD Card には保存できないようです。
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


    //==========================================================
    //Simple CSV load/save

    List<string[]> csvData;     //Loaded data buffer

    //Required : <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" /> (or 'WRITE_EXTERNAL_STORAGE')

    //Load specified file as CSV data.
    //(*) To read to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //
    //指定ファイルをCSVデータとして読み込む。
    //※Android で External Storage に読み込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    public void LoadCsvData(string path)
    {
        XDebug.Log("LoadCsvData : path = " + path);
        if (string.IsNullOrEmpty(path))
            return;

        if (csvData == null)
            csvData = new List<string[]>();
        else
            csvData.Clear();

        csvData = LoadSimpleCsv(path);

        if (csvData != null)
        {
            DisplayCsvData();

            if (toastControl != null)
                toastControl.Show(loadSuccessMessage.Text);
        }
    }

    //Required : <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

    //Save the data list buffer (csvData) as CSV data.
    //(*) Note the file may not be visible until MediaScanner runs.
    //(*) To write to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //(*) For security reasons, it can not be saved directly from the Unity to the SD card.
    //    To save the text file on the SD card, use 'StorageSaveTextController'.
    //
    //データリストのバッファ（csvData）をCSVデータとして保存する。
    //※MediaScanner が走るまではファイルが見えないことがあるので注意。
    //※Android で External Storage に書き込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    //※セキュリティ上、Unity から直接 SD Card には保存できません。
    //　SDカードにテキストファイルを保存するには StorageSaveTextController を使って下さい。
    public void SaveCsvData(string path)
    {
        XDebug.Log("SaveCsvData : path = " + path);
        if (string.IsNullOrEmpty(path))
            return;

        if (csvData == null || csvData.Count == 0)
        {
            XDebug.Log("Csv Data is empty. Please load csv file first.");
            return;
        }

        DisplayCsvData();

        if (SaveSimpleCsv(csvData, path))
        {
            if (toastControl != null)
                toastControl.Show(saveSuccessMessage.Text);

            if (mediaScannerControl != null)
                mediaScannerControl.StartScan(path);
        }
    }

    //Display csv data buffer (csvData)
    public void DisplayCsvData()
    {
        foreach (var item in csvData)
            XDebug.Log(string.Join(", ", item));
    }


    //Required : '<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />' (or 'WRITE_EXTERNAL_STORAGE') in 'AndroidManifest.xml'.

    //Read the text file, split each line by comma, and return it as a list.
    //(*) Note that it does not correspond to the nesting of commas, double quotes, etc (It is not a correct CSV file format).
    //(*) To read from External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //
    //テキストファイルを読み込み、各行をカンマ区切りで分割してリストで返す。
    //※カンマの入れ子、ダブルクォートなどには対応してないので注意（正しいCSVファイル形式ではありません）。
    //※Android で External Storage から読み込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    //
    //(CSV format)
    //https://tools.ietf.org/html/rfc4180
    //http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm
    private List<string[]> LoadSimpleCsv(string path)
    {
        List<string[]> lines = new List<string[]>();

        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var fields = line.Split(',').Select(e => e.Trim()).ToArray();
                        lines.Add(fields);
                    }
                }
            }
        }
        catch (Exception e)
        {
            XDebug.Log(e.Message);
            return null;
        }

        return lines;
    }

    //Required : '<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'.

    //Combine the buffer (csvData) of the data list with a comma and save it as a simple CSV file.
    //(*) Note that it does not correspond to the nesting of commas, double quotes, etc (It is not a correct CSV file format).
    //(*) To write to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //(*) For security reasons, it seems that it can not be saved directly from SD card to Unity.
    //
    //データリストのバッファ（csvData）をカンマで結合して、簡易CSVファイルとして保存する。
    //※カンマの入れ子、ダブルクォートなどには対応してないので注意（正しいCSVファイル形式ではありません）。
    //※Android で External Storage に書き込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    //※セキュリティ上、Unity から直接 SD Card には保存できないようです。
    //
    //(CSV format)
    //https://tools.ietf.org/html/rfc4180
    //http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm
    private static bool SaveSimpleCsv(List<string[]> lines, string path)
    {
        StringBuilder sb = new StringBuilder(1024);

        foreach (var item in lines)
        {
            string line = string.Join(", ", item);
            sb.Append(line).Append("\n");
        }

        return SaveText(sb.ToString(), path);
    }


    //==========================================================
    //Json load/save

    [Serializable]
    public class JsonData
    {
        public string name;
        public int age;
        public float time;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    JsonData jsonData;  //Loaded parameter buffer

    //Required : <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" /> (or 'WRITE_EXTERNAL_STORAGE')

    //Load specified file as JSON data.
    //(*) To read to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //
    //指定ファイルをJSONデータとして読み込む。
    //※Android で External Storage に読み込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    public void LoadJsonData(string path)
    {
        XDebug.Log("LoadJsonData : path = " + path);
        if (string.IsNullOrEmpty(path))
            return;

        string text = LoadText(path);
        if (!string.IsNullOrEmpty(text))
        {
            jsonData = JsonUtility.FromJson<JsonData>(text);

            XDebug.Log(jsonData);

            if (toastControl != null)
                toastControl.Show(loadSuccessMessage.Text);
        }
    }

    //Required : <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

    //Save the data parameter buffer (jsonData) as JSON data.
    //(*) Note the file may not be visible until MediaScanner runs.
    //(*) To write to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //(*) For security reasons, it can not be saved directly from the Unity to the SD card.
    //    To save the text file on the SD card, use 'StorageSaveTextController'.
    //
    //データパラメタのバッファ（jsonData）をJSONデータとして保存する。
    //※MediaScanner が走るまではファイルが見えないことがあるので注意。
    //※Android で External Storage に書き込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    //※セキュリティ上、Unity から直接 SD Card には保存できません。
    //　SDカードにテキストファイルを保存するには StorageSaveTextController を使って下さい。
    public void SaveJsonData(string path)
    {
        XDebug.Log("SaveJsonData : path = " + path);
        if (string.IsNullOrEmpty(path))
            return;

        if (jsonData == null)
        {
            XDebug.Log("Json Data is empty. Please load json file first.");
            return;
        }

        XDebug.Log(jsonData);

        if (SaveText(jsonData.ToString(), path))
        {
            if (toastControl != null)
                toastControl.Show(saveSuccessMessage.Text);

            if (mediaScannerControl != null)
                mediaScannerControl.StartScan(path);
        }
    }


    //==========================================================
    //Callback handlers

    string extSeparator = "; ";

    //Register 'Dropdown.OnValueChanged' callback of text extension
    public void OnTextMimeTypeChanged(int index)
    {
        if (textMime != null)
        {
            string[] mimeTypes = textMime[index];
            if (storageLoadTextControl != null)
                storageLoadTextControl.MimeTypes = mimeTypes;
            if (storageSaveTextControl != null)
                storageSaveTextControl.MimeTypes = mimeTypes;
            XDebug.Log("Change MIME Type for text : " + string.Join(extSeparator, mimeTypes));
        }
    }

    //Register 'Dropdown.OnValueChanged' callback of file extension
    public void OnFileMimeTypeChanged(int index)
    {
        if (fileMime != null)
        {
            string[] mimeTypes = fileMime[index];
            if (storageOpenFileControl != null)
                storageOpenFileControl.MimeTypes = mimeTypes;
            if (storageSaveFileControl != null)
                storageSaveFileControl.MimeTypes = mimeTypes;

            XDebug.Log("Change MIME Type for file : " + string.Join(extSeparator, mimeTypes));
        }
    }

    //Register 'Dropdown.OnValueChanged' callback of image extension
    public void OnImageMimeTypeChanged(int index)
    {
        if (imageMime != null)
        {
            string[] mimeTypes = imageMime[index];
            if (storageOpenImageControl != null)
                storageOpenImageControl.MimeTypes = mimeTypes;

            XDebug.Log("Change MIME Type for image : " + string.Join(extSeparator, mimeTypes));
        }
    }

    //Register 'Dropdown.OnValueChanged' callback of video extension
    public void OnVideoMimeTypeChanged(int index)
    {
        if (videoMime != null)
        {
            string[] mimeTypes = videoMime[index];
            if (storageOpenVideoControl != null)
                storageOpenVideoControl.MimeTypes = mimeTypes;

            XDebug.Log("Change MIME Type for video : " + string.Join(extSeparator, mimeTypes));
        }
    }

    //Register 'Dropdown.OnValueChanged' callback of audio extension
    public void OnAudioMimeTypeChanged(int index)
    {
        if (audioMime != null)
        {
            string[] mimeTypes = audioMime[index];
            if (storageOpenAudioControl != null)
                storageOpenAudioControl.MimeTypes = mimeTypes;

            XDebug.Log("Change MIME Type for audio : " + string.Join(extSeparator, mimeTypes));
        }
    }

    //Register 'StorageOpenFileController.OnResultInfo' callback 
    public void OnOpenFile(ContentInfo info)
    {
        XDebug.Log("OnOpenFile : " + info);
        XDebug.Log("path = " + info.path);

        if (!string.IsNullOrEmpty(info.fileUri))
        {
            string ext = Path.GetExtension(info.path);
            XDebug.Log("ext = " + ext);

            switch (ext)
            {
                case ".pdf":
                case ".xls":
                case ".xlsx":
                case ".doc":
                case ".docx":
                case ".ppt":        //Not available?
                case ".pptx":       //ok
                    if (actionControl != null)
                    {
                        //It can be opened if there is a dedicated viewer (e.g. Adobe Acrobat Reader, etc.).
                        //The SD card may fail to open (depend on application correspondence).
                        //When there is no corresponding application, do nothing or display error (depend on installed application).
                        //
                        //専用のビューワがあれば開ける（例：Adobe Acrobat Reader 等）。
                        //SDカードではオープン失敗することがある（アプリの対応による）。
                        //対応するアプリが無い場合、何も起こらないかエラーメッセージ出る（アプリによる）。
                        actionControl.actionType = AndroidActionController.ActionType.URI;
                        actionControl.StartActionURI(info.fileUri);
                        XDebug.Log("StartActionURI : uri = " + info.fileUri);
                        //actionControl.actionType = AndroidActionController.ActionType.UriWithMimeType;
                        //actionControl.StartActionUriWithMimeType(info.fileUri, info.mimeType);
                        //XDebug.Log("StartActionUriWithMimeType : uri = " + info.fileUri + ", mimeType = " + info.mimeType);
                    }
                    break;
                case ".csv":
                    LoadCsvData(info.path);
                    break;
                case ".json":
                    LoadJsonData(info.path);
                    break;
                case ".txt":
                    if (displayText != null)
                    {
                        if (!string.IsNullOrEmpty(info.path))
                            displayText.text = LoadText(info.path);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (info.mimeType)
            {
                case AndroidMimeType.File.pdf:
                    if (actionControl != null)
                    {
                        //It can be opened if there is a dedicated viewer (e.g. [Google] Drive PDF Viewer, etc.).
                        //When there is no corresponding application, do nothing or display error (depend on installed application).
                        //
                        //専用のビューワがあれば開ける場合もある（例：[Google] Drive PDF Viewer 等）。
                        //対応するアプリが無い場合、何も起こらないかエラーメッセージ出る（アプリによる）。
                        actionControl.actionType = AndroidActionController.ActionType.UriWithMimeType;
                        actionControl.StartActionUriWithMimeType(info.uri, info.mimeType);
                        XDebug.Log("StartActionUriWithMimeType : uri = " + info.uri + ", mimeType = " + info.mimeType);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    //Register 'StorageSaveFileController.OnResultInfo' callback 
    public void OnSaveFile(ContentInfo info)
    {
        XDebug.Log("OnSaveFile : " + info);
        XDebug.Log("path = " + info.path);

        if (!string.IsNullOrEmpty(info.path))
        {
            string ext = Path.GetExtension(info.path);
            XDebug.Log("ext = " + ext);

            switch (ext)
            {
                case ".csv":
                    SaveCsvData(info.path);
                    break;
                case ".json":
                    SaveJsonData(info.path);
                    break;
                case ".txt":
                    if (displayText != null && !string.IsNullOrEmpty(displayText.text))
                    {
                        if (SaveText(displayText.text, info.path))
                            XDebug.Log("Save to : " + info.path);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            XDebug.Log("'path' is empty.");
        }
    }


    //==========================================================
    //Load audio and play

    public void LoadAudio(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            XDebug.Log("Path is empty. Please download the audio file.");
            return;
        }

        StartCoroutine(LoadToAudioClipAndPlay(path));
    }

    public void LoadAudio(AudioInfo info)
    {
        if (string.IsNullOrEmpty(info.path))
        {
            XDebug.Log("Path is empty. Please download the audio file.");
            return;
        }

        XDebug.Log("LoadAudio : title = " + info.title + " / artist = " + info.artist);
        StartCoroutine(LoadToAudioClipAndPlay(info.path));
    }

    IEnumerator LoadToAudioClipAndPlay(string path)
    {
        if (audioSource == null)
            yield break;

        StopAudio();

        using(WWW www = new WWW("file://" + path))
        {
            while (!www.isDone)
                yield return null;

            AudioClip audioClip = www.GetAudioClip(false, true);
            if (audioClip.loadState != AudioDataLoadState.Loaded)
            {
                XDebug.Log("Failed to load AudioClip.");
                SetEnableAudioButtons(false);
                yield break;
            }

            audioSource.clip = audioClip;
            SetEnableAudioButtons(true);
            PlayAudio();
        }
    }

    public void PlayAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    //Audio operation enable on/off
    public void SetEnableAudioButtons(bool enable)
    {
        if (playAudioButton != null)
            playAudioButton.interactable = enable;
        if (stopAudioButton != null)
            stopAudioButton.interactable = enable;
    }

}
