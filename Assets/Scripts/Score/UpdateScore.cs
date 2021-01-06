using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UpdateScore : MonoBehaviour
{
    private ScoreManager scoreManager;
    private AudioPlayer audioPlayer;
    private Transform gapTransform;

    private int scorePerGap;    

    /// <summary>
    /// Set transform
    /// </summary>
    private void Awake()
    {
        gapTransform = transform;
    }

    /// <summary>
    /// Set basic values
    /// </summary>
    /// <param name="scoreManager">Scoremanager so we can call AddScore when needed</param>
    /// <param name="scorePerGap">Amount of points we can give when adding to the score</param>
    /// <param name="audioPlayer">AudioPlayer to play an audiofragment</param>
    public void Init(ScoreManager scoreManager, int scorePerGap, AudioPlayer audioPlayer)
    {
        this.scoreManager = scoreManager;
        this.scorePerGap = scorePerGap;
        this.audioPlayer = audioPlayer;
    }

    /// <summary>
    /// If the player exits at the bottom then we can add to the score
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") && other.transform.position.y < gapTransform.position.y)
        {
            scoreManager.AddScore(scorePerGap);
            audioPlayer.PlayScore();
        }
    }
}
