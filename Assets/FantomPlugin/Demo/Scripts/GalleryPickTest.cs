using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Demonstration of get image information from the gallery.
//(*) Please download whole sphere mesh 'Sphere100.fbx' from the URL.
//http://warapuri.com/post/131599525953/
//･When saving a screenshot to External Storage, the following permission is required for 'AndroidManifest.xml'.
//(*)'<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'
//
//ギャラリーからの画像情報取得のデモ
//※全天球のメッシュ「Sphere100.fbx」は以下からダウンロードして下さい。
//http://warapuri.com/post/131599525953/
//・スクリーンショットをストレージに保存する場合は、以下のパーミッションが「AndroidManifest.xml」に必要になります。
//※'<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'
public class GalleryPickTest : MonoBehaviour {

    public Material textureMat;         //Material applying texture.                                //テクスチャを適用するマテリアル
    public Image image;                 //Image to apply texture.                                   //テクスチャを適用する画像
    public GameObject cube;             //Cube object to apply texture.                             //テクスチャを適用するキューブオブジェクト
    public GameObject sphere;           //Whole sphere (for 360 degrees image) to apply texture.    //テクスチャを適用する全天球オブジェクト
    public GameObject chara;            //Character Model or other (Texture does not apply)         //表示するキャラクターなど（テクスチャは適用しない）

    public int defaultWidth = 512;      //Alternate value when width get failed.                    //幅の取得に失敗したときの代替値
    public int defaultHeight = 512;     //Alternate value when height get failed.                   //高さの取得に失敗したときの代替値

    public GameObject[] hideUIOnScreenshot;    //UI to hide in screenshot.


    //Register 'ToastController.Show' in the inspector.
    [Serializable] public class ToastHandler : UnityEvent<string> { }   //message
    public ToastHandler OnToast;

    //Register 'MediaScannerController.StartScan' in the inspector.
    [Serializable] public class MediaScanHandler : UnityEvent<string> { }   //path
    public MediaScanHandler OnMediaScan;


    // Use this for initialization
    private void Start () {
        if (cube != null)
            cube.SetActive(false);
        if (sphere != null)
            sphere.SetActive(false);
        if (chara != null)
            chara.SetActive(false);
#if !UNITY_EDITOR && UNITY_ANDROID
        XDebug.Log("'WRITE_EXTERNAL_STORAGE' permission = " + AndroidPlugin.CheckPermission("android.permission.WRITE_EXTERNAL_STORAGE"));
#endif
    }

    // Update is called once per frame
    //private void Update () {

    //}


    //==========================================================
    //UI

    //Callback handeler when switch UI image.      //UI の Image
    public void OnImageModeClick(bool isOn)
    {
        if (image != null)
            image.gameObject.SetActive(isOn);
    }

    //Callback handeler when switch cube object.   //Cube
    public void OnCubeModeClick(bool isOn)
    {
        if (cube != null)
            cube.SetActive(isOn);
    }

    //Callback handeler when switch whole sphere (360 degrees).    //全天球（360度）
    public void OnSphereModeClick(bool isOn)
    {
        if (sphere != null)
            sphere.SetActive(isOn);
    }

    //Callback handeler when display character etc.    //キャラクターなどの表示
    public void OnCharaClick(bool isOn)
    {
        if (chara != null)
            chara.SetActive(isOn);
    }


    //==========================================================
    //Gallery pick and load image

    //Callback handler when image information can be get from the gallery.  //ギャラリーから画像情報を取得できたときのコールバックハンドラ
    public void OnGalleryPick(string path, int width, int height)
    {
        XDebug.Log("OnGalleryPick: path = " + path + ", width = " + width + ", height = " + height);

        width = width > 0 ? width : defaultWidth;       //Alternate value when width get failed.    //幅の取得に失敗したときの代替値
        height = height > 0 ? height : defaultHeight;   //Alternate value when height get failed.   //高さの取得に失敗したときの代替値
        LoadAndSetImage(path, width, height);
    }

