using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.Sensor;

public class KinectData
{
    public DateTime Timestamp { get; set; }

    public float Temperature { get; set; }

    public byte[] Color { get; set; }

    public ushort[] Depth { get; set; }

    public byte[] BodyIndex { get; set; }

    public ImuSample Imu { get; set; }

    public List<Body> Bodies { get; set; }
}
