using UnityEngine;
using UnityEngine.UI;

public class Azure_Kinect_Color : MonoBehaviour
{
    [SerializeField] private KinectConfiguration _configuration;
    [SerializeField] private RawImage _image;

    private Texture2D _texture;

    private readonly KinectSensor _dataProvider = new KinectSensor();

    private void Start()
    {
        _dataProvider.Start(_configuration);

        _texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        _image.texture = _texture;
    }

    private void Update()
    {
        if (!_dataProvider.IsRunning) return;

        FrameData frameData = _dataProvider.Update();

        if (frameData != null)
        {
            byte[] colorData = frameData.ColorData;

            _texture.LoadImage(colorData);
        }
    }

    private void OnDestroy()
    {
        _dataProvider.Stop();
    }
}
