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

            string serialNumber = kinect.SerialNum;

            Debug.Log($"Serial number: {serialNumber}");

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

                Debug.Log($"Color Resolution: {kinect.CurrentColorResolution}");
                Debug.Log($"Depth Mode: {kinect.CurrentDepthMode}");
            }
            catch
            {
                Debug.Log("Invalid camera configuration!");
            }
        }
    }

    private void Update()
    {
        if (kinect == null) return;
        if (kinect.CurrentColorResolution == ColorResolution.Off && kinect.CurrentDepthMode == DepthMode.Off) return;

        using (Capture capture = kinect.GetCapture())
        {
            Debug.Log($"Temperature: {capture.Temperature}°C");

            using (Image color = capture.Color)
            using (Image depth = capture.Depth)
            using (Image ir = capture.IR)
            {
                Debug.Log($"Color: {color.WidthPixels}x{color.HeightPixels}");
                Debug.Log($"Depth: {depth.WidthPixels}x{depth.HeightPixels}");
                Debug.Log($"IR: {ir.WidthPixels}x{ir.HeightPixels}");
            }
        }
    }

    private void OnDestroy()
    {
        kinect?.StopCameras();
        kinect?.Dispose();
    }
}
