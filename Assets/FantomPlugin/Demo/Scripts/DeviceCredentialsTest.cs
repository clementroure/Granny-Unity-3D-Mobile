using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

// Open Device Credentials screen demo (API Level 21 or higher)
//
// Note: Processing after successful authentication should be done within the 'private' (or 'internal') method if possible (for security reasons).
//
//※認証成功後の処理はなるべく'private'(または'internal')メソッド内で行った方が良い（セキュリティのため）。
public class DeviceCredentialsTest : MonoBehaviour {

    //Register 'OKDialogController.Show' etc. in the inspector.
    public UnityEvent OnAPIAlert;           //Alert of "Lower API Level"

    //Register 'OKDialogController.Show' etc. in the inspector.
    public UnityEvent OnNotSupportAlert;    //Alert of "Not Supported"

    //Register 'ToastController.Show' in the inspector.
    [Serializable] public class ToastHandler : UnityEvent<string> { }   //message
    public ToastHandler OnToast;


    //Local Values
    int apiLevel = 0;   //0: (not set)


    // Use this for initialization
    private void Start () {
#if !UNITY_EDITOR && UNITY_ANDROID
        apiLevel = AndroidPlugin.GetAPILevel();
#endif
    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}


    //Device Credentials demo
    public void OpenDeviceCredentials()
    {
        DeviceCredentialsController credentialController = FindObjectOfType<DeviceCredentialsController>();
        if (credentialController != null)
        {
            credentialController.Show(DeviceCredentialsSuccess);  //same as below  //以下と同じ
            //credentialController.SetOnSuccess(DeviceCredentialsSuccess);
            //credentialController.Show();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("DeviceCredentialsController not found.");
#endif
        }
    }

    //Callback handler when authorization success.
    //(*) 'public' scope is not good security.
    private void DeviceCredentialsSuccess()
    {
        string str = "Device Credentials : Success";
        XDebug.Log(str);
        if (OnToast != null)
            OnToast.Invoke(str);
    }

    //Callback handler when unauthorized, cancel.
    public void DeviceCredentialsFailure()
    {
        string str = "Device Credentials : Unauthorized, Cancel";
        XDebug.Log(str);
        if (OnToast != null)
            OnToast.Invoke(str);
    }

    //Callback handler when not supported, security is turned off.
    public void DeviceCredentialsError(string message)
    {
        XDebug.Log("Device Credentials : " + message);
        if (message == "ERROR_NOT_SUPPORTED")
        {
            if (OnAPIAlert != null &&
                0 < apiLevel && apiLevel < 21)
            {
                OnAPIAlert.Invoke();
                return;
            }
            if (OnNotSupportAlert != null && apiLevel >= 21)
            {
                OnNotSupportAlert.Invoke();
                return;
            }
        }
        
        if (OnToast != null)
            OnToast.Invoke(message);
    }

}
