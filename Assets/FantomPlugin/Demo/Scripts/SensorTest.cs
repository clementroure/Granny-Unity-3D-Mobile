using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FantomLib;

//Sensor values acquisition demo
//センサーの値取得デモ
//http://fantom1x.blog130.fc2.com/blog-entry-294.html
public class SensorTest : MonoBehaviour {

    public ScrollRect scrollRect;

    public Text displayAccelerometer;
    public Text displayMagneticField;
    public Text displayGyroscope;
    public Text displayLight;
    public Text displayPressure;
    public Text displayProximity;
    public Text displayGravity;
    public Text displayLinearAcceleration;
    public Text displayRotationVector;
    public Text displayRelativeHumidity;
    public Text displayAmbientTemperature;
    public Text displayMagneticFieldUncalibrated;
    public Text displayGameRotationVector;
    public Text displayGyroscopeUncalibrated;
    public Text displaySignificantMotion;
    public Text displayStepDetector;
    public Text displayStepCounter;
    public Text displayStepCounter2;
    public Text displayGeomagneticRotationVector;
    public Text displayHeartRate;
    public Text displayPose6DOF;
    public Text displayStationaryDetect;
    public Text displayMotionDetect;
    public Text displayHeartBeat;
    public Text displayLowLatencyOffbodyDetect;
    public Text displayAccelerometerUncalibrated;

    //Unity built-in
    public Text displayInputAcceleration;
    public Text displayInputGyro;
    public Text displayInputCompass;

    public Text displayTest;
    public int testSensorTypeInt;


    //Message when permission granted
    public LocalizeString messagePermissionGranted = new LocalizeString(SystemLanguage.English,
        new List<LocalizeString.Data>()
        {
            new LocalizeString.Data(SystemLanguage.English, "Permission has been granted.\nPlease restart the application to activate the sensor."),    //default language
            new LocalizeString.Data(SystemLanguage.Japanese, "権限が許可されました。\nセンサーをアクティブにするにはアプリを再起動して下さい。"),
            new LocalizeString.Data(SystemLanguage.ChineseSimplified, "授权已被授予。\n重新启动应用程序以激活传感器。"),
            new LocalizeString.Data(SystemLanguage.Korean, "권한이 부여되었습니다.\n센서를 활성화하려면 응용 프로그램을 다시 시작하십시오."),
        });



    // Use this for initialization
    private void Start ()
    {
        InitDisplayNeedIsSupport();
    }

    // Update is called once per frame
    private void Update()
    {
        DisplayUnityBuiltinSensor();
    }

    //Fit the scroll view to the device rotation
    public void OnOrientationChanged(string orientation)
    {
        if (scrollRect == null)
            return;

        if (orientation == "PORTRAIT")
        {
            scrollRect.horizontal = true;
            scrollRect.normalizedPosition = Vector2.up;
            scrollRect.vertical = false;
        }
        else if (orientation == "LANDSCAPE")
        {
            scrollRect.vertical = true;
            scrollRect.normalizedPosition = Vector2.up;
            scrollRect.horizontal = false;
        }
    }

    //Needs check for IsSupportedSensor when display initialized
    private void InitDisplayNeedIsSupport()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        if (displaySignificantMotion != null)
            displaySignificantMotion.text = "SignificantMotion : " + (AndroidPlugin.IsSupportedSensor(SensorType.SignificantMotion) ? "(supported)" : "(not supported)");
        if (displayStepDetector != null)
            displayStepDetector.text = "StepDetector : " + (AndroidPlugin.IsSupportedSensor(SensorType.StepDetector) ? "(supported)" : "(not supported)");
        if (displayHeartRate != null)
        {
            if (!AndroidPlugin.CheckPermission("android.permission.BODY_SENSORS"))
                displayHeartRate.text = "HeartRate : (permission denied)";
            else
                displayHeartRate.text = "HeartRate : " + (AndroidPlugin.IsSupportedSensor(SensorType.HeartRate) ? "(supported)" : "(not supported)");
        }
        if (displayStationaryDetect != null)
            displayStationaryDetect.text = "StationaryDetect : " + (AndroidPlugin.IsSupportedSensor(SensorType.StationaryDetect) ? "(supported)" : "(not supported)");
        if (displayMotionDetect != null)
            displayMotionDetect.text = "MotionDetect : " + (AndroidPlugin.IsSupportedSensor(SensorType.MotionDetect) ? "(supported)" : "(not supported)");
        if (displayHeartBeat != null)
            displayHeartBeat.text = "HeartBeat : " + (AndroidPlugin.IsSupportedSensor(SensorType.HeartBeat) ? "(supported)" : "(not supported)");
        if (displayLowLatencyOffbodyDetect != null)
            displayLowLatencyOffbodyDetect.text = "LowLatencyOffbodyDetect : " + (AndroidPlugin.IsSupportedSensor(SensorType.LowLatencyOffbodyDetect) ? "(supported)" : "(not supported)");

