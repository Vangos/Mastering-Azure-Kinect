using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.Sensor;

public class FrameData
{
    public DateTime Timestamp { get; set; }

    public float Temperature { get; set; }

    public byte[] ColorData { get; set; }

    public ushort[] DepthData { get; set; }

    public byte[] UserIndexData { get; set; }

    public ImuSample ImuData { get; set; }

    public List<Body> BodyData { get; set; }
}
