using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class Body
{
    public uint ID { get; set; }

    public Dictionary<JointId, Joint> Joints { get; set; }

    public Body()
    {
        Joints = new Dictionary<JointId, Joint>();
    }

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
