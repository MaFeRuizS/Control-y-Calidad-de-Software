using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    [Header("Settings")]
    public int categoryValue; // El número que representa esta categoría (4, 5, 6, etc.)
    
    [Header("References")]
    public TextMeshProUGUI valueText;
    public Image buttonImage;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.green;
    public Color hoverColor = Color.yellow;
    public Color pressedColor = Color.white;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        if (valueText != null)
        {
            valueText.text = categoryValue.ToString();
        }

        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    void OnButtonClick()
    {
        // Enviar la selección al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ClassifyElement(categoryValue);
        }

        StartCoroutine(ButtonPressAnimation());
    }

    IEnumerator ButtonPressAnimation()
    {
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
            transform.localScale = Vector3.one * 0.9f;
            
            yield return new WaitForSeconds(0.1f);
            
            buttonImage.color = normalColor;
            transform.localScale = Vector3.one;
        }
    }

    // Opcional: Efectos al pasar el mouse
    public void OnPointerEnter()
    {
        if (buttonImage != null)
        {
            buttonImage.color = hoverColor;
        }
    }

    public void OnPointerExit()
    {
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }
}
