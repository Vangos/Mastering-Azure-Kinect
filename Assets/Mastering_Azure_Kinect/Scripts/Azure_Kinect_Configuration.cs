using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class Azure_Kinect_Configuration : MonoBehaviour
{
    [SerializeField] private FPS cameraFps = FPS.FPS30;
    [SerializeField] private ImageFormat colorFormat = ImageFormat.ColorBGRA32;
    [SerializeField] private ColorResolution colorResolution = ColorResolution.R1080p;
    [SerializeField] private DepthMode depthMode = DepthMode.NFOV_Unbinned;
    [SerializeField] private bool syncedImagesOnly = true;

    private Device kinect;

    private void Start()
    {
        int deviceCount = Device.GetInstalledCount();

        Debug.Log($"Found {deviceCount} devices.");

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
            {

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
