using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Open Wifi setting, Request Bluetooth enable, Battery status, Device Orientation status
public class AndroidSettingsTest : MonoBehaviour {

    //Messages
    public string notReachableMessage = "Internet not reachable";                                           //ネットワークに到達できない
    public string reachableViaCarrierDataNetworkMessage = "Internet reachable via Carrier Data Network";    //キャリアデータネットワーク経由で到達できる
    public string reachableViaLocalAreaNetworkMessage = "Internet reachable via Local Area Network";        //Wifiまたはケーブル経由で到達できる


    // Use this for initialization
    private void Start () {
        CheckReachableNetwork();
        CheckSystemInfoBattery();
        CheckPermissions();
    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}


    //Check for permissions are granted.    //パーミッションのチェック
    public void CheckPermissions()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        XDebug.Log("RECORD_AUDIO : " + (AndroidPlugin.CheckPermission("android.permission.RECORD_AUDIO") ? "Granted":"Denied"));
        XDebug.Log("WRITE_EXTERNAL_STORAGE : " + (AndroidPlugin.CheckPermission("android.permission.WRITE_EXTERNAL_STORAGE") ? "Granted":"Denied"));
        XDebug.Log("READ_EXTERNAL_STORAGE : " + (AndroidPlugin.CheckPermission("android.permission.READ_EXTERNAL_STORAGE") ? "Granted":"Denied"));  //If 'WRITE_EXTERNAL_STORAGE' is present, it gets granted.  //'WRITE_EXTERNAL_STORAGE' があれば、granted になる
        XDebug.Log("BLUETOOTH : " + (AndroidPlugin.CheckPermission("android.permission.BLUETOOTH") ? "Granted":"Denied"));
        XDebug.Log("VIBRATE : " + (AndroidPlugin.CheckPermission("android.permission.VIBRATE") ? "Granted":"Denied"));
        XDebug.Log("BODY_SENSORS : " + (AndroidPlugin.CheckPermission("android.permission.BODY_SENSORS") ? "Granted":"Denied"));
#endif
    }

    //Check for Internet connection.    //インターネット接続の確認
    public void CheckReachableNetwork()
    {
        switch (Application.internetReachability) {
        case NetworkReachability.NotReachable:
            XDebug.Log(notReachableMessage);
            break;
        case NetworkReachability.ReachableViaCarrierDataNetwork:
            XDebug.Log(reachableViaCarrierDataNetworkMessage);
            break;
        case NetworkReachability.ReachableViaLocalAreaNetwork:
            XDebug.Log(reachableViaLocalAreaNetworkMessage);
            break;
        }
    }

    //Wifi settings
    public void OpenWifiSettings()
    {
        XDebug.Log(DateTime.Now.ToString("HH:mm:ss") + " OpenWifiSettings called");
#if UNITY_EDITOR
        Debug.Log("OpenWifiSettings called");
#elif UNITY_ANDROID
        AndroidPlugin.OpenWifiSettings(gameObject.name, "ReceiveWifiResult");
        //AndroidPlugin.OpenWifiSettings();
#endif
    }

    //Callback handler when Wifi Settings closed
    public void ReceiveWifiResult(string result)
    {
        XDebug.Log(DateTime.Now.ToString("HH:mm:ss") + " ReceiveWifiResult : result = " + result);
        CheckReachableNetwork();
    }

    //Callback handler when an error occurs in WifiS ettings.
    public void ReceiveWifiError(string message)
    {
        XDebug.Log("ReceiveWifiError : " + message);
    }



    //Bluetooth request enable
    public void StartBluetoothRequestEnable()
    {
#if UNITY_EDITOR
        Debug.Log("StartBluetoothRequestEnable called");
#elif UNITY_ANDROID
        AndroidPlugin.StartBluetoothRequestEnable(gameObject.name, "ReceiveBluetoothResult");
        //AndroidPlugin.StartBluetoothRequestEnable();
#endif
    }

    //Callback handler when Bluetooth request enable finished.
    private void ReceiveBluetoothResult(string result)
    {
        XDebug.Log("ReceiveBluetoothResult : result = " + result);
    }

    //Callback handler when Bluetooth request enable finished (yes/no).
    public void ReceiveBluetoothResult(bool isOn)
    {
        XDebug.Log("ReceiveBluetoothResult : isOn = " + isOn);
    }

    //Callback handler when an error occurs in Bluetooth request enable.
    public void ReceiveBluetoothError(string message)
    {
        XDebug.Log("ReceiveBluetoothError : " + message);

    }


    //Check for SystemInfo.battery~
    public void CheckSystemInfoBattery()
    {
        XDebug.Log("SystemInfo.batteryStatus = " + SystemInfo.batteryStatus);
        XDebug.Log("SystemInfo.batteryLevel = " + SystemInfo.batteryLevel);
    }


    //Battery status details
    public Text batteryTimeDisplay;
    public Text batteryLevelDisplay;
    public Text batteryStatusDisplay;
    public Text batteryHealthDisplay;
    public Text batteryTemperatureDisplay;

    //Callback handler from BatteryStatusController.OnStatus
    public void ReceiveBatteryStatus(BatteryInfo info)
    {
        //XDebug.Log(info.timestamp + " : level = " + info.level + ", scale = " + info.scale + ", percent = " + info.percent + ", status = " + info.status + ", health = " + info.health + ", temperature = " + info.temperature);
        //XDebug.Log(info);

        if (batteryTimeDisplay != null)
            batteryTimeDisplay.text = "Last Update : " + info.timestamp.Split(' ')[1];

        if (batteryLevelDisplay != null)
            batteryLevelDisplay.text = info.percent + "% (" + info.level + "/" + info.scale + ")";

        if (batteryStatusDisplay != null)
            batteryStatusDisplay.text = "Status : " + info.status;

        if (batteryHealthDisplay != null)
            batteryHealthDisplay.text = "Health : " + info.health;

        if (batteryTemperatureDisplay != null)
            batteryTemperatureDisplay.text = "Temperature : " + info.temperature + " ℃";
    }


    //Callback handler from OrientationStatusController.OnStatusChanged
    public void ReceiveOrientationChange(string status)
    {
        DateTime dt = DateTime.Now;
        XDebug.Log("[" + dt.ToString("HH:mm:ss") + "] " + status);
        XDebug.Log("Input.deviceOrientation = " + Input.deviceOrientation);
    }
}
