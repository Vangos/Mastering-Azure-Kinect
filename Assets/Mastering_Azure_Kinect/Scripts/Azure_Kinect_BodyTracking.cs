using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class Azure_Kinect_BodyTracking : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;

    private readonly List<Stickman> _stickmen = new List<Stickman>();
    private readonly KinectSensor _kinect = new KinectSensor();

    private void Start()
    {
        _kinect.Start(_configuration);
    }

    private void Update()
    {
        if (!_kinect.IsRunning) return;

        KinectData frameData = _kinect.Update();
    }

    private void OnDestroy()
    {
        _kinect.Stop();
    }

    private void UpdateStickmen(List<Skeleton> skeletons)
    {
        if (skeletons == null) return;

        if (_stickmen.Count != skeletons.Count)
        {
            foreach (Stickman stickman in _stickmen)
            {
                Destroy(stickman.gameObject);
            }

            _stickmen.Clear();

            foreach (Skeleton body in skeletons)
            {
                Stickman stickman = (Instantiate(Resources.Load("Stickman")) as GameObject)?.GetComponent<Stickman>();
                _stickmen.Add(stickman);
            }
        }

        for (int i = 0; i < skeletons.Count; i++)
        {
            _stickmen[i].Load(skeletons[i]);
        }
    }
}
