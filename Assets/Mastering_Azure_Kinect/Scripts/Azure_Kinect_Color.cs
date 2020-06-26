using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.UI;
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
        }
    }

    private void Update()
    {
        if (kinect != null)
        {
            using (Capture capture = kinect.GetCapture())
            using (Image color = capture.Color)
            {
                Debug.Log(color.WidthPixels + "x" + color.HeightPixels + "x" + color.StrideBytes + " : " +
                          color.Memory.Length);

                if (texture == null)
                {
                    texture = new Texture2D(color.WidthPixels, color.HeightPixels, TextureFormat.BGRA32, false);
                    image.texture = texture;
                }

                texture.LoadRawTextureData(color.Memory.ToArray());
                texture.Apply();
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
