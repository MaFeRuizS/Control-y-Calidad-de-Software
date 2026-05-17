using UnityEngine;
using UnityEngine.UI;

public class ToggleVisual : MonoBehaviour
{
    public Toggle toggle;
    public Image background;
    public Sprite onSprite;
    public Sprite offSprite;

    void Start()
    {
        UpdateVisual();
        toggle.onValueChanged.AddListener(delegate { UpdateVisual(); });
    }

    void UpdateVisual()
    {
        background.sprite = toggle.isOn ? onSprite : offSprite;
    }
}