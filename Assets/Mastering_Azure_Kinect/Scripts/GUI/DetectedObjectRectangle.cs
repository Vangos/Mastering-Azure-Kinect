using UnityEngine;
using UnityEngine.UI;

public class DetectedObjectRectangle : MonoBehaviour
{
    [SerializeField] private Image _border;
    [SerializeField] private Text _textType;
    [SerializeField] private Text _textDistance;

    public void Load(string type, Vector2 center, Vector2 size, Vector3 position)
    {
        _textType.text = type;
        _textDistance.text = position.z.ToString("N2") + "m";

        _border.rectTransform.anchoredPosition = new Vector2(center.x, -center.y);
        _border.rectTransform.sizeDelta = size;
    }
}
