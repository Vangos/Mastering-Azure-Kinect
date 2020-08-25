using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

/// <summary>
/// Represents a human body.
/// </summary>
public class Body
{
    /// <summary>
    /// The unique identifier of the body.
    /// </summary>
    public uint ID { get; set; }

    /// <summary>
    /// The human body joints.
    /// </summary>
    public Dictionary<JointId, Joint> Joints { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="Body"/>.
    /// </summary>
    public Body()
    {
        Joints = new Dictionary<JointId, Joint>();
    }

    /// <summary>
    /// Creates a list of body objects.
    /// </summary>
    /// <param name="bodyFrame">The Azure Kinect body frame to use.</param>
    /// <returns>A list of tracked bodies.</returns>
    public static List<Body> Create(Frame bodyFrame)
    {
        List<Body> bodies = new List<Body>();

        for (uint i = 0; i < bodyFrame.NumberOfBodies; i++)
        {
            Body body = new Body
            {
                ID = bodyFrame.GetBodyId(i)
            };

            Skeleton skeleton = bodyFrame.GetBodySkeleton(i);

            foreach (JointId jointId in Enum.GetValues(typeof(JointId)))
            {
                if (jointId == JointId.Count) continue;

                var joint = skeleton.GetJoint(jointId);

                body.Joints.Add(jointId, new Joint
                {
                    Type = jointId,
                    Confidence = joint.ConfidenceLevel,
                    Position = new Vector3
                    (
                        joint.Position.X / 1000.0f, 
                        joint.Position.Y / 1000.0f, 
                        joint.Position.Z / 1000.0f
                    ),
                    Orientation = new Quaternion
                    (
                        joint.Quaternion.X, 
                        joint.Quaternion.Y, 
                        joint.Quaternion.Z, 
                        joint.Quaternion.W
                    )
                });

            }
            
            bodies.Add(body);
        }

        return bodies;
    }
}
