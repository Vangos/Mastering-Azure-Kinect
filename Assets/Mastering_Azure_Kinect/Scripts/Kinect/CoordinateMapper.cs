using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;

/// <summary>
/// Provides transformations across the 2D and 3D space.
/// </summary>
public class CoordinateMapper : IDisposable
{
    private readonly Transformation _transformation;
    private Calibration _calibration;
    private Capture _capture;

    public CoordinateMapper(Calibration calibration)
    {
        _calibration = calibration;
        _transformation = _calibration.CreateTransformation();
    }

    public void Update(Capture capture)
    {
        _capture = capture;
    }

    public Image ColorToDepth =>
        _transformation.ColorImageToDepthCamera(_capture);

    public Image DepthToColor =>
        _transformation.DepthImageToColorCamera(_capture);

    public void Dispose()
    {
        ColorToDepth?.Dispose();
        DepthToColor?.Dispose();
    }

    /// <summary>
    /// Maps the specified point from the 3D world space to the 2D color space.
    /// </summary>
    /// <param name="point3D">The 3D world point.</param>
    /// <returns>The corresponding 2D color point.</returns>
    public Vector2 MapWorldToColor(Vector3 point3D)
    {
        Vector2 point2D = Vector2.zero;

        try
        {
            var input = new System.Numerics.Vector3(point3D.x, point3D.y, point3D.z);
            var output = _calibration.TransformTo2D(input, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);

            if (output.HasValue)
            {
                point2D.Set(output.Value.X, output.Value.Y);
            }
        }
        catch
        {
            // Ignore - Color is turned off
        }

        return point2D;
    }

    /// <summary>
    /// Maps the specified point from the 3D world space to the 2D depth space.
    /// </summary>
    /// <param name="point3D">The 3D world point.</param>
    /// <returns>The corresponding 2D depth point.</returns>
    public Vector2 MapWorldToDepth(Vector3 point3D)
    {
        Vector2 point2D = Vector2.zero;

        try
        {
            var input = new System.Numerics.Vector3(point3D.x, point3D.y, point3D.z);
            var output = _calibration.TransformTo2D(input, CalibrationDeviceType.Depth, CalibrationDeviceType.Depth);

            if (output.HasValue)
            {
                point2D.Set(output.Value.X, output.Value.Y);
            }
        }
        catch
        {
            // Ignore - Depth is turned off
        }

        return point2D;
    }

    /// <summary>
    /// Maps the specified point from the 2D color space to the 3D world space.
    /// </summary>
    /// <param name="point2D">The 2D color point.</param>
    /// <returns>The corresponding 3D world point.</returns>
    public Vector3 MapColorToWorld(Vector2 point2D)
    {
        ushort[] depthData = MemoryMarshal.Cast<byte, ushort>(DepthToColor.Memory.Span).ToArray();

        int index = (int)point2D.y * DepthToColor.WidthPixels + (int)point2D.x;
        ushort depth = depthData[index];

        Vector3 point3D = Vector3.zero;

        try
        {
            var input = new System.Numerics.Vector2(point2D.x, point2D.y);
            var output = _calibration.TransformTo3D(input, depth, CalibrationDeviceType.Color, CalibrationDeviceType.Depth);

            if (output.HasValue)
            {
                point3D.Set(output.Value.X, output.Value.Y, output.Value.Z);
            }
        }
        catch
        {
            // Ignore - Color is turned off
        }

        return point3D;
    }

    /// <summary>
    /// Maps the specified point from the 2D depth space to the 3D world space.
    /// </summary>
    /// <param name="point2D">The 2D depth point.</param>
    /// <param name="depth">The depth of the 2D point (in meters).</param>
    /// <returns>The corresponding 3D world point.</returns>
    public Vector3 MapDepthToWorld(Vector2 point2D, float depth)
    {
        Vector3 point3D = Vector3.zero;

        try
        {
            var input = new System.Numerics.Vector2(point2D.x, point2D.y);
            var output = _calibration.TransformTo3D(input, depth, CalibrationDeviceType.Color,
                CalibrationDeviceType.Depth);

            if (output.HasValue)
            {
                point3D.Set(output.Value.X, output.Value.Y, output.Value.Z);
            }
        }
        catch
        {
            // Ignore - Depth is turned off
        }

        return point3D;
    }

    public Vector2 MapColorToDepth(Vector2 pointColor)
    {
        ushort[] depthData = MemoryMarshal.Cast<byte, ushort>(DepthToColor.Memory.Span).ToArray();

        int index = (int)pointColor.y * DepthToColor.WidthPixels + (int)pointColor.x;
        ushort depth = depthData[index];

        Vector2 pointDepth = Vector2.zero;

        try
        {
            var input = new System.Numerics.Vector2(pointColor.x, pointColor.y);
            var output = _calibration.TransformTo2D(input, depth, CalibrationDeviceType.Color, CalibrationDeviceType.Depth);

            if (output.HasValue)
            {
                pointDepth.Set(output.Value.X, output.Value.Y);
            }
        }
        catch
        {
            // Ignore - Color or Depth is turned off
        }

        return pointDepth;
    }

    public Vector2 MapDepthToColor(Vector2 pointDepth, float depth)
    {
        Vector2 pointColor = Vector2.zero;

        try
        {
            var input = new System.Numerics.Vector2(pointDepth.x, pointDepth.y);
            var output = _calibration.TransformTo2D(input, depth, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);

            if (output.HasValue)
            {
                pointColor.Set(output.Value.X, output.Value.Y);
            }
        }
        catch
        {
            // Ignore - Color or Depth is turned off
        }

        return pointColor;
    }
}
