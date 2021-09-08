using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.ServiceLocator;

public class SfxController : Service
{
    public void Play(string id)
    {
        if (audioManager == null)
            audioManager = AppRoot.Instance.GetService<AudioManager>();

        AudioClip clip = audioManager.GetSfx(id);
        audioSource.PlayOneShot(clip);
    }

    public void PlayAmbience(string id, bool loop = false)
    {
        if (audioManager == null)
            audioManager = AppRoot.Instance.GetService<AudioManager>();

        AudioClip clip = audioManager.GetSfx(id);
        ambienceSource.clip = clip;
        ambienceSource.loop = loop;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ambienceSource = GetComponentsInChildren<AudioSource>()[1];
    }

    private AudioSource audioSource, ambienceSource;
    private AudioManager audioManager;
}
