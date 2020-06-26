using System;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class Azure_Kinect_Configuration : MonoBehaviour
{
    [SerializeField] private KinectConfiguration configuration;

    private Device kinect;

    private void Start()
    {
        int deviceCount = Device.GetInstalledCount();

        Debug.Log($"Found {deviceCount} device(s).");

        if (deviceCount > 0)
        {
            kinect = Device.Open();

            string serial = kinect.SerialNum;

            Debug.Log($"Serial number: {serial}");

            try
            {
                kinect.StartCameras(new DeviceConfiguration
                {
                    CameraFPS = configuration.CameraFps,
                    ColorFormat = configuration.ColorFormat,
                    ColorResolution = configuration.ColorResolution,
                    DepthMode = configuration.DepthMode,
                    WiredSyncMode = configuration.WiredSyncMode,
                    SynchronizedImagesOnly = configuration.SynchronizedImagesOnly,
                    DisableStreamingIndicator = configuration.DisableStreamingIndicator
                });
            }
            catch
            {
                Debug.Log("Invalid camera configuration!");
            }

            kinect.StartImu();
        }
    }

    private void Update()
    {
        if (kinect == null) return;
        if (kinect.CurrentColorResolution == ColorResolution.Off && kinect.CurrentDepthMode == DepthMode.Off) return;
        
        using (Capture capture = kinect.GetCapture())
        using (Image color = capture.Color)
        using (Image depth = capture.Depth)
        using (Image ir = capture.IR)
        {
            //Debug.Log(color.WidthPixels + "x" + color.HeightPixels + "x" + color.StrideBytes + " : " + color.Memory.Length);
        }
    }

    private void OnDestroy()
    {
        kinect?.Dispose();
    }
}
