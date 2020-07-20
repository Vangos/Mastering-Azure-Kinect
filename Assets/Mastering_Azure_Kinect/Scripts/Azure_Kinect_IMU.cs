using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.UI;

public class Azure_Kinect_IMU : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private Transform cube;

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
        }
    }

    private void OnDestroy()
    {
        _dataProvider.Stop();
    }
}
