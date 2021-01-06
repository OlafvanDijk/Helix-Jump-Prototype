using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [Tooltip("Reference to the Finish UI. Used for Enabling the UI")]
    [SerializeField] private GameObject FinishUI;
    [Tooltip("Text field containing the number of the finished level")]
    [SerializeField] private TextMeshProUGUI levelPassed_Text;

    /// <summary>
    /// On reaching finish set the new level index and display the level number to the player
    /// </summary>
    public void ShowUI()
    {
        FinishUI.SetActive(true);
        int currentLevel = PlayerPrefs.GetInt("currentLevel") + 1;
        levelPassed_Text.text = $"Level {currentLevel} Passed!";
        PlayerPrefs.SetInt("currentLevel", currentLevel);
    }

    /// <summary>
    /// Reloads the scene. Spawnlayer will then get the currentLevel index from the Playerprefs while preparing the level.
    /// </summary>
    public void Continue()
    {
        SceneManager.LoadScene(0);
    }
}
