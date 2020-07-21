using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class KinectSensor
{
    private Device _device;
    private Tracker _tracker;
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

        if (configuration == null)
        {
            configuration = KinectConfiguration.Default;
            
            Debug.Log("No configuration was provided. Using the default configuration.");
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

            _tracker = Tracker.Create(_device.GetCalibration(), new TrackerConfiguration
            {
                ProcessingMode = configuration.TrackerProcessingMode,
                SensorOrientation = configuration.SensorOrientation
            });

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
            try
            {
                while (_isRunning)
                {
                    using (Capture capture = _device.GetCapture())
                    using (Image color = capture.Color)
                    using (Image depth = capture.Depth)
                    {
                        DateTime timestamp = DateTime.FromBinary(depth.SystemTimestampNsec);
                        byte[] colorData = MemoryMarshal.AsBytes(color.Memory.Span).ToArray();
                        ushort[] depthData = MemoryMarshal.Cast<byte, ushort>(depth.Memory.Span).ToArray();
                        byte[] bodyIndexData = null;
                        List<Body> bodyData = null;
                        ImuSample imuSample = _device.GetImuSample();

                        _tracker.EnqueueCapture(capture);

                        using (Frame bodyFrame = _tracker.PopResult(TimeSpan.Zero, false))
                        {
                            if (bodyFrame != null)
                            {
                                using (Image bodyIndex = bodyFrame.BodyIndexMap)
                                {
                                    bodyIndexData = MemoryMarshal.AsBytes(bodyIndex.Memory.Span).ToArray();
                                }

                                bodyData = Body.Create(bodyFrame);
                            }
                        }

                        FrameData newData = new FrameData
                        {
                            Timestamp = timestamp,
                            Temperature = capture.Temperature,
                            ColorData = colorData,
                            DepthData = depthData,
                            UserIndexData = bodyIndexData,
                            BodyData = bodyData,
                            ImuData = imuSample
                        };

                        lock (_lock)
                        {
                            FrameData temp = newData;
                            _frameData = temp;
                        }
                    }
                }
            }
            catch
            {
                // Tried to access disposed objects. Ignore.
            }
        });
    }
}
