using System.Runtime.InteropServices;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.UI;
using Image = Microsoft.Azure.Kinect.Sensor.Image;

public class Azure_Kinect_Color : MonoBehaviour
{
    [SerializeField] private KinectConfiguration configuration;
    [SerializeField] private RawImage image;

    private Device kinect;
    private Texture2D texture;

    private void Start()
    {
        int deviceCount = Device.GetInstalledCount();

        if (deviceCount > 0)
        {
            kinect = Device.Open();

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

            int colorWidth = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            int colorHeight = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

            texture = new Texture2D(colorWidth, colorHeight, TextureFormat.RGB24, false);
            image.texture = texture;
        }
        else
        {
            Debug.LogWarning("No Kinect devices available.");
        }
    }

    private void Update()
    {
        if (kinect == null) return;

        using (Capture capture = kinect.GetCapture())
        using (Image color = capture.Color)
        {
            byte[] colorData = MemoryMarshal.AsBytes(color.Memory.Span).ToArray();

            texture.LoadImage(colorData);
        }
    }

    private void OnDestroy()
    {
        kinect?.StopCameras();
        kinect?.Dispose();
    }
}