    //Image loading and setting.    //画像の読み込みとセット
    private void LoadAndSetImage(string path, int width, int height)
    {
        Texture2D texture = LoadToTexture2D(path, width, height, TextureFormat.ARGB32, false, FilterMode.Bilinear);
        if (texture != null)
        {
            RectTransform rt = image.rectTransform;
            int h = (int)rt.sizeDelta.y;
            int w = width * h / height;
            rt.sizeDelta = new Vector2(w, h);   //Make the same ratio as the image with the height as the reference.  //縦を基準として画像と同じ比率にする

            try
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;
                textureMat.mainTexture = texture;
            }
            catch (Exception)
            {
                XDebug.Log("Sprite.Create failed.");
            }
        }
        else
        {
            XDebug.Log("CreateTexture2D failed.");
#if !UNITY_EDITOR && UNITY_ANDROID
            XDebug.Log("'READ_EXTERNAL_STORAGE' permission = " + AndroidPlugin.CheckPermission("android.permission.READ_EXTERNAL_STORAGE"));
#endif
        }
    }

    //Load the image from the specified path and generates a Texture2D. //指定パスから画像を読み込み、テクスチャを生成する。
    private static Texture2D LoadToTexture2D(string path, int width, int height, TextureFormat format, bool mipmap, FilterMode filter)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(width, height, format, mipmap);
            texture.LoadImage(bytes);
            texture.filterMode = filter;
            texture.Compress(false);
            return texture;
        }
        catch (Exception e)
        {
            XDebug.Log(e.ToString());
            return null;
        }
    }

    //Callback handler when error or cancel.    //エラーやキャンセルのときのコールバックハンドラ
    public void OnError(string message)
    {
        XDebug.Log("GalleryPickTest.OnError : " + message);
    }


    //==========================================================
    //Screenshot

    string filePrefix = "screenshot_";  //Prefix of filename to be saved.
    bool isSaving = false;              //Ignore while saving.

    //Run screenshot
    public void ScreenShot()
    {
        if (isSaving)
            return;     //Ignore while saving.

        string fileName = filePrefix + DateTime.Now.ToString("yyMMdd_HHmmss") + ".png";
        string dir = Application.persistentDataPath;
#if UNITY_EDITOR
        dir = Application.dataPath + "/..";
#elif UNITY_ANDROID
        if (!AndroidPlugin.CheckPermission("android.permission.WRITE_EXTERNAL_STORAGE"))
        {
            XDebug.Log("'WRITE_EXTERNAL_STORAGE' permission denied.");
            return;
        }

        dir = AndroidPlugin.GetExternalStorageDirectoryPictures();
        if (string.IsNullOrEmpty(dir))
            dir = AndroidPlugin.GetExternalStorageDirectory();
#endif

        string path = dir + "/" + fileName;
        StartCoroutine(StartScreenshot(path));
    }

    //Hide the UI and execute the screenshot. If save the screenshot successfully, run MeidaScanner.
    private IEnumerator StartScreenshot(string path)
    {
        isSaving = true;

        foreach (var item in hideUIOnScreenshot)
            item.SetActive(false);
        yield return null;

        yield return StartCoroutine(SaveScreenshotPng(path));

        foreach (var item in hideUIOnScreenshot)
            item.SetActive(true);

        if (isSaving)
        {
            if (OnToast != null)
                OnToast.Invoke("Save ScreenShot completed");

            if (OnMediaScan != null)
                OnMediaScan.Invoke(path);

            XDebug.Log("Save to : " + path);
        }

        yield return null;
        isSaving = false;
    }

    //Callback handler when MediaScanner scan completed.
    public void ReceiveMediaScan(string message)
    {
        XDebug.Log("ReceiveMediaScan : " + message);
    }


    //(*)To write to External Storage on Android, you need permission in the 'AndroidManifest.xml' file.
    //※Android で External Storage に書き込みをするには、「AndroidManifest.xml」にパーミッションが必要。
    //Required: '<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'
    //
    //･The script referred to the following.
    // https://docs.unity3d.com/jp/540/ScriptReference/Texture2D.EncodeToPNG.html
    private IEnumerator SaveScreenshotPng(string path)
    {
	    // We should only read the screen buffer after rendering is complete
	    yield return new WaitForEndOfFrame();

	    // Create a texture the size of the screen, RGB24 format
	    int width = Screen.width;
	    int height = Screen.height;
	    Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

	    // Read screen contents into the texture
	    tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
	    tex.Apply();

	    // Encode texture into PNG
	    byte[] bytes = tex.EncodeToPNG();
        DestroyImmediate(tex);

        //For testing purposes, also write to a file in the project folder
        try
        {
            File.WriteAllBytes(path, bytes);
        }
        catch (Exception e)
        {
            isSaving = false;
            XDebug.Log(e.ToString());
        }

        yield return new WaitForEndOfFrame();
    }


#pragma warning disable 0219
    //For debug

    //Load the specified path by pressing the button.   //ボタン押下で指定パスを読み込む。
    public void TestLoadImage()
    {
        string path = "";
        string file = "/_Test/Images/CheckerTile.png";
#if UNITY_EDITOR
        path = Application.dataPath + file;
#elif UNITY_ANDROID
        path = AndroidPlugin.GetExternalStorageDirectoryPictures() + file;
#endif
        XDebug.Log("TestLoadImage : path = " + path);
        if (!string.IsNullOrEmpty(path))
            LoadAndSetImage(path, 256, 256);
    }

}
