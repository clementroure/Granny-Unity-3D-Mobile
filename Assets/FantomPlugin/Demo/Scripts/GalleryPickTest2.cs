using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using FantomLib;

// Demonstration of get image / movie information from the gallery.
//(*) For the method of setting 360 degrees (whole sphere) object, please refer to the following.
// http://fantom1x.blog130.fc2.com/blog-entry-297.html
//(*) Please download whole sphere mesh 'Sphere100.fbx' from the URL.
// http://warapuri.com/post/131599525953/
//(*) For the sake of simplicity, this demo does not take into account performance (eg RenderTexture seems to be heavily loaded).
//    Since VideoPlayer has another method such as Material Override, if you use it for an actual application, 
//    it may be better to change the implementation method better performance.
//(*) When saving a screenshot to External Storage, the following permission is required for 'AndroidManifest.xml'.
//   '<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'
//
//
// ギャラリーからの画像/動画情報取得のデモ
//※360度（全天球）のオブジェクトの設定方法は以下を参照して下さい。
// http://fantom1x.blog130.fc2.com/blog-entry-297.html
//※全天球のメッシュ「Sphere100.fbx」は以下からダウンロードして下さい。
// http://warapuri.com/post/131599525953/
//※このデモでは簡略のため、パフォーマンスは考慮に入れてません（例えば RenderTexture は負荷が高いと思われる）。
//　VideoPlayer には Material Override など、別の方法もあるので、実際のアプリに使用する場合は、実装方法を変更した方が良いパフォーマンスを得られる場合があります。
//※スクリーンショットをストレージに保存する場合は、以下のパーミッションが「AndroidManifest.xml」に必要になります。
//　'<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />' in 'AndroidManifest.xml'
public class GalleryPickTest2 : MonoBehaviour {

    public Material textureMat;                 //Material applying texture.                                //テクスチャを適用するマテリアル
    public Image image;                         //Image to apply texture.                                   //テクスチャを適用する画像
    public bool fitOrientation = true;          //Rotate image according to 'orientation' info.             //'orientation' 情報に合わせて、画像を回転する
    public GameObject cube;                     //Cube object to apply texture.                             //テクスチャを適用するキューブオブジェクト
    public GameObject sphere;                   //Whole sphere (for 360 degrees image) to apply texture.    //テクスチャを適用する全天球オブジェクト
    public GameObject chara;                    //Character Model or other (Texture does not apply)         //表示するキャラクターなど（テクスチャは適用しない）

    public VideoPlayer videoPlayer;
    public Material videoRenderTextureMat;      //Material applying render texture.                         //テクスチャを適用するマテリアル
    public bool fitRenderTexture = true;        //Generate render texture dynamically according to video size.//動画サイズに合わせて、動的にテクスチャを生成する。
    public RawImage videoImage;                 //RawImage to apply texture (for video).                    //テクスチャを適用する画像（動画用）
    public GameObject videoCube;                //Cube object to apply texture.                             //テクスチャを適用するキューブオブジェクト
    public GameObject videoSphere;              //Whole sphere (for 360 degrees image) to apply texture.    //テクスチャを適用する全天球オブジェクト
    public Button playVideoButton;
    public Button stopVideoButton;

    public GameObject UIVisibilityButton;       //Ui visible on/off for 360 degree

    public SmoothFollow3 smoothFollow3;

    public GameObject[] hideUIOnScreenshot;     //UI to hide in screenshot.
    public GameObject[] hideUIOnUIVisibility;   //UI to hide in UI visible for 360 degrees.

    public Screenshot screenshot;               //Screenshot function

    //Mainly 'ToastController.Show' is called.
    public ToastController toastControl;

    //Mainly 'MediaScannerController.StartScan' is called.
    public MediaScannerController mediaScannerControl;

    public SendTextController sendTextControl;  //For share contents
    public Button shareButton;                  //Share (Send text) button

    public MailerController mailerControl;      //For mail attachment (Email address is input to application)
    public Button mailButton;                   //Mail button


    public SystemLanguage localizeLanguage = SystemLanguage.Unknown;    //current localize language

