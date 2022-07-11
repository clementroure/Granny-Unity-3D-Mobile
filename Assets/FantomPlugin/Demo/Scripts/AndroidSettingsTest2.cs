using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Open Wifi setting, Request Bluetooth enable, Battery status, Device Orientation status and localize, etc.
public class AndroidSettingsTest2 : MonoBehaviour {

    //Inspector Settings
    public bool checkReachableNetwork = true;
    public bool checkSystemInfoBattery = true;


    //Messages
    public LocalizeString notReachableMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Internet not reachable"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "ネットワークに到達できない"),
        });

    public LocalizeString reachableViaCarrierDataNetworkMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Internet reachable via Carrier Data Network"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "キャリアデータネットワーク経由で到達できる"),
        });

    public LocalizeString reachableViaLocalAreaNetworkMessage = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Internet reachable via Local Area Network"),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "Wifiまたはケーブル経由で到達できる"),
        });


    //==========================================================

    // Use this for initialization
    private void Start () {
        if (checkReachableNetwork)
            CheckReachableNetwork();
        if (checkSystemInfoBattery)
            CheckSystemInfoBattery();
    }
    
    // Update is called once per frame
    //private void Update () {
        
    //}

    
    //==========================================================

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


    //==========================================================
    //Wifi settings demo

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


    //==========================================================
    //Bluetooth request demo

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


    //==========================================================
    //Battery demo

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
    public Text batteryVoltageDisplay;

    //Callback handler from BatteryStatusController.OnStatus
    public void ReceiveBatteryStatus(BatteryInfo info)
    {
        //XDebug.Log(info);

        if (batteryTimeDisplay != null)
            batteryTimeDisplay.text = "Update : " + info.timestamp.Split(' ')[1];

        if (batteryLevelDisplay != null)
            batteryLevelDisplay.text = info.percent + "% (" + info.level + "/" + info.scale + ")";

        if (batteryStatusDisplay != null)
            batteryStatusDisplay.text = "Status : " + info.status;

        if (batteryHealthDisplay != null)
            batteryHealthDisplay.text = "Health : " + info.health;

        if (batteryTemperatureDisplay != null)
            batteryTemperatureDisplay.text = "Temperature : " + info.temperature + " ℃";

        if (batteryVoltageDisplay != null)
            batteryVoltageDisplay.text = "Voltage : " + info.voltage.ToString("F2") + " V";
    }


    //==========================================================
    //Orientation demo

    //Callback handler from OrientationStatusController.OnStatusChanged
    public void ReceiveOrientationChange(string status)
    {
        DateTime dt = DateTime.Now;
        XDebug.Log("[" + dt.ToString("HH:mm:ss") + "] " + status);
        XDebug.Log("Input.deviceOrientation = " + Input.deviceOrientation);
    }


    //==========================================================

    public Text cpuRateDisplay;
    public CpuRateBar[] cpuRateBars;

    StringBuilder sb = new StringBuilder(512);
    string format = "{0}: User:{1:F1}% Nice:{2:F1}% Sys:{3:F1}% Idle:{4:F1}% Ratio:{5:F1}%";
    
    public void RecieveCpuRates(CpuRateInfo[] infos)
    {
        sb.Length = 0;
        int len = Mathf.Max(infos.Length, cpuRateBars.Length);
        for (int i = 0; i < len; i++)
        {
            if (i < infos.Length)
            {
                var item = infos[i];
                sb.AppendFormat(format, item.name, item.user, item.nice, item.system, item.idle, item.ratio);
                sb.Append("\n");
            }

            if (i < cpuRateBars.Length)
            {
                if (i < infos.Length)
                {
                    cpuRateBars[i].gameObject.SetActive(true);
                    cpuRateBars[i].SetCpuRate(infos[i]);
                }
                else
                {
                    cpuRateBars[i].gameObject.SetActive(false);
                }
            }
        }

        if (cpuRateDisplay != null)
            cpuRateDisplay.text = sb.ToString().Trim();
    }
}
