using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sfx, music;
    public AudioSource sfxSource, musicSource;
    public AudioMixerGroup[] channel;

    private AudioSource loopingSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Level Music");
       
    }

    public void PlaySfx(string name, int sourceIndex, bool loop = false)
    {
        Sound s = Array.Find(sfx, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.outputAudioMixerGroup = channel[sourceIndex];
            sfxSource.PlayOneShot(s.clip);
           /* AudioSource source = gameObject.AddComponent<AudioSource>();

            if (loop)
            {
                loopingSource = source;
                loopingSource.loop = true;
                loopingSource.Play();
            }
            else
            {
                source.PlayOneShot(s.clip);
                Destroy(source, s.clip.length); // Destroy the one-shot audio source after it finishes playing
            }
           */
        }
    }

    public void StopLoopingSfx()
    {
        if (loopingSource != null)
        {
            loopingSource.Stop();
            Destroy(loopingSource);
            loopingSource = null;
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(music, x => x.name == name);

        if (s == null)
        {
            Debug.Log("music sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    


}
