﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

/// <summary>
/// Encapsulates the streaming functionality of a Kinect sensor.
/// </summary>
public class KinectSensor
{
    private Device _device;
    private Tracker _tracker;
    private KinectData _frameData;
    private volatile bool _isRunning;
    private readonly object _lock = new object();

    private DateTime _lastRequestedTimestamp;

    /// <summary>
    /// Specifies whether the Kinect sensor is currently running.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Returns the current Azure Kinect device.
    /// </summary>
    public Device Device => _device;

    /// <summary>
    /// The coordinate mapper.
    /// </summary>
    public CoordinateMapper CoordinateMapper { get; internal set; }

    /// <summary>
    /// Starts streaming.
    /// </summary>
    /// <param name="configuration">The Azure Kinect configuration.</param>
    public void Start(KinectConfiguration configuration = null)
    {
        _isRunning = false;

        if (Device.GetInstalledCount() <= 0)
        {
            Debug.LogWarning("No Kinect devices available.");
            return;
        }

        if (configuration == null) configuration = new KinectConfiguration();

        try
        {
            _device = Device.Open(configuration.DeviceIndex);

            if (_device == null)
            {
                Debug.LogError($"Kinect sensor {configuration.DeviceIndex} is not available.");
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

            Calibration calibration = _device.GetCalibration();

            CoordinateMapper = new CoordinateMapper(calibration);

            _tracker = Tracker.Create(calibration, new TrackerConfiguration
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

    /// <summary>
    /// Stops streaming.
    /// </summary>
    public void Stop()
    {
        _isRunning = false;

        try
        {
            CoordinateMapper?.Dispose();

            _tracker?.Shutdown();
            _tracker?.Dispose();

            _device?.StopCameras();
            _device?.StopImu();
            _device?.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Ignored - Tried to access a disposed object.
        }
    }

    /// <summary>
    /// Returns the latest Azure Kinect frame data.
    /// </summary>
    /// <returns>A collection of Azure Kinect data.</returns>
    public KinectData Update()
    {
        if (!_isRunning) return null;
        if (_frameData == null) return null;

        lock (_lock)
        {
            if (_frameData.Timestamp == _lastRequestedTimestamp) return null;

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
                        CoordinateMapper.Update(capture);

                        DateTime timestamp = DateTime.FromBinary(depth.SystemTimestampNsec);
                        byte[] colorData = MemoryMarshal.AsBytes(color.Memory.Span).ToArray();
                        ushort[] depthData = MemoryMarshal.Cast<byte, ushort>(depth.Memory.Span).ToArray();
                        BGRA[] colorToDepthData = null;
                        byte[] bodyIndexData = null;
                        List<Skeleton> bodyData = null;
                        ImuSample imuSample = _device.GetImuSample();

                        using (Image colorToDepthImage = CoordinateMapper.ColorToDepth)
                        {
                            colorToDepthData = colorToDepthImage.GetPixels<BGRA>().ToArray();
                        }

                        _tracker.EnqueueCapture(capture);

                        using (Frame bodyFrame = _tracker.PopResult(TimeSpan.Zero, false))
                        {
                            if (bodyFrame != null && bodyFrame.NumberOfBodies > 0)
                            {
                                bodyData = new List<Skeleton>();

                                for (uint i = 0; i < bodyFrame.NumberOfBodies; i++)
                                {
                                    Skeleton skeleton = bodyFrame.GetBodySkeleton(i);

                                    bodyData.Add(skeleton);
                                }

                                using (Image bodyIndex = bodyFrame.BodyIndexMap)
                                {
                                    bodyIndexData = MemoryMarshal.AsBytes(bodyIndex.Memory.Span).ToArray();
                                }
                            }
                        }

                        KinectData newData = new KinectData
                        {
                            Timestamp = timestamp,
                            Temperature = capture.Temperature,
                            Color = colorData,
                            Depth = depthData,
                            ColorToDepth = colorToDepthData,
                            BodyIndex = bodyIndexData,
                            Bodies = bodyData,
                            Imu = imuSample
                        };

                        lock (_lock)
                        {
                            KinectData temp = newData;
                            _frameData = temp;
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // Ignored - Tried to access a disposed object.
            }
        });
    }
}
