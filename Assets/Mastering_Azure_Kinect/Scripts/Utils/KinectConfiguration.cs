using System;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

/// <summary>
/// Exposes the configuration properties in the Unity Editor.
/// </summary>
[Serializable]
public class KinectConfiguration
{
    [SerializeField] private FPS _cameraFps = FPS.FPS30;
    [SerializeField] private ImageFormat _colorFormat = ImageFormat.ColorMJPG;
    [SerializeField] private ColorResolution _colorResolution = ColorResolution.R1080p;
    [SerializeField] private DepthMode _depthMode = DepthMode.NFOV_Unbinned;
    [SerializeField] private WiredSyncMode _wiredSyncMode = WiredSyncMode.Standalone;
    [SerializeField] private bool _synchronizedImagesOnly = true;
    [SerializeField] private bool _disableStreamingIndicator = false;

    /// <summary>
    /// The desired frame rate of the camera.
    /// ATTENTION: If the camera resolution does not support the specified frame rate, the configuration will throw an exception.
    /// </summary>
    public FPS CameraFps => _cameraFps;

    /// <summary>
    /// The desired color image format.
    /// </summary>
    public ImageFormat ColorFormat => _colorFormat;

    /// <summary>
    /// The desired color image resolution.
    /// </summary>
    public ColorResolution ColorResolution => _colorResolution;

    /// <summary>
    /// The desired depth mode.
    /// </summary>
    public DepthMode DepthMode => _depthMode;

    /// <summary>
    /// The desired synchronization mode when connecting two or more Kinect devices together.
    /// </summary>
    public WiredSyncMode WiredSyncMode => _wiredSyncMode;

    /// <summary>
    /// Indicates whether Kinect will return only synchronized color and depth images.
    /// </summary>
    public bool SynchronizedImagesOnly => _synchronizedImagesOnly;

    /// <summary>
    /// Specifies whether the streaming light indicator of the device is disabled.
    /// </summary>
    public bool DisableStreamingIndicator => _disableStreamingIndicator;
}
