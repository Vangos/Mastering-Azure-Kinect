using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public class Azure_Kinect_IMU : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private Transform _cube;

    private readonly KinectSensor _dataProvider = new KinectSensor();

    private void Start()
    {
        _dataProvider.Start(_configuration);
    }

    private void Update()
    {
        if (!_dataProvider.IsRunning) return;

        FrameData frameData = _dataProvider.Update();

        if (frameData != null)
        {
            ImuSample imuSample = frameData.ImuData;

            Vector3 accelerometerSample = new Vector3
            (
                imuSample.AccelerometerSample.X,
                imuSample.AccelerometerSample.Y,
                imuSample.AccelerometerSample.Z
            );

            float pitch = Pitch(accelerometerSample);
            float yaw = Yaw(accelerometerSample);
            float roll = Roll(accelerometerSample);

            _cube.rotation = Quaternion.LookRotation(accelerometerSample);
        }
    }

    private void OnDestroy()
    {
        _dataProvider.Stop();
    }

    /// <summary>
    /// Returns the rotation of the camera around the X axis.
    /// Positive: the camera faces downwards.
    /// Negative: the camera faces upwards.
    /// Zero: the camera is straight.
    /// </summary>
    /// <param name="sample">The accelerometer sample.</param>
    /// <returns>The Pitch rotation angle.</returns>
    public float Pitch(Vector3 sample)
    {
        float value = Mathf.Atan2(sample.x, Mathf.Sqrt(sample.y * sample.y + sample.z * sample.z));

        return value;
    }

    public float Yaw(Vector3 sample)
    {
        float value = Mathf.Atan2(sample.z, Mathf.Sqrt(sample.x * sample.x + sample.z * sample.z));

        return value;
    }

    public float Roll(Vector3 sample)
    {
        float value = Mathf.Atan2(sample.y, Mathf.Sqrt(sample.x * sample.x + sample.z * sample.z));

        return value;
    }
}
