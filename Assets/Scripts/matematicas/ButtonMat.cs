using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMat : MonoBehaviour
{
    [Header("Referencias")]
    public Image buttonImage;
    
    [Header("Feedback Visual")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.gray;
    public Color pressedColor = Color.gray;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }

        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
    }

    void OnButtonClick()
    {
        // --- LA MAGIA ESTÁ AQUÍ ---
        // Llama al GameManager de Matemáticas y le envía su propia Etiqueta (Tag)
        if (GameManagerMatematicas.Instance != null)
        {
            GameManagerMatematicas.Instance.ClassifyElement(gameObject.tag);
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
}