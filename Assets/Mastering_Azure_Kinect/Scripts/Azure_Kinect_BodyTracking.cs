using System.Collections.Generic;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class Azure_Kinect_BodyTracking : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private GameObject _stickmanPrefab;

    private readonly KinectSensor _dataProvider = new KinectSensor();
    private List<Stickman> _stickmen = new List<Stickman>();

    private void Start()
    {
        _dataProvider.Start(_configuration);
    }

    private void Update()
    {
        if (!_dataProvider.IsRunning) return;

        KinectData frameData = _dataProvider.Update();

        if (frameData?.Bodies != null && frameData.Bodies.Count > 0)
        {
            UpdateStickmen(frameData.Bodies);
        }
    }

    private void OnDestroy()
    {
        _dataProvider.Stop();
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
                Stickman stickman = Instantiate(_stickmanPrefab).GetComponent<Stickman>();
                _stickmen.Add(stickman);
            }
        }

        for (int i = 0; i < bodies.Count; i++)
        {
            _stickmen[i].Load(bodies[i]);
        }
    }
}