        if (displayTest != null && testSensorTypeInt > 0)
            displayTest.text = testSensorTypeInt + " : " + (AndroidPlugin.IsSupportedSensor(testSensorTypeInt) ? "(supported)" : "(not supported)");
#endif
        //Unity built-in
        if (displayInputGyro != null)
            Input.gyro.enabled = true;

        if (displayInputCompass != null)
            Input.compass.enabled = true;
    }

    private void DisplayUnityBuiltinSensor()
    {
        if (displayInputAcceleration != null)
            displayInputAcceleration.text = "Input.acceleration : x=" + Input.acceleration.x + ", y=" + Input.acceleration.y + ", z=" + Input.acceleration.z;

        if (displayInputGyro != null)
            displayInputGyro.text = "Input.gyro.attitude.eulerAngles : x=" + Input.gyro.attitude.eulerAngles.x + ", y=" + Input.gyro.attitude.eulerAngles.y + ", z=" + Input.gyro.attitude.eulerAngles.z;

        if (displayInputCompass != null)
            displayInputCompass.text = "Input.compass.rawVector : x=" + Input.compass.rawVector.x + ", y=" + Input.compass.rawVector.y + ", z=" + Input.compass.rawVector.z;
    }

    public void OnSensorChanged(int sensorType, float[] values)
    {
        DateTime dt = DateTime.Now;
        string timeStr = dt.ToString("HH:mm:ss");

        switch (sensorType)
        {
            case (int)SensorType.Accelerometer:
                if (displayAccelerometer != null)
                    displayAccelerometer.text = "Accelerometer : x=" + values[0] + ", y=" + values[1] + ", z=" + values[2] + " [rad/s]";
                break;
            case (int)SensorType.MagneticField:
                if (displayMagneticField != null)
                    displayMagneticField.text = "MagneticField : x=" + values[0] + ", y=" + values[1] + ", z=" + values[2] + " [uT]";
                break;
            case (int)SensorType.Gyroscope:
                if (displayGyroscope != null)
                    displayGyroscope.text = "Gyroscope : x=" + values[0] + ", y=" + values[1] + ", z=" + values[2] + " [rad/s]";
                break;
            case (int)SensorType.Light:
                if (displayLight != null)
                    displayLight.text = "Light : " + values[0] + " [lux]";
                break;
            case (int)SensorType.Pressure:
                if (displayPressure != null)
                    displayPressure.text = "Pressure : " + values[0] + " [hPa]";
                break;
            case (int)SensorType.Proximity:
                if (displayProximity != null)
                    displayProximity.text = "Proximity : " + values[0] + " [cm]";
                break;
            case (int)SensorType.Gravity:
                if (displayGravity != null)
                    displayGravity.text = "Gravity : " + values[0] + " [m/s^2]";
                break;
            case (int)SensorType.LinearAcceleration:
                if (displayLinearAcceleration != null)
                    displayLinearAcceleration.text = "LinearAcceleration : x=" + values[0] + ", y=" + values[1] + ", z=" + values[2] + " [rad/s]";
                break;
            case (int)SensorType.RotationVector:
                if (displayRotationVector != null)
                    displayRotationVector.text = "RotationVector : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());   //（※[0]~[2]:API 9, [3][4]:API 18）
                break;
            case (int)SensorType.RelativeHumidity:
                if (displayRelativeHumidity != null)
                    displayRelativeHumidity.text = "RelativeHumidity : " + values[0] + " [%]";
                break;
            case (int)SensorType.AmbientTemperature:
                if (displayAmbientTemperature != null)
                    displayAmbientTemperature.text = "AmbientTemperature : " + values[0] + " [℃]";
                break;
            case (int)SensorType.MagneticFieldUncalibrated:
                if (displayMagneticFieldUncalibrated != null)
                    displayMagneticFieldUncalibrated.text = "MagneticFieldUncalibrated : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());
                break;
            case (int)SensorType.GameRotationVector:
                if (displayGameRotationVector != null)
                    displayGameRotationVector.text = "GameRotationVector : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());
                break;
            case (int)SensorType.GyroscopeUncalibrated:
                if (displayGyroscopeUncalibrated != null)
                    displayGyroscopeUncalibrated.text = "GyroscopeUncalibrated : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());
                break;
            case (int)SensorType.SignificantMotion:
                if (displaySignificantMotion != null)
                {
                    displaySignificantMotion.text = "SignificantMotion : " + values[0] + " [" + timeStr + "] (last detect)";
                    Invoke("SignificantMotionSensorReregistration", SignificantMotionRestartTime);
                }
                break;
            case (int)SensorType.StepDetector:
                if (displayStepDetector != null)
                    displayStepDetector.text = "StepDetector : " + values[0] + " [" + timeStr + "] (last detect)";  //values[0] is always 1.0 (trigger).
                break;
            case (int)SensorType.StepCounter:
                if (displayStepCounter != null)
                    displayStepCounter.text = "StepCounter(system boot) : " + values[0] + " [steps]";
                break;
            case (int)SensorType.GeomagneticRotationVector:
                if (displayGeomagneticRotationVector != null)
                    displayGeomagneticRotationVector.text = "GeomagneticRotationVector : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());
                break;
            case (int)SensorType.HeartRate:
                if (displayHeartRate != null)
                    displayHeartRate.text = "HeartRate : " + values[0] + " [bpm]";
                break;
            case (int)SensorType.Pose6DOF:
                if (displayPose6DOF != null)
                    displayPose6DOF.text = "Pose6DOF : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());
                break;
            case (int)SensorType.StationaryDetect:
                if (displayStepDetector != null)
                    displayStepDetector.text = "StationaryDetect : " + values[0] + " [" + timeStr + "] (last detect)";  //values[0] is always 1.0 (trigger).
                break;
            case (int)SensorType.MotionDetect:
                if (displayMotionDetect != null)
                    displayMotionDetect.text = "MotionDetect : " + values[0] + " [" + timeStr + "] (last detect)";  //values[0] is always 1.0 (trigger).
                break;
            case (int)SensorType.HeartBeat:
                if (displayHeartBeat != null)
                    displayHeartBeat.text = "HeartBeat : " + values[0] + " [" + timeStr + "] (confidence)";  //confidence = 0~1
                break;
            case (int)SensorType.LowLatencyOffbodyDetect:
                if (displayLowLatencyOffbodyDetect != null)
                    displayLowLatencyOffbodyDetect.text = "LowLatencyOffbodyDetect : " + values[0] + " [" + timeStr + "] (off/on-body)";  //confidence = 0~1
                break;
            case (int)SensorType.AccelerometerUncalibrated:
                if (displayAccelerometerUncalibrated != null)
                    displayAccelerometerUncalibrated.text = "AccelerometerUncalibrated : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());
                break;
            default:
                if (displayTest != null && testSensorTypeInt > 0 && sensorType == testSensorTypeInt)
                    displayTest.text = "Sensor ID " + testSensorTypeInt + " : " + string.Join(", ", values.Select(e => e.ToString()).ToArray());
                break;
        }
    }

    public void OnAccelerometerSensorChanged(float x, float y, float z)
    {
        if (displayAccelerometer != null)
            displayAccelerometer.text = "Accelerometer : x=" + x + ", y=" + y + ", z=" + z + " [m/s^2]";
    }

    public void OnMagneticFieldSensorChanged(float x, float y, float z)
    {
        if (displayMagneticField != null)
            displayMagneticField.text = "MagneticField : x=" + x + ", y=" + y + ", z=" + z + " [uT]";
    }

    public void OnGyroscopeSensorChanged(float x, float y, float z)
    {
        if (displayGyroscope != null)
            displayGyroscope.text = "Gyroscope : x=" + x + ", y=" + y + ", z=" + z+ " [rad/s]";
    }

    public void OnLightSensorChanged(float value)
    {
        if (displayLight != null)
            displayLight.text = "Light : " + value + " [lux]";
    }

    public void OnPressureSensorChanged(float value)
    {
        if (displayPressure != null)
            displayPressure.text = "Pressure : " + value + " [hPa]";
    }

    public void OnProximitySensorChanged(float value)
    {
        if (displayProximity != null)
            displayProximity.text = "Proximity : " + value + " [cm]";
    }

    public void OnGravitySensorChanged(float value)
    {
        if (displayGravity != null)
            displayGravity.text = "Gravity : " + value + " [m/s^2]";
    }

    public void OnLinearAccelerationSensorChanged(float x, float y, float z)
    {
        if (displayLinearAcceleration != null)
            displayLinearAcceleration.text = "LinearAcceleration : x=" + x + ", y=" + y + ", z=" + z + " [m/s^2]";
    }

    public void OnRotationVectorSensorChanged(float x, float y, float z)
    {
        if (displayRotationVector != null)
            displayRotationVector.text = "RotationVector : x=" + x + ", y=" + y + ", z=" + z;
    }

    public void OnRelativeHumiditySensorChanged(float value)
    {
        if (displayRelativeHumidity != null)
            displayRelativeHumidity.text = "RelativeHumidity : " + value + " [%]";
    }

    public void OnAmbientTemperatureSensorChanged(float value)
    {
        if (displayAmbientTemperature != null)
            displayAmbientTemperature.text = "AmbientTemperature : " + value + " [℃]";
    }

    public void OnGameRotationVectorSensorChanged(float x, float y, float z)
    {
        if (displayGameRotationVector != null)
            displayGameRotationVector.text = "GameRotationVector : x=" + x + ", y=" + y + ", z=" + z;
    }

    const float SignificantMotionRestartTime = 1f;

    public void OnSignificantMotionSensorChanged()
    {
        if (displaySignificantMotion != null)
        {
            DateTime dt = DateTime.Now;
            string timeStr = dt.ToString("HH:mm:ss");
            displaySignificantMotion.text = "SignificantMotion : " + timeStr + " (last detect)";
            Invoke("SignificantMotionSensorReregistration", SignificantMotionRestartTime);
        }
    }

    //Once SignificantMotionSensor triggers, it automatically becomes disable, so start again (It restart when Pause->Resume of the application).
    //継続センサーは一度トリガしたら自動的に無効になるため、再度開始する（アプリの Pause → Resume 時にも再開される）。
    private void SignificantMotionSensorReregistration()
    {
        SignificantMotionController significantControl = FindObjectOfType<SignificantMotionController>();
        if (significantControl != null)
            significantControl.StartListening();
    }

    public void OnStepDetectorSensorChanged()
    {
        if (displayStepDetector != null)
        {
            DateTime dt = DateTime.Now;
            string timeStr = dt.ToString("HH:mm:ss");
            displayStepDetector.text = "StepDetector : " + timeStr + " (last detect)";  //sensor value is always 1.0 (trigger).
        }
    }

    public void OnStepCounterSensorChanged(int value)
    {
        if (displayStepCounter2 != null)
            displayStepCounter2.text = "StepCounter(since start) : " + value + " [steps]";
    }

    public void OnGeomagneticRotationVectorSensorChanged(float x, float y, float z)
    {
        if (displayGeomagneticRotationVector != null)
            displayGeomagneticRotationVector.text = "GeomagneticRotationVector : x=" + x + ", y=" + y + ", z=" + z;
    }

    public void OnHeartRateSensorChanged(float value)
    {
        if (displayHeartRate!= null)
            displayHeartRate.text = "HeartRate : " + value + " [bpm]";
    }

    public void OnStationaryDetectSensorChanged()
    {
        if (displayStationaryDetect != null)
        {
            DateTime dt = DateTime.Now;
            string timeStr = dt.ToString("HH:mm:ss");
            displayStationaryDetect.text = "StationaryDetect : " + timeStr + " (last detect)";  //sensor value is always 1.0 (trigger).
        }
    }

    public void OnMotionDetectSensorChanged()
    {
        if (displayMotionDetect != null)
        {
            DateTime dt = DateTime.Now;
            string timeStr = dt.ToString("HH:mm:ss");
            displayMotionDetect.text = "MotionDetect : " + timeStr + " (last detect)";  //sensor value is always 1.0 (trigger).
        }
    }

    public void OnHeartBeatSensorChanged(float value)
    {
        if (displayHeartBeat != null)
        {
            DateTime dt = DateTime.Now;
            string timeStr = dt.ToString("HH:mm:ss");
            displayHeartBeat.text = "HeartBeat : " + value + "[" + timeStr + "] (confidence)";  //[confidence=0~1]
        }
    }

    public void OnLowLatencyOffbodyDetectSensorChanged(bool flg)
    {
        if (displayLowLatencyOffbodyDetect != null)
        {
            DateTime dt = DateTime.Now;
            string timeStr = dt.ToString("HH:mm:ss");
            displayLowLatencyOffbodyDetect.text = "LowLatencyOffbodyDetect : " + (flg ? "on-body" : "off-body") + "[" + timeStr + "] (last detect)";  //[0 (device is off-body) or 1 (device is on-body)]
        }
    }

    public void OnOrientationSensorChanged(float x, float y, float z)
    {
        if (displayTest != null)
            displayTest.text = "Orientation : x=" + x + ", y=" + y + ", z=" + z;
    }


    //Register for 'PermissionCheckController.OnAllowed'
    public void OnPermissionAllowed(string permission)
    {
#if UNITY_EDITOR
        Debug.Log("OnPermissionAllowed : permission = " + permission);
#elif UNITY_ANDROID
        AndroidPlugin.ShowToast("'" + permission.Replace("android.permission.", "") + "' : " + messagePermissionGranted.Text, true);
#endif
    }

}
