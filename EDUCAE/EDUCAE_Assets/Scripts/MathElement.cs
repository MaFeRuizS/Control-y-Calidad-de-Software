using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MathElement : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI operationText;
    public SpriteRenderer cardSprite;

    private int correctAnswer;
    private string operationString;

    // Lista de respuestas válidas (las categorías que tienes)
    private int[] validAnswers = { 4, 5, 6, 10 };

    void Update()
    {
        // Mover el elemento hacia abajo
        float speed = GameManager.Instance.GetFallSpeed();
        transform.position += Vector3.down * speed * Time.deltaTime;
    }

    public void Initialize()
    {
        GenerateOperation();
        UpdateVisuals();
    }

    void GenerateOperation()
    {
        // Seleccionar una respuesta válida aleatoria
        correctAnswer = validAnswers[Random.Range(0, validAnswers.Length)];

        // Generar operación basada en la respuesta
        int operationType = Random.Range(0, 4); // 0: suma, 1: resta, 2: multiplicación, 3: especial

        switch (operationType)
        {
            case 0: // Suma que da la respuesta correcta
                GenerateSuma();
                break;

            case 1: // Resta que da la respuesta correcta
                GenerateResta();
                break;

            case 2: // Multiplicación que da la respuesta correcta
                GenerateMultiplicacion();
                break;

            case 3: // Casos especiales (raíces, divisiones)
                GenerateEspecial();
                break;
        }
    }

    void GenerateSuma()
    {
        // Generar suma que resulte en correctAnswer
        int num1 = Random.Range(1, correctAnswer);
        int num2 = correctAnswer - num1;
        operationString = $"{num1} + {num2}";
    }

    void GenerateResta()
    {
        // Generar resta que resulte en correctAnswer
        int num1 = correctAnswer + Random.Range(1, 8);
        int num2 = num1 - correctAnswer;
        operationString = $"{num1} - {num2}";
    }

    void GenerateMultiplicacion()
    {
        // Solo para respuestas que tienen divisores convenientes
        if (correctAnswer == 4)
        {
            // 2 × 2 = 4 o 1 × 4 = 4
            if (Random.value > 0.5f)
                operationString = "2 × 2";
            else
                operationString = "1 × 4";
        }
        else if (correctAnswer == 5)
        {
            operationString = "1 × 5";
        }
        else if (correctAnswer == 6)
        {
            // 2 × 3 = 6 o 1 × 6 = 6
            if (Random.value > 0.5f)
                operationString = "2 × 3";
            else
                operationString = "1 × 6";
        }
        else if (correctAnswer == 10)
        {
            // 2 × 5 = 10 o 1 × 10 = 10
            if (Random.value > 0.5f)
                operationString = "2 × 5";
            else
                operationString = "1 × 10";
        }
    }

    void GenerateEspecial()
    {
        // Raíces cuadradas o divisiones
        if (correctAnswer == 4)
        {
            // √16 = 4 o 8 ÷ 2 = 4
            if (Random.value > 0.5f)
                operationString = "√16";
            else
                operationString = "8 ÷ 2";
        }
        else if (correctAnswer == 5)
        {
            // √25 = 5 o 10 ÷ 2 = 5
            if (Random.value > 0.5f)
                operationString = "√25";
            else
                operationString = "10 ÷ 2";
        }
        else if (correctAnswer == 6)
        {
            // √36 = 6 o 12 ÷ 2 = 6
            if (Random.value > 0.5f)
                operationString = "√36";
            else
                operationString = "12 ÷ 2";
        }
        else if (correctAnswer == 10)
        {
            // √100 = 10 o 20 ÷ 2 = 10
            if (Random.value > 0.5f)
                operationString = "√100";
            else
                operationString = "20 ÷ 2";
        }
    }

    void UpdateVisuals()
    {
        if (operationText != null)
        {
            operationText.text = operationString;
        }

        // Cambiar color de la tarjeta aleatoriamente
        if (cardSprite != null)
        {
            Color[] colors = {
                new Color(0.2f, 0.6f, 0.86f), // Azul
                new Color(0.18f, 0.8f, 0.44f), // Verde
                new Color(0.95f, 0.77f, 0.06f), // Amarillo
                new Color(0.61f, 0.35f, 0.71f)  // Morado
            };
            cardSprite.color = colors[Random.Range(0, colors.Length)];
        }
    }

    public bool CheckAnswer(int selectedAnswer)
    {
        return selectedAnswer == correctAnswer;
    }

    public int GetCorrectAnswer()
    {
        return correctAnswer;
    }

    void OnMouseDown()
    {
        // Opcional: permitir hacer clic en el elemento para seleccionarlo
        Debug.Log($"Elemento clickeado: {operationString} = {correctAnswer}");
    }
}