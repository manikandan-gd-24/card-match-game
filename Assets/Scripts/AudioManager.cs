using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    /// <summary>
    /// AUDIO CLIPS ----------
    /// </summary>
    [Header("AUDIO CLIPS")]
    [Space(5)]
    [SerializeField] private AudioClip[] backgroundMusicClips;

    [SerializeField] private AudioClip cardFlip;
    [SerializeField] private AudioClip incorrectMatch;
    [SerializeField] private AudioClip correctMatch;
    [SerializeField] private AudioClip levelCompleted;

    /// <summary>
    /// AUDIO SOURCES ----------
    /// </summary>
    [Header("AUDIO SOURCE")]
    [Space(5)]
    [SerializeField] private AudioSource background_AudioSource;
    [SerializeField] private AudioSource sfx_AudioSource;

    /// <summary>
    /// INTEGERS ----------
    /// </summary>
    private int lastIndex = -1;   
        

    private void Start()
    {
        Init();

        Invoke("BindEvents", 2f);
        StartCoroutine(PlayBGMusicLoop());
    }

    private void BindEvents()
    {
        GameManager.Instance.PlaySfxAudio += PlaySfx;
        BoardManager.Instance.PlaySfxAudio += PlaySfx;
    }

    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //BindEvents();
    }

    

    private int GetRandomIndex()
    {
        int index;

        do
        {
            index = UnityEngine.Random.Range(0, backgroundMusicClips.Length);
        }
        while (index == lastIndex);

        lastIndex = index;
        return index;
    }

    private IEnumerator PlayBGMusicLoop()
    {
        while (true)
        {
            int randomIndex = GetRandomIndex();

            background_AudioSource.clip = backgroundMusicClips[randomIndex];
            background_AudioSource.Play();

            yield return new WaitForSeconds(background_AudioSource.clip.length);
        }
    }

    private void PlaySfx(string audiostate)
    {
        sfx_AudioSource.clip = null;

        switch (audiostate)
        {
            case "cardflipped":
                sfx_AudioSource.clip = cardFlip;
                sfx_AudioSource.Play();
                break;
            case "correctmatch":
                sfx_AudioSource.clip = correctMatch;
                sfx_AudioSource.Play();
                break;
            case "incorrectmatch":
                sfx_AudioSource.clip = incorrectMatch;
                sfx_AudioSource.Play();
                break;
            case "lvlcompleted":
                sfx_AudioSource.clip = levelCompleted;
                sfx_AudioSource.Play();
                break;
            default:
                DebugManager.Instance.Log($"Incorrect Audio state : {audiostate}");
                break;
        }
    }
}
