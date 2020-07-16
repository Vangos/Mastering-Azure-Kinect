using System;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

/// <summary>
/// Exposes the configuration properties in the Unity Editor.
/// </summary>
[Serializable]
public class KinectConfiguration
{
    [SerializeField] private FPS cameraFps = FPS.FPS30;
    [SerializeField] private ImageFormat colorFormat = ImageFormat.ColorMJPG;
    [SerializeField] private ColorResolution colorResolution = ColorResolution.R1080p;
    [SerializeField] private DepthMode depthMode = DepthMode.NFOV_Unbinned;
    [SerializeField] private WiredSyncMode wiredSyncMode = WiredSyncMode.Standalone;
    [SerializeField] private bool synchronizedImagesOnly = true;
    [SerializeField] private bool disableStreamingIndicator = false;

    /// <summary>
    /// The desired frame rate of the camera.
    /// ATTENTION: If the camera resolution does not support the specified frame rate, the configuration will throw an exception.
    /// </summary>
    public FPS CameraFps => cameraFps;

    /// <summary>
    /// The desired color image format.
    /// </summary>
    public ImageFormat ColorFormat => colorFormat;

    /// <summary>
    /// The desired color image resolution.
    /// </summary>
    public ColorResolution ColorResolution => colorResolution;

    /// <summary>
    /// The desired depth mode.
    /// </summary>
    public DepthMode DepthMode => depthMode;

    /// <summary>
    /// The desired synchronization mode when connecting two or more Kinect devices together.
    /// </summary>
    public WiredSyncMode WiredSyncMode => wiredSyncMode;

    /// <summary>
    /// Indicates whether Kinect will return only synchronized color and depth images.
    /// </summary>
    public bool SynchronizedImagesOnly => synchronizedImagesOnly;

    /// <summary>
    /// Specifies whether the streaming light indicator of the device is disabled.
    /// </summary>
    public bool DisableStreamingIndicator => disableStreamingIndicator;
}
