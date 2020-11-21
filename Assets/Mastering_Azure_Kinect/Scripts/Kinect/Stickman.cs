using System;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class Stickman : MonoBehaviour
{
    [SerializeField] private Transform[] _cubes = new Transform[(int)JointId.Count];
    [SerializeField] private LineRenderer[] _lines = new LineRenderer[(int)JointId.Count];

    private readonly Tuple<JointId, JointId>[] _bones = 
    {
        Tuple.Create(JointId.EarLeft, JointId.EyeLeft),
        Tuple.Create(JointId.EyeLeft, JointId.Nose),
        Tuple.Create(JointId.Nose, JointId.EyeRight),
        Tuple.Create(JointId.EyeRight, JointId.EarRight),
        Tuple.Create(JointId.Nose, JointId.Head),
        Tuple.Create(JointId.Head, JointId.Neck),
        Tuple.Create(JointId.Neck, JointId.SpineChest),
        Tuple.Create(JointId.Neck, JointId.ClavicleLeft),
        Tuple.Create(JointId.Neck, JointId.ClavicleRight),
        Tuple.Create(JointId.ClavicleLeft, JointId.ShoulderLeft),
        Tuple.Create(JointId.ClavicleRight, JointId.ShoulderRight),
        Tuple.Create(JointId.ClavicleRight, JointId.ShoulderRight),
        Tuple.Create(JointId.ShoulderLeft, JointId.ElbowLeft),
        Tuple.Create(JointId.ShoulderRight, JointId.ElbowRight),
        Tuple.Create(JointId.ElbowLeft, JointId.WristLeft),
        Tuple.Create(JointId.ElbowRight, JointId.WristRight),
        Tuple.Create(JointId.WristLeft, JointId.HandLeft),
        Tuple.Create(JointId.WristRight, JointId.HandRight),
        Tuple.Create(JointId.HandLeft, JointId.HandTipLeft),
        Tuple.Create(JointId.HandRight, JointId.HandTipRight),
        Tuple.Create(JointId.HandLeft, JointId.ThumbLeft),
        Tuple.Create(JointId.HandRight, JointId.ThumbRight),
        Tuple.Create(JointId.SpineChest, JointId.SpineNavel),
        Tuple.Create(JointId.SpineNavel, JointId.Pelvis),
        Tuple.Create(JointId.Pelvis, JointId.HipLeft),
        Tuple.Create(JointId.Pelvis, JointId.HipRight),
        Tuple.Create(JointId.HipLeft, JointId.KneeLeft),
        Tuple.Create(JointId.HipRight, JointId.KneeRight),
        Tuple.Create(JointId.KneeLeft, JointId.AnkleLeft),
        Tuple.Create(JointId.KneeRight, JointId.AnkleRight),
        Tuple.Create(JointId.AnkleLeft, JointId.FootLeft),
        Tuple.Create(JointId.AnkleRight, JointId.FootRight),
    };

    /// <summary>
    /// Loads the specified body object to the current stickman.
    /// </summary>
    /// <param name="body">The body object to load.</param>
    public void Load(Skeleton body)
    {
        for (int i = 0; i < _cubes.Length; i++)
        {
            var joint = body.GetJoint((JointId) i);

            Vector3 position = new Vector3(joint.Position.X / 1000.0f, -joint.Position.Y / 1000.0f, joint.Position.Z / 1000.0f);
            Quaternion orientation = new Quaternion(joint.Quaternion.X, joint.Quaternion.Y, joint.Quaternion.Z, joint.Quaternion.W);

            _cubes[i].position = position;
            _cubes[i].rotation = orientation;
        }

        for (int i = 0; i < _bones.Length; i++)
        {
            var joint1 = body.GetJoint(_bones[i].Item1);
            var joint2 = body.GetJoint(_bones[i].Item2);

            Vector3 position1 = new Vector3(joint1.Position.X / 1000.0f, -joint1.Position.Y / 1000.0f, joint1.Position.Z / 1000.0f);
            Vector3 position2 = new Vector3(joint2.Position.X / 1000.0f, -joint2.Position.Y / 1000.0f, joint2.Position.Z / 1000.0f);

            _lines[i].SetPosition(0, position1);
            _lines[i].SetPosition(1, position2);
        }
    }
}
