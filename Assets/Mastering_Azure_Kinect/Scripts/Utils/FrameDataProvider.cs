using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class FrameDataProvider
{
    private Device _device;
    private Tracker _tracker;
    private Calibration _calibration;
    private FrameData _frameData;
    private volatile bool _isRunning;
    private readonly object _lock = new object();

    private DateTime _lastRequestedTimestamp;

    public bool IsRunning => _isRunning;
    public Device Device => _device;

    public void Start(KinectConfiguration configuration = null, int deviceIndex = 0)
    {
        _isRunning = false;

        if (Device.GetInstalledCount() <= 0)
        {
            Debug.LogWarning("No Kinect devices available.");
            return;
        }

        try
        {
            _device = Device.Open(deviceIndex);

            if (_device == null)
            {
                Debug.LogError($"Kinect sensor {deviceIndex} is not available.");
                return;
            }

            _device.StartCameras(new DeviceConfiguration
            {
                CameraFPS = configuration.CameraFps,
                ColorFormat = configuration.ColorFormat,
                ColorResolution = configuration.ColorResolution,
                DepthMode = configuration.DepthMode,
                DisableStreamingIndicator = configuration.DisableStreamingIndicator,
                WiredSyncMode = configuration.WiredSyncMode,
                SynchronizedImagesOnly = configuration.SynchronizedImagesOnly
            });
            _device.StartImu();

            _calibration = _device.GetCalibration();

            _tracker = Tracker.Create(_calibration, TrackerConfiguration.Default);

            _isRunning = true;

            Stream();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void Stop()
    {
        _isRunning = false;

        _tracker?.Shutdown();
        _tracker?.Dispose();

        _device?.StopCameras();
        _device?.StopImu();
        _device?.Dispose();
    }

    public FrameData Update()
    {
        if (!_isRunning) return null;
        if (_frameData == null) return null;

        lock (_lock)
        {
            if (_frameData.Timestamp == _lastRequestedTimestamp)
            {
                return null;
            }

            _lastRequestedTimestamp = _frameData.Timestamp;

            return _frameData;
        }
    }

    private void Stream()
    {
        Task.Run(() =>
        {
            while (_isRunning)
            {
                using (Capture capture = _device.GetCapture())
                using (Image color = capture.Color)
                using (Image depth = capture.Depth)
                {

                    //Tracker.EnqueueCapture(capture);

                    //using (var f = Tracker.PopResult(TimeSpan.Zero, false))
                    //{
                    //}

                    ImuSample imuSample = _device.GetImuSample();

                    FrameData newData = new FrameData
                    {
                        Timestamp = DateTime.FromBinary(depth.SystemTimestampNsec),
                        Temperature = capture.Temperature,
                        ColorData = MemoryMarshal.AsBytes(color.Memory.Span).ToArray(),
                        DepthData = MemoryMarshal.Cast<byte, ushort>(depth.Memory.Span).ToArray(),
                        ImuData = imuSample
                    };

                    lock (_lock)
                    {
                        FrameData temp = newData;
                        _frameData = temp;
                    }
                }
            }
        });
    }
}
