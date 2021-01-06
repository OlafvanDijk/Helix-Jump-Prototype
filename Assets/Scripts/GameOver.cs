using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    [Header("Script References")]
    [Tooltip("Reference to the ScoreManager so we can get the layersPassed and layerTransformParent values")]
    [SerializeField] private ScoreManager scoreManager;
    [Tooltip("Reference to the ScoreManager so we can get the current level")]
    [SerializeField] private SpawnLayers spawnLayers;

    [Header("UI")]
    [Tooltip("Text field displaying the percentage completed")]
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Events")]
    [SerializeField] private UnityEvent OnGameOver;
    [SerializeField] private UnityEvent OnRestart;

    /// <summary>
    /// Invoke OnGameOver which shows the UI
    /// Show the percentage completed
    /// </summary>
    public void ShowUI()
    {
        OnGameOver.Invoke();
        float layersPassed = scoreManager.GetLayersPassed();
        Level currentLevel = spawnLayers.GetCurrentLevel();
        float percentageCompleted = Mathf.Round(((layersPassed / currentLevel.amountOfTotalLayers) * 100));
        percentageText.text = percentageCompleted + "% COMPLETED";
    }

    /// <summary>
    /// Restarts level by loading the scene again
    /// </summary>
    public void RestartLevel()
    {
        OnRestart.Invoke();
        SceneManager.LoadScene(0);
    }
}
