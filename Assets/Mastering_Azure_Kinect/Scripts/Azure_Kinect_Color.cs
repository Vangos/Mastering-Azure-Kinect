using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Image = Microsoft.Azure.Kinect.Sensor.Image;

public class Azure_Kinect_Color : MonoBehaviour
{
    [SerializeField] private FPS cameraFps = FPS.FPS30;
    [SerializeField] private ImageFormat colorFormat = ImageFormat.ColorBGRA32;
    [SerializeField] private ColorResolution colorResolution = ColorResolution.R1080p;
    [SerializeField] private DepthMode depthMode = DepthMode.NFOV_Unbinned;
    [SerializeField] private bool syncedImagesOnly = true;

    private Device kinect;

    [SerializeField] private RawImage image;
    private Texture2D texture;

    private void Start()
    {
        int deviceCount = Device.GetInstalledCount();

        Debug.Log($"Found {deviceCount} device(s).");

        if (deviceCount > 0)
        {
            kinect = Device.Open();

            kinect.StartCameras(new DeviceConfiguration
            {
                CameraFPS = cameraFps,
                ColorFormat = colorFormat,
                ColorResolution = colorResolution,
                DepthMode = depthMode,
                SynchronizedImagesOnly = syncedImagesOnly
            });

            int colorWidth = kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            int colorHeight = kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

            texture = new Texture2D(colorWidth, colorHeight, TextureFormat.BGRA32, false);
            image.texture = texture;
        }
    }

    private void Update()
    {
        if (kinect != null)
        {
            using (Capture capture = kinect.GetCapture())
            using (Image color = capture.Color)
            {
                // Fastest method
                byte[] colorData = color.Memory.ToArray();
                texture.LoadRawTextureData(colorData);
                texture.Apply();

                // Slower method
                //Color32[] colorData = color.GetPixels<Color32>().ToArray();

                //for (int i = 0; i < colorData.Length; i++)
                //{
                //    Color32 c = colorData[i];
                //    byte r = c.r;
                //    byte g = c.g;
                //    byte b = c.b;
                //    byte a = c.a;

                //    colorData[i].a = a;
                //    colorData[i].r = b;
                //    colorData[i].g = g;
                //    colorData[i].b = r;
                //}

                //texture.SetPixels32(colorData);
                //texture.Apply();
            }
        }
    }

    private void OnDestroy()
    {
        if (kinect != null)
        {
            kinect.StopCameras();
            kinect.Dispose();
        }
    }
}
