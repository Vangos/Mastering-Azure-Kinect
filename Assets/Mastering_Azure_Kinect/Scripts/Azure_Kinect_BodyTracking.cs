using System.Collections.Generic;
using UnityEngine;

public class Azure_Kinect_BodyTracking : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;

    private readonly KinectSensor _kinect = new KinectSensor();
    private List<Stickman> _stickmen = new List<Stickman>();

    private void Start()
    {
        _kinect.Start(_configuration);
    }

    private void Update()
    {
        if (!_kinect.IsRunning) return;

        KinectData frameData = _kinect.Update();

        if (frameData?.Bodies != null && frameData.Bodies.Count > 0)
        {
            UpdateStickmen(frameData.Bodies);
        }
    }

    private void OnDestroy()
    {
        _kinect.Stop();
    }

    private void UpdateStickmen(List<Body> bodies)
    {
        if (bodies == null)
        {
            return;
        }

        if (_stickmen == null)
        {
            _stickmen = new List<Stickman>();
        }

        if (_stickmen.Count != bodies.Count)
        {
            foreach (Stickman stickman in _stickmen)
            {
                Destroy(stickman.gameObject);
            }

            _stickmen.Clear();

            foreach (Body body in bodies)
            {
                Stickman stickman = (Instantiate(Resources.Load("Stickman")) as GameObject)?.GetComponent<Stickman>();
                _stickmen.Add(stickman);
            }
        }

        for (int i = 0; i < bodies.Count; i++)
        {
            _stickmen[i].Load(bodies[i]);
        }
    }
}
