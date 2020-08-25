using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.Sensor;

/// <summary>
/// Encapsulates the Azure Kinect frame data.
/// </summary>
public class KinectData
{
    /// <summary>
    /// The timestamp of the current frame.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The current temperature of the sensor.
    /// </summary>
    public float Temperature { get; set; }

    /// <summary>
    /// The color data.
    /// </summary>
    public byte[] Color { get; set; }

    /// <summary>
    /// The depth data.
    /// </summary>
    public ushort[] Depth { get; set; }

    /// <summary>
    /// The body-index data.
    /// </summary>
    public byte[] BodyIndex { get; set; }

    /// <summary>
    /// The IMU (accelerometer and gyroscope) data.
    /// </summary>
    public ImuSample Imu { get; set; }

    /// <summary>
    /// The body data.
    /// </summary>
    public List<Body> Bodies { get; set; }
}
