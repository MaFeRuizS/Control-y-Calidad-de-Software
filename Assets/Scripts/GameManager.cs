using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float elementFallSpeed = 2f;
    public int maxLives = 3;
    public int elementsPerLevel = 15;
    public float levelTimeLimit = 120f;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelText;
    public GameObject[] lifeHearts;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

    [Header("Gameplay")]
    public Transform spawnPoint;
    public Transform groundLimit;
    public GameObject elementPrefab;
    public Transform[] categoryButtons;

    // Game State
    private int currentScore = 0;
    private int currentLives;
    private int currentLevel = 1;
    private float timeRemaining;
    private int elementsClassified = 0;
    private bool isGameActive = false;

    // Current element
    private GameObject currentElement;
    private MathElement currentMathElement;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
            CheckElementPosition();
        }
    }

    public void InitializeGame()
    {
        currentScore = 0;
        currentLives = maxLives;
        currentLevel = 1;
        timeRemaining = levelTimeLimit;
        elementsClassified = 0;
        isGameActive = true;

        UpdateUI();
        SpawnNewElement();
    }

    void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            GameOver();
        }

        UpdateUI();
    }

    void CheckElementPosition()
    {
        if (currentElement != null && currentElement.transform.position.y <= groundLimit.position.y)
        {
            LoseLife();
            Destroy(currentElement);
            SpawnNewElement();
        }
    }

    public void SpawnNewElement()
    {
        if (elementsClassified >= elementsPerLevel)
        {
            LevelComplete();
            return;
        }

        // Destroy previous element if exists
        if (currentElement != null)
        {
            Destroy(currentElement);
        }

        // Create new element
        currentElement = Instantiate(elementPrefab, spawnPoint.position, Quaternion.identity);
        currentMathElement = currentElement.GetComponent<MathElement>();
        
        if (currentMathElement != null)
        {
            currentMathElement.Initialize();
        }
    }

    public void ClassifyElement(int selectedCategory)
    {
        if (currentMathElement == null || !isGameActive) return;

        bool isCorrect = currentMathElement.CheckAnswer(selectedCategory);

        if (isCorrect)
        {
            currentScore += 10;
            elementsClassified++;
            AudioManager.Instance?.PlayCorrectSound();
        }
        else
        {
            LoseLife();
            AudioManager.Instance?.PlayWrongSound();
        }

        UpdateUI();
        Destroy(currentElement);
        SpawnNewElement();
    }

    void LoseLife()
    {
        currentLives--;
        UpdateLivesUI();

        if (currentLives <= 0)
        {
            ShowRepechaje();
        }
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeHearts.Length; i++)
        {
            if (lifeHearts[i] != null)
            {
                lifeHearts[i].SetActive(i < currentLives);
            }
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + currentScore;

        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (levelText != null)
            levelText.text = "Nivel: " + currentLevel;
    }

    void ShowRepechaje()
    {
        isGameActive = false;
        // Aquí se mostraría la pregunta de repechaje
        // Por ahora, terminamos el juego
        GameOver();
    }

    void GameOver()
    {
        isGameActive = false;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        Debug.Log("Game Over! Puntuación final: " + currentScore);
    }

    void LevelComplete()
    {
        isGameActive = false;
        currentLevel++;
        
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        Debug.Log("¡Nivel " + (currentLevel - 1) + " completado!");
    }

    public void RestartGame()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        InitializeGame();
    }

    public void NextLevel()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        timeRemaining = levelTimeLimit;
        elementsClassified = 0;
        elementFallSpeed += 0.5f; // Incrementar dificultad
        isGameActive = true;

        SpawnNewElement();
    }

    public float GetFallSpeed()
    {
        return elementFallSpeed;
    }
}
