using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.UI;
using Image = Microsoft.Azure.Kinect.Sensor.Image;

public class Azure_Kinect_Depth : MonoBehaviour
{
    [SerializeField] private KinectConfiguration configuration;
    [SerializeField] private RawImage image;

    [Range(0, 10000)]
    [SerializeField] private ushort maxDepth = 6000;
    [SerializeField] private DepthVisualization visualization;

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

            int depthWidth = kinect.GetCalibration().DepthCameraCalibration.ResolutionWidth;
            int depthHeight = kinect.GetCalibration().DepthCameraCalibration.ResolutionHeight;

            texture = new Texture2D(depthWidth, depthHeight, TextureFormat.RGB24, false);
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
        using (Image depth = capture.Depth)
        {
            ushort[] depthData = MemoryMarshal.Cast<byte, ushort>(depth.Memory.Span).ToArray();

            byte[] pixels =
                visualization == DepthVisualization.Gray
                    ? Grayscale(depthData)
                    : Jet(depthData);

            texture.LoadRawTextureData(pixels);
            texture.Apply();
        }
    }

    private void OnDestroy()
    {
        kinect?.StopCameras();
        kinect?.Dispose();
    }

    private byte[] Grayscale(ushort[] data)
    {
        const int channels = 3; // RGB has 3 channels.
        const int maxByte = byte.MaxValue;

        byte[] pixels = new byte[data.Length * channels];

        for (int i = 0; i < data.Length; i++)
        {
            ushort depth = data[i];

            if (depth > 0)
            {
                byte gray = (byte)((float)depth / maxDepth * maxByte);

                pixels[i * channels + 0] = gray;
                pixels[i * channels + 1] = gray;
                pixels[i * channels + 2] = gray;
            }
        }

        return pixels;
    }

    private byte[] Jet(ushort[] data)
    {
        const int channels = 3; // RGB has 3 channels.
        const int maxByte = 255;

        float min = -1.0f;
        float max = 1.0f;

        byte[] pixels = new byte[data.Length * channels];

        for (int i = 0; i < data.Length; i++)
        {
            ushort depth = data[i];

            if (depth > 0)
            {
                float t = depth * (max - min) / maxDepth + min;

                float red = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t - max)) * maxByte;
                float green = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t)) * maxByte;
                float blue = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t + max)) * maxByte;

                pixels[i * channels + 0] = (byte)red;
                pixels[i * channels + 1] = (byte)green;
                pixels[i * channels + 2] = (byte)blue;
            }
        }

        return pixels;
    }
}

enum DepthVisualization
{
    Gray,
    Jet
}
