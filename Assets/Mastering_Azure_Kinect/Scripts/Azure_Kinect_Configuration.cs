using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class Azure_Kinect_Configuration : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;

    private readonly KinectSensor _kinect = new KinectSensor();

    private void Start()
    {
        int deviceCount = Device.GetInstalledCount();

        Debug.Log($"Found {deviceCount} device(s).");

        _kinect.Start(_configuration);

        if (_kinect.IsRunning)
        {
            Debug.Log($"Color Resolution: {_kinect.Device.CurrentColorResolution}");
            Debug.Log($"Depth Mode: {_kinect.Device.CurrentDepthMode}");
        }
    }

    private void Update()
    {
        if (!_kinect.IsRunning) return;
        if (_kinect.Device.CurrentColorResolution == ColorResolution.Off && _kinect.Device.CurrentDepthMode == DepthMode.Off) return;

        KinectData frameData = _kinect.Update();

        if (frameData != null)
        {
            float temperature = frameData.Temperature;
            Debug.Log($"Temperature: {temperature}°C");
        }
    }

    private void OnDestroy()
    {
        _kinect.Stop();
    }
}
