using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.Azure.Kinect.Sensor;
using Vector3 = System.Numerics.Vector3;

/// <summary>
/// Provides transformations across the 2D and 3D space.
/// </summary>
public class CoordinateMapper
{
    private readonly Transformation _transformation;
    private Image _depthToColor;
    private ushort[] _data;

    /// <summary>
    /// The native device calibration.
    /// Source: https://microsoft.github.io/Azure-Kinect-Sensor-SDK/master/struct_microsoft_1_1_azure_1_1_kinect_1_1_sensor_1_1_calibration.html
    /// </summary>
    public Calibration Calibration { get; set; }

    internal CoordinateMapper(Calibration calibration)
    {
        Calibration = calibration;

        _transformation = Calibration.CreateTransformation();
    }

    internal void Update(Capture capture)
    {
        _depthToColor = _transformation.DepthImageToColorCamera(capture);
        _data = MemoryMarshal.Cast<byte, ushort>(_depthToColor.Memory.Span).ToArray();
    }

    internal Image DepthToColor(Capture capture)
    {
        return _transformation.DepthImageToColorCamera(capture);
    }

    internal Image DepthToPointCloud(Image depth)
    {
        return _transformation.DepthImageToPointCloud(depth);
    }

    internal Image ColorToDepth(Capture capture)
    {
        return _transformation.ColorImageToDepthCamera(capture);
    }

    internal void Release()
    {
        _depthToColor?.Dispose();
    }

    /// <summary>
    /// Maps the specified point from the 3D world space to the 2D color space.
    /// </summary>
    /// <param name="point3D">The 3D world point.</param>
    /// <returns>The corresponding 2D color point.</returns>
    public UnityEngine.Vector2 MapWorldToColor(UnityEngine.Vector3 point3D)
    {
        UnityEngine.Vector2 result = UnityEngine.Vector2.zero;

        try
        {
            Vector3 source3D = new Vector3(point3D.x, point3D.y, point3D.z);
            Vector2? source2D =
                Calibration.TransformTo2D(source3D, CalibrationDeviceType.Depth, CalibrationDeviceType.Color);

            if (source2D.HasValue)
            {
                result.Set(source2D.Value.X, source2D.Value.Y);
            }
        }
        catch
        {
            // Ignore - Color is OFF
        }

        return result;
    }

    /// <summary>
    /// Maps the specified point from the 3D world space to the 2D depth space.
    /// </summary>
    /// <param name="point3D">The 3D world point.</param>
    /// <returns>The corresponding 2D depth point.</returns>
    public UnityEngine.Vector2 MapWorldToDepth(UnityEngine.Vector3 point3D)
    {
        UnityEngine.Vector2 result = UnityEngine.Vector2.zero;

        try
        {
            Vector3 source3D = new Vector3(point3D.x, point3D.y, point3D.z);
            Vector2? source2D =
                Calibration.TransformTo2D(source3D, CalibrationDeviceType.Depth, CalibrationDeviceType.Depth);

            if (source2D.HasValue)
            {
                result.Set(source2D.Value.X, source2D.Value.Y);
            }
        }
        catch
        {
            // Ignore - Depth is OFF
        }

        return result;
    }

    /// <summary>
    /// Maps the specified point from the 2D color space to the 3D world space.
    /// </summary>
    /// <param name="point2D">The 2D color point.</param>
    /// <returns>The corresponding 3D world point.</returns>
    public UnityEngine.Vector3 MapColorToWorld(UnityEngine.Vector2 point2D)
    {
        int index = (int)point2D.y * _depthToColor.WidthPixels + (int)point2D.x;

        ushort depth = _data[index];

        UnityEngine.Vector3 result = UnityEngine.Vector3.zero;

        try
        {
            Vector2 source2D = new Vector2(point2D.x, point2D.y);
            Vector3? source3D = Calibration.TransformTo3D(source2D, (float)depth, CalibrationDeviceType.Color,
                CalibrationDeviceType.Depth);

            if (source3D.HasValue)
            {
                result.Set(source3D.Value.X / 1000.0f, source3D.Value.Y / 1000.0f, source3D.Value.Z / 1000.0f);
            }
        }
        catch
        {
            // Ignore
        }

        return result;
    }

    /// <summary>
    /// Maps the specified point from the 2D depth space to the 3D world space.
    /// </summary>
    /// <param name="point2D">The 2D depth point.</param>
    /// <param name="depth">The depth of the 2D point (in meters).</param>
    /// <returns>The corresponding 3D world point.</returns>
    public UnityEngine.Vector3 MapDepthToWorld(UnityEngine.Vector2 point2D, float depth)
    {
        UnityEngine.Vector3 result = UnityEngine.Vector3.zero;

        try
        {
            Vector2 source2D = new Vector2(point2D.x, point2D.y);
            Vector3? source3D = Calibration.TransformTo3D(source2D, depth, CalibrationDeviceType.Color,
                CalibrationDeviceType.Depth);

            if (source3D.HasValue)
            {
                result.Set(source3D.Value.X, source3D.Value.Y, source3D.Value.Z);
            }
        }
        catch
        {
            // Ignore
        }

        return result;
    }
}
