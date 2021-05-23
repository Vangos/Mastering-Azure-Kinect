using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using UnityEngine.UI;

public class Azure_Kinect_BackgroundRemoval : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private RawImage _image;

    private Texture2D _texture;
    private Color32[] _colors;

    private readonly KinectSensor _kinect = new KinectSensor();

    private void Start()
    {
        _kinect.Start(_configuration);

        int depthWidth = 640;
        int depthHeight = 576;

        _texture = new Texture2D(depthWidth, depthHeight, TextureFormat.BGRA32, false);
        _colors = new Color32[depthWidth * depthHeight];
        _image.texture = _texture;
    }

    private void Update()
    {
        if (!_kinect.IsRunning) return;

        KinectData frameData = _kinect.Update();

        if (frameData != null)
        {
            BGRA[] colorToDepth = frameData.ColorToDepth;
            byte[] bodyIndex = frameData.BodyIndex;

            if (colorToDepth != null && bodyIndex != null)
            {
                for (int i = 0; i < bodyIndex.Length; i++)
                {
                    if (bodyIndex[i] != Frame.BodyIndexMapBackground) // 255
                    {
                        // This pixel belongs to a human
                        // Use the colors of the human body
                        _colors[i].b = colorToDepth[i].B;
                        _colors[i].g = colorToDepth[i].G;
                        _colors[i].r = colorToDepth[i].R;
                        _colors[i].a = colorToDepth[i].A;
                    }
                    else
                    {
                        // This pixel belongs to the background
                        _colors[i] = Color.clear;
                    }
                }

                _texture.SetPixels32(_colors);
                _texture.Apply();
            }
        }
    }

    private void OnDestroy()
    {
        _kinect.Stop();
    }
}
