using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Azure_Kinect_ComputerVision : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private RawImage _image;
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _detectedObjectPrefab;

    private const string VisionApiKey = "8991690980884ab6b2a8188c542a77c7";
    private const string VisionEndpoint = "https://lightbuzzcomputervision.cognitiveservices.azure.com/";

    private readonly KinectSensor _kinect = new KinectSensor();

    private readonly ComputerVisionClient _azure = new ComputerVisionClient(new ApiKeyServiceClientCredentials(VisionApiKey))
    {
        Endpoint = VisionEndpoint
    };

    private readonly List<DetectedObjectRectangle> _rectangles = new List<DetectedObjectRectangle>();

    private Texture2D _texture;
    private bool _isProcessing = false;

    private void Start()
    {
        _kinect.Start(_configuration);

        _texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        _image.texture = _texture;
    }

    private void Update()
    {
        if (!_kinect.IsRunning) return;

        KinectData frameData = _kinect.Update();

        if (frameData != null)
        {
            byte[] colorData = frameData.Color;

            DetectObjects(colorData);

            _texture.LoadImage(colorData);
        }
    }

    private void OnDestroy()
    {
        _kinect.Stop();
        _azure.Dispose();

        foreach (DetectedObjectRectangle rectangle in _rectangles)
        {
            if (rectangle.isActiveAndEnabled)
            {
                Destroy(rectangle.gameObject);
            }
        }

        _rectangles.Clear();
    }

    private async void DetectObjects(byte[] colorData)
    {
        if (_isProcessing) return;
        if (!_kinect.IsRunning) return;
        if (colorData == null) return;

        try
        {
            _isProcessing = true;

            using (Stream stream = new MemoryStream(colorData))
            {
                DetectResult result = await _azure.DetectObjectsInStreamAsync(stream);

                if (result.Objects.Count != _rectangles.Count)
                {
                    foreach (DetectedObjectRectangle rectangle in _rectangles)
                    {
                        Destroy(rectangle.gameObject);
                    }

                    _rectangles.Clear();

                    foreach (DetectedObject item in result.Objects)
                    {
                        GameObject go = Instantiate(_detectedObjectPrefab, _canvas);
                        DetectedObjectRectangle rectangle = go.GetComponent<DetectedObjectRectangle>();

                        _rectangles.Add(rectangle);
                    }
                }

                for (int i = 0; i < result.Objects.Count; i++)
                {
                    DetectedObject item = result.Objects[i];
                    DetectedObjectRectangle rectangle = _rectangles[i];

                    BoundingRect rect = item.Rectangle;

                    int centerX = rect.X + rect.W / 2;
                    int centerY = rect.Y + rect.H / 2;

                    Vector2 center = new Vector2(centerX, centerY);
                    Vector2 size = new Vector2(rect.W, rect.H);
                    Vector3 position = _kinect.CoordinateMapper.MapColorToWorld(center);

                    Debug.Log(position);

                    rectangle.Load(item.ObjectProperty, center, size, position);
                }
            }

            _isProcessing = false;
        }
        catch
        {
            // Ignored.
        }
    }
}
