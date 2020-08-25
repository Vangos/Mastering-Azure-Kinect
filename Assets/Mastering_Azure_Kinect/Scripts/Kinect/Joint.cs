using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

/// <summary>
/// Represents a human body joint.
/// </summary>
public class Joint : MonoBehaviour
{
    /// <summary>
    /// The unique joint type.
    /// </summary>
    public JointId Type { get; set; }

    /// <summary>
    /// The tracking confidence of the joint.
    /// </summary>
    public JointConfidenceLevel Confidence { get; set; }

    /// <summary>
    /// The position of the joint in the 3D space.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The orientation of the joint in the 3D space.
    /// </summary>
    public Quaternion Orientation { get; set; }
}
