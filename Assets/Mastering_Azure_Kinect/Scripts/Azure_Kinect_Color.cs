using System;
using System.Buffers;
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

            texture = new Texture2D(colorWidth, colorHeight, TextureFormat.BGRA32, false);
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
            // Faster method
            byte[] colorData = new byte[color.Size];

            unsafe
            {
                using (MemoryHandle pin = color.Memory.Pin())
                {
                    Marshal.Copy((IntPtr)pin.Pointer, colorData, 0, (int)color.Size);
                }
            }

            texture.LoadRawTextureData(colorData);
            texture.Apply();

            // Slower method
            //Color32[] colorData = color.GetPixels<Color32>().ToArray();

            //for (int i = 0; i < colorData.Length; i++)
            //{
            //    byte r = colorData[i].r;
            //    byte b = colorData[i].b;

            //    colorData[i].r = b;
            //    colorData[i].b = r;
            //}

            //texture.SetPixels32(colorData);
            //texture.Apply();
        }
    }

    private void OnDestroy()
    {
        kinect?.StopCameras();
        kinect?.Dispose();
    }
}
