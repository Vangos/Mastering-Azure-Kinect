using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Azure_Kinect_ComputerVision : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private RawImage _image;

    private const string _visionApiKey = "8991690980884ab6b2a8188c542a77c7";
    private const string _visionEndpoint = "https://lightbuzzcomputervision.cognitiveservices.azure.com/";

    private readonly KinectSensor _kinect = new KinectSensor();
    private readonly ComputerVisionClient _azure = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_visionApiKey))
    {
        Endpoint = _visionEndpoint
    };

    private Texture2D _texture;
    private bool _isRunning = false;

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
            ushort[] depthData = frameData.Depth;

            DetectObjects(colorData, depthData);

            _texture.LoadImage(colorData);
        }
    }

    private void OnDestroy()
    {
        _kinect.Stop();
    }

    private async void DetectObjects(byte[] colorData, ushort[] depthData)
    {
        if (_isRunning) return;
        if (colorData == null || depthData == null) return;

        try
        {
            _isRunning = true;

            using (Stream stream = new MemoryStream(colorData))
            {
                DetectResult result = await _azure.DetectObjectsInStreamAsync(stream);
            }

            _isRunning = false;
        }
        catch
        {
            // Ignored.
        }
    }
}
