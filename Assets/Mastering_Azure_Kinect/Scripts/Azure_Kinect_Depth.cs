﻿using UnityEngine;
using UnityEngine.UI;

public class Azure_Kinect_Depth : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private RawImage _image;
    [SerializeField] private DepthVisualization _visualization;
    [SerializeField] [Range(0, 10000)] private ushort _maxDepth = 6000;

    private Texture2D _texture;
    
    private readonly KinectSensor _dataProvider = new KinectSensor();

    private void Start()
    {
        _dataProvider.Start(_configuration);

        if (_dataProvider.IsRunning)
        {
            int depthWidth = _dataProvider.Device.GetCalibration().DepthCameraCalibration.ResolutionWidth;
            int depthHeight = _dataProvider.Device.GetCalibration().DepthCameraCalibration.ResolutionHeight;

            _texture = new Texture2D(depthWidth, depthHeight, TextureFormat.RGB24, false);
            _image.texture = _texture;
        }
    }

    private void Update()
    {
        if (!_dataProvider.IsRunning) return;

        KinectData frameData = _dataProvider.Update();

        if (frameData != null)
        {
            ushort[] depthData = frameData.Depth;

            byte[] pixels =
                _visualization == DepthVisualization.Gray
                    ? Gray(depthData)
                    : Jet(depthData);

            _texture.LoadRawTextureData(pixels);
            _texture.Apply();
        }
    }

    private void OnDestroy()
    {
        _dataProvider.Stop();
    }

    private byte[] Gray(ushort[] data)
    {
        const int channels = 3; // RGB has 3 channels.
        const byte maxByte = byte.MaxValue; // 255

        byte[] pixels = new byte[data.Length * channels];

        for (int i = 0; i < data.Length; i++)
        {
            ushort depth = data[i];

            if (depth > 0)
            {
                byte gray = (byte)((float)depth / _maxDepth * maxByte);

                pixels[i * channels + 0] = gray;
                pixels[i * channels + 1] = gray;
                pixels[i * channels + 2] = gray;
            }
        }

        return pixels;
    }

    private byte[] Jet(ushort[] data)
    {
        const int channels = 3; // RGB has 3 channels.
        const byte maxByte = byte.MaxValue; // 255

        byte[] pixels = new byte[data.Length * channels];

        float min = -1.0f;
        float max = 1.0f;

        for (int i = 0; i < data.Length; i++)
        {
            ushort depth = data[i];

            if (depth > 0)
            {
                float t = depth * (max - min) / _maxDepth + min;

                float red = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t - max)) * maxByte;
                float green = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t)) * maxByte;
                float blue = Mathf.Clamp01(1.5f - Mathf.Abs(2.0f * t + max)) * maxByte;

                pixels[i * channels + 0] = (byte)red;
                pixels[i * channels + 1] = (byte)green;
                pixels[i * channels + 2] = (byte)blue;
            }
        }

        return pixels;
    }
}

enum DepthVisualization
{
    Gray,
    Jet
}
