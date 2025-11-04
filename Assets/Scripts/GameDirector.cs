using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    [Header("Gameplay")]
    public CatchColor targetColor = CatchColor.Green;
    [SerializeField] int correctPoints = 5;
    [SerializeField] int wrongPoints = -2;
    [SerializeField] float timeLimit = 60f;

    [Header("UI")]
    [SerializeField] TMP_Text txtScore;
    [SerializeField] TMP_Text txtTimer;
    [SerializeField] TMP_Text txtTarget;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text txtFinalScore;

    [Header("Audio")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip sfxCorrect;
    [SerializeField] AudioClip sfxWrong;
    [SerializeField] AudioClip sfxLose;

    int score = 0;
    float timeLeft;
    bool isOver;

    public static GameDirector I { get; private set; }

    void Awake()
    {
        I = this;
        targetColor = (CatchColor)Random.Range(0, 3); // feel fresh each run
        timeLeft = timeLimit;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (isOver) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame();
        }
        UpdateUI();

        // Simple pause (Esc)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool paused = Time.timeScale < 1f;
            Time.timeScale = paused ? 1f : 0f;
            if (gameOverPanel) gameOverPanel.SetActive(!paused);
        }
    }

    void UpdateUI()
    {
        if (txtScore) txtScore.text = $"Score: {score}";
        if (txtTimer) txtTimer.text = $"Time: {Mathf.CeilToInt(timeLeft)}";
        if (txtTarget) txtTarget.text = $"Target: {targetColor}";
    }

    public void HandleCollect(Collectible c)
    {
        if (isOver) return;

        bool correct = (c.MyColor == targetColor);
        score += correct ? correctPoints : wrongPoints;

        if (sfxSource)
            sfxSource.PlayOneShot(correct ? sfxCorrect : sfxWrong);

        UpdateUI();
    }

    void EndGame()
{
    Debug.Log("EndGame() fired");   // TEMP
    isOver = true;
    if (sfxSource && sfxLose) sfxSource.PlayOneShot(sfxLose);
    if (gameOverPanel)
    {
        gameOverPanel.SetActive(true);
        if (txtFinalScore) txtFinalScore.text = $"Final Score: {score}";
    }
    Time.timeScale = 0f;
}


    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
    public void PlayerCaught()
{
    if (isOver) return;   // ignore duplicates
    EndGame();
}



    // Enemy penalty hook (optional)
    public void HandleEnemyTouch()
    {
        timeLeft = Mathf.Max(0f, timeLeft - 3f);
        // Optionally: score -= 1;
        UpdateUI();
    }

}
