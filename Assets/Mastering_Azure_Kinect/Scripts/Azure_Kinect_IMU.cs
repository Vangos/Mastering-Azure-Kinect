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

        KinectData frameData = _dataProvider.Update();

        if (frameData != null)
        {
            ImuSample imuSample = frameData.Imu;

            Vector3 accelerometerSample = new Vector3
            (
                imuSample.AccelerometerSample.X,
                imuSample.AccelerometerSample.Y,
                imuSample.AccelerometerSample.Z
            );

            Vector3 gyroscopeSample = new Vector3
            (
                imuSample.GyroSample.X,
                imuSample.GyroSample.Y,
                imuSample.GyroSample.Z
            );

            float pitch = Pitch(accelerometerSample);
            float yaw = Yaw(accelerometerSample); // Accelerometer cannot measure Yaw
            float roll = Roll(accelerometerSample);

            //_cube.rotation = Quaternion.Euler(new Vector3(pitch, yaw, roll));
            _cube.rotation = Quaternion.Euler(Gyros(gyroscopeSample));
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
    /// <param name="accelerometer">The accelerometer sample.</param>
    /// <returns>The Pitch rotation angle.</returns>
    public float Pitch(Vector3 accelerometer)
    {
        float value = Mathf.Atan2(-accelerometer.x, Mathf.Sqrt(accelerometer.y * accelerometer.y + accelerometer.z * accelerometer.z));

        return value * 180.0f / Mathf.PI;
    }
    
    public float Yaw(Vector3 accelerometer)
    {
        float value = Mathf.Atan2(accelerometer.z, Mathf.Sqrt(accelerometer.x * accelerometer.x + accelerometer.z * accelerometer.z));

        return value * 180.0f / Mathf.PI;
    }

    public float Roll(Vector3 accelerometer)
    {
        float value = Mathf.Atan2(accelerometer.y, Mathf.Sqrt(accelerometer.x * accelerometer.x + accelerometer.z * accelerometer.z));

        return value * 180.0f / Mathf.PI;
    }

    private float _initialPitch = 0.0f;
    private float _initialYaw = 0.0f;
    private float _initialRoll = 0.0f;

    public Vector3 Gyros(Vector3 gyro)
    {
        if (gyro.x > Mathf.Abs(0.01f))
        {
            _initialRoll += gyro.x * Time.deltaTime;
        }

        if (gyro.y > Mathf.Abs(0.01f))
        {
            _initialPitch += gyro.y * Time.deltaTime;
        }

        if (gyro.z > Mathf.Abs(0.01f))
        {
            _initialYaw += gyro.z * Time.deltaTime;
        }

        return new Vector3(_initialPitch * 180.0f / Mathf.PI, _initialYaw * 180.0f / Mathf.PI, _initialRoll * 180.0f / Mathf.PI);
    }
}
