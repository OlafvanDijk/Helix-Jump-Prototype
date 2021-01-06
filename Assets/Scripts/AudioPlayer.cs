using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> bounceSounds;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip finishSound;
    [SerializeField] private AudioClip scoreSound;

    private AudioSource audioSource;
    private int bounceIndex;

    /// <summary>
    /// Get audiosource component
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays the bounce sounds in order
    /// </summary>
    public void Bounce()
    {
        if (bounceSounds.Count == 0)
            return;

        if (bounceIndex >= bounceSounds.Count)
        {
            bounceIndex = 0;
        }

        PlayClip(bounceSounds[bounceIndex]);

        bounceIndex++;
    }

    /// <summary>
    /// Play the Game Over sound
    /// </summary>
    public void GameOver()
    {
        PlayClip(gameOverSound);
    }

    /// <summary>
    /// Play the Score sound
    /// </summary>
    internal void PlayScore()
    {
        PlayClip(scoreSound);
    }

    /// <summary>
    /// Play the Finish sound
    /// </summary>
    public void Finish()
    {
        PlayClip(finishSound);
    }

    /// <summary>
    /// Plays the given clip
    /// </summary>
    /// <param name="clip">Clip to play</param>
    private void PlayClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
