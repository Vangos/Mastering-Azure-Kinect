using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class Joint : MonoBehaviour
{
    public JointId Type { get; set; }

    public JointConfidenceLevel Confidence { get; set; }

    public Vector3 Position { get; set; }

    public Quaternion Orientation { get; set; }
}
