using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class Azure_Kinect_BodyTracking : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private Transform _head;

    private readonly KinectSensor _dataProvider = new KinectSensor();

    private void Start()
    {
        _dataProvider.Start(_configuration);
    }

    private void Update()
    {
        if (!_dataProvider.IsRunning) return;

        FrameData frameData = _dataProvider.Update();

        if (frameData?.BodyData != null && frameData?.BodyData.Count > 0)
        {
            var body = frameData.BodyData[0];
            var head = body.Joints[JointId.SpineChest];

            _head.position = head.Position;
            _head.rotation = head.Orientation;

            Debug.Log(_head.position);
        }
    }

    private void OnDestroy()
    {
        _dataProvider.Stop();
    }
}
