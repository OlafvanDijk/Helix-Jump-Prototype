using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Tooltip("Text field displaying the current score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [Tooltip("Text field displaying the best score")]
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Space(5)]

    [Tooltip("Time before the multiplier resets")]
    [Range(0,3)]
    [SerializeField] private float multiplierTimer = 1f;

    private int score;
    private int bestScore;
    private int layersPassed;
    private int multiplier = 1;

    private bool multiply = false;    

    /// <summary>
    /// Set the best score field
    /// </summary>
    private void Awake()
    {
        bestScore = PlayerPrefs.GetInt("BestScore");
        bestScoreText.text = "BEST : " + bestScore;
    }

    /// <summary>
    /// Update layers passed
    /// Check current multiplier
    /// Add points to the score and update best score if needed
    /// </summary>
    /// <param name="points">Amount of base points to add</param>
    public void AddScore(int points)
    {
        layersPassed++;
        CheckMultiplier();
        points *= multiplier;

        score += points;
        scoreText.text = score.ToString();

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            bestScoreText.text = "BEST : " + bestScore;
        }
    }

    /// <summary>
    /// Returns the layers passed used to calculate the % completed when game over
    /// </summary>
    /// <returns>Amount of layers passed</returns>
    public int GetLayersPassed()
    {
        return this.layersPassed;
    }

    /// <summary>
    /// Check if the player is still in for the multiplier
    /// if so Stop the timer
    /// if not start measuring again
    /// </summary>
    private void CheckMultiplier()
    {
        if (multiply == true)
        {
            StopCoroutine(MultiplierTimer());
            multiplier *= 2;
        }

        if (multiplier == 1)
        {
            multiply = true;
        }
        StartCoroutine(MultiplierTimer());
    }

    /// <summary>
    /// After waiting for x seconds set multiply to false and reset the multiplier
    /// </summary>
    /// <returns></returns>
    private IEnumerator MultiplierTimer()
    {
        yield return new WaitForSecondsRealtime(multiplierTimer);
        multiply = false;
        multiplier = 1;
    }
}
