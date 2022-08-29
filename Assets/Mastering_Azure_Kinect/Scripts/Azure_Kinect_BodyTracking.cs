using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class Azure_Kinect_BodyTracking : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;

    private readonly KinectSensor _kinect = new KinectSensor();

    private readonly SpeedMeasurement _speedMeasurement = new SpeedMeasurement();

    private void Start()
    {
        _kinect.Start(_configuration);
    }

    private void Update()
    {
        if (!_kinect.IsRunning) return;

        KinectData frame = _kinect.Update();

        if (frame != null && frame.Bodies.Count > 0)
        {
            Skeleton body = frame.Bodies[0];

            _speedMeasurement.Check(body);
        }
    }

    private void OnDestroy()
    {
        _kinect.Stop();
    }

    // These should be event handlers for two Unity buttons.
    public void OnTimerStart()
    {
        _speedMeasurement.Start();
    }

    public void OnTimerStop()
    {
        _speedMeasurement.Stop();
    }
}

public class SpeedMeasurement
{
    private float _distance = 0.0f;
    private Vector3 _previous = Vector3.Zero;

    private DateTime _startDate;
    private DateTime _endDate;

    public void Start()
    {
        _startDate = DateTime.Now;
    }

    public void Stop()
    {
        _endDate = DateTime.Now;

        float time = (float)(_endDate - _startDate).TotalSeconds;

        if (time > 0)
        {
            float speed = _distance / time;

            Debug.Log($"Speed is {speed} meters per second");
        }
    }

    public void Check(Skeleton skeleton)
    {
        Vector3 position = skeleton.GetJoint(JointId.Pelvis).Position;

        _distance += Distance(position, _previous);

        _previous = position;
    }

    public float Distance(Vector3 point1, Vector3 point2)
    {
        return Mathf.Sqrt(
            (point2.X - point1.X) * (point2.X - point1.X) +
            (point2.Y - point1.Y) *(point2.Y - point1.Y) +
            (point2.Z - point1.Z) *(point2.Z - point1.Z));
    }

}

public class RepCounter
{
    public const float Threshold = 70.0f;

    private int _counter = 0;

    private float _min = float.MaxValue;
    private float _previous = float.NaN;

    public void Check(Skeleton body)
    {
        Vector3 elbow = 
            body.GetJoint(JointId.ElbowLeft).Position;
        Vector3 shoulder = 
            body.GetJoint(JointId.ShoulderLeft).Position;
        Vector3 wrist = 
            body.GetJoint(JointId.WristLeft).Position;

        float angle = Angle(elbow, shoulder, wrist);

        if (angle < _min)
        {
            _min = angle;
        }

        if (angle < Threshold)
        {
            if (_previous < angle && _previous == _min)
            {
                _counter++;

                Debug.Log($"Count: {_counter}");

                _min = float.MaxValue; // Reset
            }
        }

        _previous = angle;
    }

    public float Angle(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Vector3 a = new Vector3(
            point1.X - point2.X,
            point1.Y - point2.Y,
            point1.Z - point2.Z);

        Vector3 b = new Vector3(
            point1.X - point3.X,
            point1.Y - point3.Y,
            point1.Z - point3.Z);

        float dot = a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        float lengthA =
            Mathf.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
        float lengthB =
            Mathf.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z);

        if (lengthA == 0.0f || lengthB == 0.0f)
        {
            return 0.0f;
        }

        float theta = Mathf.Acos(dot / lengthA * lengthB);

        theta *= 180.0f / Mathf.PI; // Radians to degrees

        return theta;
    }
}
