using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public bool gameStarted = false;
    public bool gameOver = false;

    [Header("Optional")]
    public GameObject startUI;
    public GameObject gameOverUI;
    public GameObject crackedUI;

    private void Awake()
    {
        // Simple singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Ensure game starts locked
        gameStarted = false;
        gameOver = false;

        if (startUI) startUI.SetActive(true);
        if (gameOverUI) gameOverUI.SetActive(false);
    }

    // 🔵 Call this from a button or input
    public void StartGame()
    {
        gameStarted = true;
        gameOver = false;

        if (startUI) startUI.SetActive(false);
    }

    // 🔴 Call this when egg breaks or falls
    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;
        gameStarted = false;

        if (gameOverUI) gameOverUI.SetActive(true);
    }


public void Cracked()
    {
          if (crackedUI) crackedUI.SetActive(true);
    }
    // 🔁 Jam-style instant restart
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