    //Saved message
    public LocalizeString savedMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Save ScreenShot completed."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "スクリーンショットが保存されました。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "屏幕截图已保存。"),
            new LocalizeString.Data(SystemLanguage.Korean, "스크린 샷이 저장되었습니다."),
        });

    //Share message
    public LocalizeString shareText = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Share the screenshots!"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "スクリーンショットをシェアするよ！"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "我将分享截图！"),
            new LocalizeString.Data(SystemLanguage.Korean, "스크린 샷을 공유하는거야!"),
        });


    //==========================================================
    // Local Values

    private int defaultWidth = 384;     //Reference value of width, substitute value when acquisition fails (It will be initialized with UI size).     //幅の基準値、または取得に失敗したときの代替値（UIのサイズで初期化されます）
    private int defaultHeight = 384;    //Reference value of height, substitute value when acquisition fails (It will be initialized with UI size).    //高の基準値、または取得に失敗したときの代替値（UIのサイズで初期化されます）


    //==========================================================

    // Use this for initialization
    private void Start () {
        if (image != null)
        {
            Vector2 size = image.rectTransform.sizeDelta;
            defaultWidth = (int)size.x;
            defaultHeight = (int)size.y;
        }

        ToggleImageObject(true);
        ToggleCubeObject(false);
        ToggleSphereObject(false);
        SetEnableShareButtons(false);
        SetEnableVideoButtons(false);
        SetEnableUIVisibilityButton(false);
        OnCharaClick(false);

        if (smoothFollow3 != null)
            initValidArea = smoothFollow3.validArea;

#if !UNITY_EDITOR && UNITY_ANDROID
        //XDebug.Log("'WRITE_EXTERNAL_STORAGE' permission = " + AndroidPlugin.CheckPermission("android.permission.WRITE_EXTERNAL_STORAGE"));
#endif
    }

    // Update is called once per frame
    //private void Update () {

    //}


    //==========================================================
    //Display Log

    public void DisplayLogPermission(string permission, bool granted)
    {
        XDebug.Log(permission.Replace("android.permission.", "") + " = " + granted);
    }


    //==========================================================
    //UI

    enum Mode { Image, Cube, Sphere }
    Mode mode = Mode.Image;
    bool isVideoLoad = false;

    //Callback handeler when switch UI image.      //UI の Image
    public void OnImageModeClick(bool isOn)
    {
        ToggleImageObject(isOn);
        SetEnableUIVisibilityButton(false);
        mode = Mode.Image;
    }

    public void ToggleImageObject(bool isOn)
    {
        if (image != null)
            image.gameObject.SetActive(isOn && !isVideoLoad);
        if (videoImage != null)
            videoImage.gameObject.SetActive(isOn && isVideoLoad);
    }

    //Callback handeler when switch cube object.   //Cube
    public void OnCubeModeClick(bool isOn)
    {
        ToggleCubeObject(isOn);
        SetEnableUIVisibilityButton(false);
        mode = Mode.Cube;
    }

    public void ToggleCubeObject(bool isOn)
    {
        if (cube != null)
            cube.SetActive(isOn && !isVideoLoad);
        if (videoCube != null)
            videoCube.SetActive(isOn && isVideoLoad);
    }

    //Callback handeler when switch whole sphere (360 degrees).    //全天球（360度）
    public void OnSphereModeClick(bool isOn)
    {
        ToggleSphereObject(isOn);
        SetEnableUIVisibilityButton(true);
        mode = Mode.Sphere;
    }

    public void ToggleSphereObject(bool isOn)
    {
        if (sphere != null)
            sphere.SetActive(isOn && !isVideoLoad);
        if (videoSphere != null)
            videoSphere.SetActive(isOn && isVideoLoad);
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
    public void OnGalleryPick(ImageInfo info)
    {
        XDebug.Log("OnGalleryPick: " + info);
        isVideoLoad = false;
        StopVideo();
        SetEnableVideoButtons(false);
        ToggleImageObject(mode == Mode.Image);
        ToggleCubeObject(mode == Mode.Cube);
        ToggleSphereObject(mode == Mode.Sphere);

        int width = info.width > 0 ? info.width : defaultWidth;         //Alternate value when width get failed.    //幅の取得に失敗したときの代替値
        int height = info.height > 0 ? info.height : defaultHeight;     //Alternate value when height get failed.   //高さの取得に失敗したときの代替値
        int orientation = fitOrientation ? info.orientation : 0;        //It also becomes 0 even if get failed.     //取得に失敗したときにも 0 となる
        LoadAndSetImage(info.path, width, height, orientation);
 
        lastContent = info;
        SetEnableShareButtons(!string.IsNullOrEmpty(lastContent.uri));
    }

    //Image loading and setting.    //画像の読み込みとセット
    private void LoadAndSetImage(string path, int width, int height, int orientation)
    {
        Texture2D texture = LoadToTexture2D(path, width, height, TextureFormat.ARGB32, false, FilterMode.Bilinear);
        if (texture != null && image != null)
        {
            RectTransform rt = image.rectTransform;
            orientation = (int)Mathf.Repeat(orientation, 360);

            int w, h;
            if (orientation == 90 || orientation == 270)
            {
                w = defaultWidth;
                h = height * w / width;
            }
            else
            {
                h = defaultHeight;
                w = width * h / height;
            }

            rt.sizeDelta = new Vector2(w, h);   //Make it the same ratio as the image.  //画像と同じ比率にする
            rt.localRotation = Quaternion.Euler(0, 0, -orientation);

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

    //Load the image from the specified path and generates a Texture2D.     //指定パスから画像を読み込み、テクスチャを生成する。
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
        XDebug.Log("GalleryPickTest2.OnError : " + message);
    }


    //==========================================================
    //Screenshot

    //Run screenshot
    public void ScreenShot()
    {
        if (screenshot == null || screenshot.IsSaving)
            return;     //Ignore while saving.

        StartCoroutine(StartScreenshot());
    }

    //Hide the UI and execute the screenshot. If save the screenshot successfully, run MeidaScanner.
    private IEnumerator StartScreenshot()
    {
        SetVisibleUI(false);
        yield return null;
        screenshot.StartScreenshot();
    }

    //Toggle display of UI when screenshot
    public void SetVisibleUI(bool visible)
    {
        foreach (var item in hideUIOnScreenshot)
            item.SetActive(visible);
    }

    //Callback handler when screen shot fails
    public void ReceiveScreenshotError(string message)
    {
        SetVisibleUI(true);
        SetEnableUIVisibilityButton(mode == Mode.Sphere);
        XDebug.Log("Error Screenshot : " + message);
    }

    //Callback handler when screenshot succeeds
    public void ReceiveScreenshotComplete(string path)
    {
        SetVisibleUI(true);
        SetEnableUIVisibilityButton(mode == Mode.Sphere);

        if (toastControl != null)
            toastControl.Show(savedMessage.TextByLanguage(localizeLanguage));

        if (mediaScannerControl != null)
            mediaScannerControl.StartScan(path);

        XDebug.Log("Save to : " + path);
    }

    ContentInfo lastContent;     //For share file (Only last loaded)

    //Callback handler when MediaScanner scan completed.
    public void ReceiveMediaScan(ContentInfo info)
    {
        XDebug.Log("ReceiveMediaScan : " + info);

        lastContent = info;
        SetEnableShareButtons(!string.IsNullOrEmpty(lastContent.uri));
    }

    //UI-Buttons on/off
    public void SetEnableShareButtons(bool enable)
    {
        if (shareButton != null)
            shareButton.interactable = enable;
        if (mailButton != null)
            mailButton.interactable = enable;
    }

    //Share screenshot
    public void ShareScreenshot()
    {
        if (lastContent == null)
            return;

        string uri = lastContent.uri;
        string path = lastContent.path;
        XDebug.Log("Last content : path = " + path + ", uri = " + uri);
        
        if (string.IsNullOrEmpty(uri))
            return;

        if (sendTextControl != null)
            sendTextControl.Send(shareText.TextByLanguage(localizeLanguage), uri);
    }

    //Attach the image to an email and show
    //(*) However, when adding an attached file, it is the same method as 'Send text + attached file' (Other than the mailer is displayed).
    //※ただし、添付ファイルを追加する場合は、「テキスト送信＋添付ファイル」と同じ方法になる（メーラー以外も表示される）。
    public void SendMailScreenshot()
    {
        if (lastContent == null)
            return;

        string uri = lastContent.uri;
        string path = lastContent.path;
        XDebug.Log("Last content : path = " + path + ", uri = " + uri);

        if (string.IsNullOrEmpty(uri))
            return;

        if (mailerControl != null)
        {
            mailerControl.SetAttachment(uri);
            mailerControl.Show();
        }
    }

    //Callback handler for 'LocalizeLanguageChanger'
    public void OnLanguageChanged(SystemLanguage language)
    {
        XDebug.Log("Localize language changed (Share, Mail) : " + language);
        localizeLanguage = language;
    }


    //==========================================================
    //Gallery pick and load video

    public void OnGalleryVideoPick(VideoInfo info)
    {
        XDebug.Log("OnGalleryVideoPick: " + info);
        isVideoLoad = true;
        StopVideo();
        ToggleImageObject(mode == Mode.Image);
        ToggleCubeObject(mode == Mode.Cube);
        ToggleSphereObject(mode == Mode.Sphere);

        int width = info.width > 0 ? info.width : defaultWidth;       //Alternate value when width get failed.    //幅の取得に失敗したときの代替値
        int height = info.height > 0 ? info.height : defaultHeight;   //Alternate value when height get failed.   //高さの取得に失敗したときの代替値

        if (fitRenderTexture)   //If false, the render texture set in the material is used.     //falseの場合、マテリアルに設定されているテクスチャが使われる。
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            renderTexture.Create();
            if (videoRenderTextureMat != null)
                videoRenderTextureMat.mainTexture = renderTexture;
            if (videoImage != null)
                videoImage.texture = renderTexture;
            if (videoPlayer != null)
                videoPlayer.targetTexture = renderTexture;
        }

        if (videoImage != null)
        {
            RectTransform rt = videoImage.rectTransform;
            int h = (int)rt.sizeDelta.y;
            int w = width * h / height;
            rt.sizeDelta = new Vector2(w, h);   //Make the same ratio as the image with the height as the reference.  //縦を基準として画像と同じ比率にする
        }

        if (!string.IsNullOrEmpty(info.path))
        {
            string url = "file://" + info.path;
            if (videoPlayer != null)
            {
                videoPlayer.url = url;
                PlayVideo();
            }

            SetEnableVideoButtons(true);

            if (info.duration > 0)      //(*) Note that it may not be available depending on the application.    //※アプリによっては取得できないことがあるので注意。
            {
                long duration = info.duration;
                long ms = duration % 1000;
                long t = duration / 1000;   //ms -> sec
                long sec = t % 60;
                long min = (t / 60) % 60;
                long hour = t / 3600;
                string durationStr = hour.ToString("d2") + ":" + min.ToString("d2") + ":" + sec.ToString("d2") + "." + ms.ToString("d3");
                XDebug.Log("Time (duration) = " + durationStr);
            }
        }
        else
        {
            SetEnableVideoButtons(false);
        }

        lastContent = info;
        SetEnableShareButtons(!string.IsNullOrEmpty(lastContent.uri));
    }

    public void PlayVideo()
    {
        if (videoPlayer != null && !videoPlayer.isPlaying)
            videoPlayer.Play();
    }

    public void StopVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();
    }

    //Video operation enable on/off
    public void SetEnableVideoButtons(bool enable)
    {
        if (playVideoButton != null)
            playVideoButton.interactable = enable;
        if (stopVideoButton != null)
            stopVideoButton.interactable = enable;
    }

    //UI visibility button own on/off
    private void SetEnableUIVisibilityButton(bool enable)
    {
        if (UIVisibilityButton != null)
            UIVisibilityButton.SetActive(enable);
    }

    //UI on/off by button
    public void SetVisibleUIByButton(bool isUiOn)
    {
        foreach (var item in hideUIOnUIVisibility)
            item.SetActive(isUiOn);

        ToggleValidArea(isUiOn);
    }

    //SmoothFollow3.validArea at startup
    private Rect initValidArea;

    //When viewing at 360 degrees, you can drag in full screen
    private void ToggleValidArea(bool isUiOn)
    {
        if (smoothFollow3 != null)
        {
            if (isUiOn)
                smoothFollow3.validArea = initValidArea;        //default is the upper half of the screen
            else
                smoothFollow3.validArea = new Rect(0, 0, 1, 1); //full screen
        }
    }
}
