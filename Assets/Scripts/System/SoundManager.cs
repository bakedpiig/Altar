﻿using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    [SerializeField]
    private AudioSource efxSource;
    [SerializeField]
    private AudioSource musicSource;

    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        LoadSound();
    }

    private void LoadSound()
    {
        Object[] obj= Resources.LoadAll("Sounds");
        foreach(var audio in obj)
        {
            if(audio is AudioClip)
                soundDictionary.Add(audio.name ,(AudioClip)audio);
        }
    }

    public bool TryPlayingEffect(string audioName)
    {
        if(!soundDictionary.ContainsKey(audioName))
            return false;
        
        efxSource.clip = soundDictionary[audioName];
        efxSource.Play();
        return true;
    }

    public bool TryPlayingMusic(string audioName)
    {
        if(!soundDictionary.ContainsKey(audioName))
            return false;
        
        musicSource.clip = soundDictionary[audioName];
        musicSource.Play();
        return true;
    }

    public void Mute()
    {
        efxSource.mute = true;
        musicSource.mute = true;
    }

    public void Unmute()
    {
        efxSource.mute = false;
        musicSource.mute = false;
    }
}